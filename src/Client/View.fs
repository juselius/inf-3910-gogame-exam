module View

open Feliz
open Feliz.Bulma
open Thoth.Json
open Shared
open Client

let render (model: Model) (dispatch: Msg -> unit) =
    Html.div [
        // This starts the websocket listener and forwards all messages to update
        WebSocket.webSocketController {
            OnMessage = fun msg ->
                match Decode.Auto.fromString<WebsocketMsg> msg with
                | Ok msg -> msg |> WebSocket |> dispatch
                | Error err -> printfn "ERROR: decode GameUpdateMsg: %A" err
        }
        Html.text "Under construction."
    ]