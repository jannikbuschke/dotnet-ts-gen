module TinyTypeGen.Giraffe

open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http

let endpoint<'request, 'response> (url: string) (verb: TsGen.HttpVerb) (handler: HttpHandler) =
  { TsGen.ApiEndpoint.Request = typeof<'request>
    TsGen.ApiEndpoint.Response = typeof<'response>
    TsGen.ApiEndpoint.Method = verb
    TsGen.ApiEndpoint.Route = url },
  handler

let queryEndpointWithoutInput<'response, 'error> (url: string) (handler: HttpContext -> unit -> Task<Result<'response, 'error>>) =
  endpoint<unit, 'response> url TsGen.HttpVerb.GET (fun next ctx ->
    task {
      let! response = handler ctx ()
      match response with
      | Result.Ok result -> return! json result next ctx
      | Result.Error error -> return! json error next ctx
    })

let queryEndpoint<'request, 'response, 'error> (url: string) (handler: HttpContext -> 'request -> Task<Result<'response, 'error>>) =
  endpoint<'request, 'response> url TsGen.HttpVerb.GET (fun next ctx ->
    task {
      let request = ctx.BindQueryString<'request>()
      let! response = handler ctx request

      match response with
      | Result.Ok result -> return! json result next ctx
      | Result.Error error -> return! json error next ctx
    })

let mutationEndpoint<'request, 'response, 'error> (url: string) (handler: HttpContext -> 'request -> Task<Result<'response, 'error>>) =
  endpoint<'request, 'response> url TsGen.HttpVerb.POST (fun next ctx ->
    task {
      let! request = ctx.BindJsonAsync<'request>()
      let! response = handler ctx request

      match response with
      | Result.Ok result -> return! json result next ctx
      | Result.Error error -> return! json error next ctx
    })

let toGiraffeEndpoint (e: TsGen.ApiEndpoint * HttpHandler) =
  let endpoint, f = e

  let method =
    match endpoint.Method with
    | TsGen.HttpVerb.GET -> Giraffe.Core.GET
    | TsGen.HttpVerb.POST -> Giraffe.Core.POST
    | TsGen.HttpVerb.PATCH -> Giraffe.Core.GET
    | _ -> failwith "NOT SUPPORTED"

  method >=> route endpoint.Route >=> f
