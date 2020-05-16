// DO NOT MODIFY
module GoGame.WebSocket

open System
open System.Text
open System.Net.WebSockets
open Microsoft.AspNetCore.Http
open System.Threading

// HttpContext.Connection.Id
type ClientId = string

type Message = string

// WebSocket connection messages
type WsMsg =
    | Broadcast of Message
    | Send of ClientId * Message
    | GetConnectedClients of AsyncReplyChannel<ClientId list>
    | ClientConnected of ClientId * WebSocket
    | ClientDisconnected of ClientId

type WebSocketHandler = HttpContext -> Message -> unit

// Table of all active connections
type private ConnTable = Map<ClientId, WebSocket>

type private ByteSeg = ArraySegment<byte>

type private Mailbox = MailboxProcessor<WsMsg>

let private transmitMsg (ws : WebSocket) (message : Message) =
    let buffer = Encoding.UTF8.GetBytes message
    let segment = ByteSeg buffer
    match ws.State with
    | WebSocketState.Open ->
        ws.SendAsync (
            segment,
            WebSocketMessageType.Text,
            true, CancellationToken.None)
        |> Async.AwaitTask
        |> ignore
    | state -> printfn "WARNING: unhandled send state: %A" state
    ws.State

let private connectionAgent () =
    Mailbox.Start (fun inbox ->
        let sendMsg (cid, ws) msg =
            match transmitMsg ws msg with
            | WebSocketState.Open -> ()
            | _ -> inbox.Post (ClientDisconnected cid)
        let rec loop (conntab : ConnTable) =
            async {
                printfn "ws: waiting for msg"
                match! inbox.Receive () with
                | Broadcast msg ->
                    printfn "ws: bcast: %s" msg
                    conntab |> Seq.iter (fun x -> sendMsg (x.Key, x.Value) msg)
                    do! loop conntab
                | Send (cid, msg) ->
                    printfn "ws: send: %s %s" cid msg
                    match conntab.TryFind cid with
                    | Some ws -> sendMsg (cid, ws) msg
                    | None -> printfn "WARNING: ws: send: no such cid %s" cid
                    do! loop conntab
                | GetConnectedClients reply ->
                    reply.Reply (conntab |> Map.toList |> List.map fst)
                | ClientConnected (cid, ws) ->
                    printfn "ws: connected: %s" cid
                    Thoth.Json.Net.Encode.Auto.toString (4, Shared.Channel cid)
                    |> sendMsg (cid, ws)
                    do! loop (Map.add cid ws conntab)
                | ClientDisconnected cid ->
                    printfn "ws: disconnected %s" cid
                    do! loop (Map.remove cid conntab)
            }
        loop Map.empty
    )

// Create a new websocket connection of the specified type
let private newWsConnection (ctx : HttpContext) =
    printfn "INFO: IsWebSocketRequest: %A" ctx.WebSockets.IsWebSocketRequest
    async {
        match ctx.WebSockets.IsWebSocketRequest with
        | true ->
            let! webSocket =
                ctx.WebSockets.AcceptWebSocketAsync()
                |> Async.AwaitTask
            return Some webSocket
        | false ->
            ctx.Response.StatusCode <- 400
            return None
    }

let private newConnectionHandler (ctx : HttpContext) (agent : Mailbox) handler =
    async {
        let cid = ctx.Connection.Id
        let recv : byte [] = Array.zeroCreate 4096
        let! ct = Async.CancellationToken
        let handle (webSocket : WebSocket) =
            let mutable cycle = true
            async {
                while cycle do
                    let! result =
                        webSocket.ReceiveAsync (ByteSeg recv, ct)
                        |> Async.AwaitTask
                    if result.CloseStatus.HasValue then
                        cycle <- false
                    else
                        recv
                        |> Encoding.UTF8.GetString
                        |> function
                        | "Close" ->
                            agent.Post (ClientDisconnected cid)
                            cycle <- false
                        | msg ->
                            handler ctx msg
            }
        match! newWsConnection ctx with
        | Some ws ->
            agent.Post (ClientConnected (cid, ws))
            do! handle ws
        | None -> ()
    }

let WS = connectionAgent ()

type WebSocketMiddleware(next : RequestDelegate, path : string, handler : WebSocketHandler) =
    member __.Invoke(ctx : HttpContext) =
        async {
            if ctx.Request.Path = PathString path then
                let cid = ctx.Connection.Id
                printfn "INFO: new ws connection %s" cid
                do! newConnectionHandler ctx WS handler
            else
                return! next.Invoke ctx |> Async.AwaitTask
        } |> Async.StartAsTask
