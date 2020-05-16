module Main

open Fable.Core.JsInterop

importAll "./style.scss"

open Elmish
open Elmish.React
open Elmish.Debug
open Elmish.HMR
open Client

// App
Program.mkProgram init update View.render
|> Program.withReactSynchronous "feliz-app"
|> Program.run
