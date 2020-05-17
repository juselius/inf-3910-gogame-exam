// DO NOT MODIFY
module Client.WebSocket

open FSharp.Control
open Fable.Reaction
open Fable.React

type Model = string

type Message = string

type Msg =
    | Encode of string
    | Decode of Model

type Props = {
    OnMessage : string -> unit
}

let update (props : Props) (model : Model) (msg : Msg) =
    match msg with
    | Encode _ -> ""
    | Decode x -> props.OnMessage x;  ""

let encode msg =
    printfn "WS: encode: %A" msg
    match msg with
    | Encode s ->  s
    | msg ->
        printfn "WARNING: encode: unknown message %A" msg
        ""

let decode str =
    printfn "WS: decode: %A" str
    Decode str |> Some

let stream _ msgs =
    msgs
    |> AsyncRx.msgChannel "ws://localhost:8080/ws" encode decode
    |> AsyncRx.tag "msgs"

let webSocketController (props : Props) =
    let controller (_ : Props) =
        let initialModel = ""
        let model = Hooks.useReducer (update props, initialModel)
        let dispatch, _ =
            Reaction.useStatefulStream (
                model.current,
                model.update,
                stream)
        span [] []
    FunctionComponent.Of controller props
