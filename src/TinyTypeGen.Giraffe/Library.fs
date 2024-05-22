module TinyTypeGen.Giraffe

open System.Collections.Generic
open System.Linq
open System.Net
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
      | Result.Error error ->
        return! (setStatusCode ((int) HttpStatusCode.BadRequest) >=> json error) next ctx
    })

let queryEndpoint<'request, 'response, 'error> (url: string) (handler: HttpContext -> 'request -> Task<Result<'response, 'error>>) =
  endpoint<'request, 'response> url TsGen.HttpVerb.GET (fun next ctx ->
    task {
      let request = if typeof<'request> = typeof<unit> then Unchecked.defaultof<'request> else ctx.BindQueryString<'request>()
      let! response = handler ctx request

      match response with
      | Result.Ok result -> return! json result next ctx
      | Result.Error error ->
        return! (setStatusCode ((int) HttpStatusCode.BadRequest) >=> json error) next ctx
    })

type PaginatedResult<'response> = {
  Items: IEnumerable<'response>
  Total: int
}

let tryReadStreamId (request: HttpRequest)  =
  request.Headers["X-App-StreamId"]
  |> Seq.tryHead

let tryReadStreamIdAsGuid (request: HttpRequest)  =
  request
  |> tryReadStreamId
  |> Option.bind (fun x ->
    match x |> System.Guid.TryParse with
    | true, id -> Some id
    | false, _ -> None)

let tryReadAsInt (name: string) (request: HttpRequest)  =
  request.Headers[name]
  |> Seq.tryHead
  |> Option.map (fun x ->
    match x |> System.Int32.TryParse with
    | true, id -> Some id
    | false, _ -> None)
  |> Option.flatten

let tryReadPageAndPageSize (request: HttpRequest)  =
  let page = tryReadAsInt "X-App-Page" request
  let pageSize = tryReadAsInt "X-App-PageSize" request
  page, pageSize

let readPageAndPageSizeOrDefault (request: HttpRequest) (defaultPage, defaultPageSize) =
  request |> tryReadPageAndPageSize |> (fun (page, pageSize) -> page |> Option.defaultValue defaultPage, pageSize |> Option.defaultValue defaultPageSize)

let paginatedQueryEndpoint<'request, 'response, 'error> (url: string) (createQuery: HttpContext -> 'request -> ((unit -> Task<int>) * ((int * int) -> Task<IEnumerable<'response>>))) =
  endpoint<'request, PaginatedResult<'response>> url TsGen.HttpVerb.GET (fun next ctx ->
    task {
      // bind query string with json?
      let request = if typeof<'request> = typeof<unit> then Unchecked.defaultof<'request> else ctx.BindQueryString<'request>()
      let count, query = createQuery ctx request
      let! count = count()
      let page, pageSize = readPageAndPageSizeOrDefault ctx.Request (1, 10)
      let! items = query(page,pageSize)
      return! json ({ Items = items; Total = count }) next ctx
    })

let mutationEndpoint<'request, 'response, 'error> (url: string) (handler: HttpContext -> 'request -> Task<Result<'response, 'error>>) =
  endpoint<'request, 'response> url TsGen.HttpVerb.POST (fun next ctx ->
    task {
      let! request = ctx.BindJsonAsync<'request>()
      let! response = handler ctx request

      match response with
      | Result.Ok result -> return! json result next ctx
      | Result.Error error ->
        return! (setStatusCode ((int) HttpStatusCode.BadRequest) >=> json error) next ctx
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
