module GoGame.Api

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.V2
open Giraffe
open Thoth.Json.Net
open GoGame.WebSocket
open Shared

let handleExample next ctx =
    let encoder = Encode.Auto.generateEncoder<WebsocketMsg> ()
    task {
        let msg =
            Example "hello world!"
            |> encoder
            |> Encode.toString 4
        WS.Post (Broadcast msg)
        return! json "ok" next ctx
    }
