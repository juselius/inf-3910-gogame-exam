module Shared

type ConnectionId = string

type WebsocketMsg =
    | Example of string
    | Channel of ConnectionId

let defaultGridSize = 11
