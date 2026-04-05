module TinyTypeGen.Giraffe

open System.Collections.Generic
open System.Net
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open System.Linq

let endpoint<'request, 'response> (url: string) (verb: HttpVerb) (handler: HttpHandler) =
  let kind =
    match verb with
    | HttpVerb.GET -> EndpointKind.Query
    | _ -> EndpointKind.Mutation

  {
    ApiEndpoint.Request = typeof<'request>
    ApiEndpoint.Response = typeof<'response>
    ApiEndpoint.Error = []
    ApiEndpoint.Method = verb
    ApiEndpoint.Route = url
    ApiEndpoint.ResponseNullable = false
    ApiEndpoint.Kind = kind
  },
  handler

let queryEndpointWithoutInput<'response, 'error>
  (url: string)
  (handler: HttpContext -> unit -> Task<Result<'response, 'error>>)
  =
  endpoint<unit, 'response>
    url
    HttpVerb.GET
    (fun next ctx ->
      task {
        let! response = handler ctx ()

        match response with
        | Ok result -> return! json result next ctx
        | Error error -> return! (setStatusCode (int HttpStatusCode.BadRequest) >=> json error) next ctx
      }
    )

let queryEndpoint<'request, 'response, 'error>
  (url: string)
  (handler: HttpContext -> 'request -> Task<Result<'response, 'error>>)
  =
  endpoint<'request, 'response>
    url
    HttpVerb.GET
    (fun next ctx ->
      task {
        let request =
          if typeof<'request> = typeof<unit> then
            Unchecked.defaultof<'request>
          else
            ctx.BindQueryString<'request>()

        let! response = handler ctx request

        match response with
        | Ok result -> return! json result next ctx
        | Error error -> return! (setStatusCode (int HttpStatusCode.BadRequest) >=> json error) next ctx
      }
    )

type PaginatedResult<'response> =
  {
    Items: IEnumerable<'response>
    Total: int
  }

let tryReadStreamId (request: HttpRequest) =
  request.Headers["X-App-StreamId"] |> Seq.tryHead

let tryReadStreamIdAsGuid (request: HttpRequest) =
  request
  |> tryReadStreamId
  |> Option.bind (fun x ->
    match x |> System.Guid.TryParse with
    | true, id -> Some id
    | false, _ -> None
  )

let tryReadAsInt (name: string) (request: HttpRequest) =
  request.Headers[name]
  |> Seq.tryHead
  |> Option.map (fun x ->
    match x |> System.Int32.TryParse with
    | true, id -> Some id
    | false, _ -> None
  )
  |> Option.flatten

let tryReadPageAndPageSize (request: HttpRequest) =
  let page = tryReadAsInt "X-App-Page" request
  let pageSize = tryReadAsInt "X-App-PageSize" request
  page, pageSize

let readPageAndPageSizeOrDefault (request: HttpRequest) (defaultPage, defaultPageSize) =
  request
  |> tryReadPageAndPageSize
  |> fun (page, pageSize) -> page |> Option.defaultValue defaultPage, pageSize |> Option.defaultValue defaultPageSize

let paginatedQuery countAsync toListAsync (query: IQueryable<'a>) (page: uint) (size: uint) =
  task {
    let! count = query |> countAsync
    let skip = int (page * size)
    let take = int size
    let! entities = query.Skip(skip).Take take |> toListAsync
    return
      {
        Items = entities
        Total = count
      }
  }

let paginatedQueryEndpoint<'request, 'response, 'error>
  (url: string)
  (createQuery: HttpContext -> 'request -> ((unit -> Task<int>) * ((int * int) -> Task<IEnumerable<'response>>)))
  =
  endpoint<'request, PaginatedResult<'response>>
    url
    HttpVerb.GET
    (fun next ctx ->
      task {
        // bind query string with json?
        let request =
          if typeof<'request> = typeof<unit> then
            Unchecked.defaultof<'request>
          else
            ctx.BindQueryString<'request>()

        let count, query = createQuery ctx request
        let! count = count ()
        let page, pageSize = readPageAndPageSizeOrDefault ctx.Request (1, 10)
        let! items = query (page, pageSize)
        return!
          json
            {
              Items = items
              Total = count
            }
            next
            ctx
      }
    )

let mutationEndpoint<'request, 'response, 'error>
  (url: string)
  (handler: HttpContext -> 'request -> Task<Result<'response, 'error>>)
  =
  endpoint<'request, 'response>
    url
    HttpVerb.POST
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

let toGiraffeEndpoint (e: ApiEndpoint * HttpHandler) =
  let endpoint, f = e

  let method =
    match endpoint.Method with
    | HttpVerb.GET -> GET
    | HttpVerb.POST -> POST
    // | HttpVerb.PATCH -> GET
    | _ -> failwith "NOT SUPPORTED"

  method >=> route endpoint.Route >=> f
