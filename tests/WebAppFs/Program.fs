open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Giraffe
open Giraffe.EndpointRouting
open TinyTypeGen.GiraffeEndpointRouting
open FsToolkit.ErrorHandling
open System.Threading.Tasks
open System.Net
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Http.Metadata

type Program() = class end // Dummy type for WebApplicationFactory<Program>

/// your error cases
type ApiError =
  | NotFound of string
  | NotEditable
  | BadRequest of string list

// copy this into your project and adjust and use it to make a unified api
let handler path (handler: HttpContext -> 'request -> Task<Result<'response, ApiError>>) =
  let errorToStatusCode =
    function
    | NotFound _ -> HttpStatusCode.NotFound
    | NotEditable -> HttpStatusCode.Locked
    | BadRequest _ -> HttpStatusCode.BadRequest
  routeWithMetadata
    path
    [|
      acceptJson typeof<'response>
      producesResponseType200 typeof<'response>
      // adjust error codes as needed
      producesProblemDetails HttpStatusCode.BadRequest
      producesProblemDetails HttpStatusCode.NotFound
      producesProblemDetails HttpStatusCode.InternalServerError
    |]
    (fun next ctx ->
      task {
        try
          let! request = ctx.BindJsonAsync<'request>()
          let! response = handler ctx request

          match response with
          | Ok result -> return! json result next ctx
          | Error error -> return! (setStatusCode (int (errorToStatusCode error)) >=> json error) next ctx
        with e ->
          // log exception, adjust problem details...
          return!
            (setStatusCode (int HttpStatusCode.InternalServerError)
             >=> json (ProblemDetails(Title = "Internal error")))
              next
              ctx
      }
    )

// Request/Response can be named types or anonmous records
[<CLIMutable>]
type CreateUserResponse =
  {
    Id: int
    Name: string
  }

// A fully typed Giraffe Endpoint with metadata for input and output inferred
let createUser =
  handler
    "/create"
    (fun httpContext (request: {| UserName: string |}) ->
      taskResult {
        do!
          String.IsNullOrEmpty request.UserName
          |> Result.requireTrue (BadRequest [ "UserName is required" ])
        // ... save
        return
          {
            Id = 1
            Name = request.UserName
          }
      }
    )

[<EntryPoint>]
let main args =
  let builder = WebApplication.CreateBuilder args
  builder.Services.AddGiraffe().AddEndpointsApiExplorer() |> ignore
  let app = builder.Build()
  app.UseRouting() |> ignore
  let endpoints: Endpoint list =
    [
      POST [ subRoute "/api" [ subRoute "/user" [ createUser ] ] ]
      GET
        [
          subRoute
            "/api"
            [
              subRoute
                "/user"
                [
                  // adding metadata per endpoint using Giraffe 'routeWithExtensions'
                  routeWithExtensions
                    (fun config ->
                      config
                        .WithMetadata(AcceptsMetadata([| "application/json" |], typeof<string>, isOptional = false))
                        .WithMetadata(ProducesResponseTypeMetadata(200, typeof<string>))
                    )
                    "/get-name"
                    (text "xxx")
                ]
            ]
        ]
    ]

  app.UseEndpoints(fun app -> app.MapGiraffeEndpoints endpoints) |> ignore

  app.Run()

  0
