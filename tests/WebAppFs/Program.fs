open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.Metadata
open Microsoft.AspNetCore.Mvc.ApiExplorer
open Microsoft.AspNetCore.Routing
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open Giraffe.EndpointRouting
open TinyTypeGen
open TinyTypeGen.GiraffeEndpointRouting
open FsToolkit.ErrorHandling
type Program() = class end // Dummy type for WebApplicationFactory<Program>

let createUser =
  mutation "/create" (fun ctx (request: {| UserName: string |}) -> taskResult { return "" })

[<CLIMutable>]
type CreateUserInput =
  {
    Name: string
    Email: string
  }

[<CLIMutable>]
type CreateUserOutput =
  {
    Id: int
    Name: string
  }

let createUserHandler: HttpHandler =
  bindJson<CreateUserInput> (fun input ->
    let output =
      {
        Id = 1
        Name = input.Name
      }
    json output
  )

let sayHelloWorld: HttpHandler = text "Hello World, from Giraffe"

[<EntryPoint>]
let main args =
  let builder = WebApplication.CreateBuilder(args)
  builder.Services.AddGiraffe().AddEndpointsApiExplorer().AddControllers()
  |> ignore
  let app = builder.Build()
  app.UseRouting() |> ignore
  let endpoints: Endpoint list =
    [
      POST [ subRoute "/api" [ subRoute "/user" [ createUser ] ] ]
      GET
        [
        // route "/" (text "Hello World")
        // routef "/users/%i" (fun id -> json {| UserId = id |})
        // route "hello" sayHelloWorld
        // (routeWithExtensions
        //   (_.WithName("create user name")
        //     .WithMetadata(AcceptsMetadata([| "application/json" |], typeof<{| X: string |}>, false))
        //     .WithMetadata(AcceptsMetadata([| "application/json" |], typeof<{| X: string |}>, false)))
        //   "/create-user"
        //   createUserHandler)
        ]
    // POST [
    //   route "/users" createUserHandler
    // ]
    ]

  app.UseEndpoints(fun app ->

    // app.MapControllers() |> ignore
    app.MapGiraffeEndpoints endpoints

    app.MapGet("/hello", Func<string>(fun () -> "Hello World!")) |> ignore
    app
      .MapPost("/anon0", Func<{| X: string |}, {| X: string |}>(fun (x) -> {| X = "Hello World!" |}))
      .Accepts<{| X: string |}>("application/json")
      .Produces<int>()
      .WithMetadata("test")
      .Produces<bool>()
    |> ignore
    app
      .MapGet("/anon", Func<{| X: string |}>(fun () -> {| X = "Hello World!" |}))
      .Produces<int>()
      .WithMetadata("test")
      .Produces<bool>()
    |> ignore

    app.MapPost("/anon2", Func<{| Count: int |}, {| X: string |}>(fun (count) -> {| X = "Hello World!" |}))
    |> ignore
  )
  |> ignore

  app.Run()

  0 // Exit code
