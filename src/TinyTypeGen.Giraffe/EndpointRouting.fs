module TinyTypeGen.GiraffeEndpointRouting

open System.Net
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Giraffe.EndpointRouting

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http.Metadata

let mutation<'request, 'response, 'error>
  (path: string)
  (handler: HttpContext -> 'request -> Task<Result<'response, 'error>>)
  =
  routeWithExtensions
    _.WithMetadata(AcceptsMetadata([| "application/json" |], typeof<'request>, false))
      .WithMetadata(ProducesResponseTypeMetadata(200, typeof<'response>))
    path
    (fun next ctx ->
      task {
        try
          let! request = ctx.BindJsonAsync<'request>()
          let! response = handler ctx request

          match response with
          | Ok result -> return! json result next ctx
          | Error error -> return! (setStatusCode (int HttpStatusCode.BadRequest) >=> json error) next ctx
        with e ->
          return!
            (setStatusCode (int HttpStatusCode.InternalServerError)
             >=> json {| Error = e.Message |})
              next
              ctx
      }
    )
