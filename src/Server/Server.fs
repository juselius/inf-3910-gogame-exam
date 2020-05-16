open System
open System.IO

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Shared

open GoGame.Api
open GoGame.WebSocket

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath =
    tryGetEnv "CONTENT_ROOT"
    |> function
    | Some root -> Path.GetFullPath root
    | None -> Path.GetFullPath "../Client/public"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us

let webApp : HttpHandler =
    choose [
        route "/api/example" >=> handleExample
    ]

let wsHandler (ctx : HttpContext) (msg : Message) =
    printfn "wsHandler: receive: %s" msg

let jsonSerializer = Thoth.Json.Giraffe.ThothSerializer ()

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore
    services.AddSingleton<Serialization.Json.IJsonSerializer>(jsonSerializer) |> ignore

let configureApp (app : IApplicationBuilder) =
    app.UseDefaultFiles()
        .UseStaticFiles()
        .UseWebSockets()
        .UseMiddleware<WebSocketMiddleware>("/ws", wsHandler)
        .UseGiraffe webApp

WebHost
    .CreateDefaultBuilder()
    .UseWebRoot(publicPath)
    .UseContentRoot(publicPath)
    .Configure(Action<IApplicationBuilder> configureApp)
    .ConfigureServices(configureServices)
    .UseUrls("http://0.0.0.0:" + port.ToString() + "/")
    .Build()
    .Run()
