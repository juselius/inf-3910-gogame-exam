module Client

open System
open Elmish
open Thoth.Json
open Thoth.Fetch
open Shared

type Msg =
    | Init
    | WebSocket of WebsocketMsg

let init () =
    (), Cmd.none

let handleWebSocket (model : Model) =
    function
    | Example s ->
        printfn "Received from websocket: %s" s
        model, Cmd.none
    | Channel _ -> model, Cmd.none

let update (msg: Msg) (model : Model) =
    match msg with
    | Init  -> model, Cmd.none
    | WebSocket x -> handleWebSocket model x
