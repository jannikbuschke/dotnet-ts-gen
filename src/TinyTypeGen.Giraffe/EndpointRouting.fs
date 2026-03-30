module TinyTypeGen.GiraffeEndpointRouting

open System.Net
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Giraffe.EndpointRouting
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http.Metadata

let routeWithMetadata route (configurations: obj array) =
  routeWithExtensions (fun builder -> configurations |> Array.fold (fun (acc: IEndpointConventionBuilder) metadata -> acc.WithMetadata(metadata)) builder) route

let acceptJson inputType =
  AcceptsMetadata([| "application/json" |], inputType, isOptional = false) :> obj

let producesResponseType200 outputType =
  ProducesResponseTypeMetadata(200, outputType)

let producesProblemDetails (code: HttpStatusCode) =
  ProducesResponseTypeMetadata(int code, typeof<Microsoft.AspNetCore.Mvc.ProblemDetails>)

/// adds AcceptsMetadata json input type and
/// Produces response type with 200
/// to a HttpFunc
// let json200 inputType outputType =
//   routeWithExtensions
//     _.WithMetadata(AcceptsMetadata([| "application/json" |], inputType, isOptional = false))
//       .WithMetadata(ProducesResponseTypeMetadata(200, outputType))
//
// let post2<'request, 'response, 'error>
//   (path: string)
//   (handler: HttpContext -> 'request -> Task<Result<'response, 'error>>)
//   =
//   json200
//     typeof<'request>
//     typeof<'response>
//     path
//     (fun next ctx ->
//       task {
//         try
//           let! request = ctx.BindJsonAsync<'request>()
//           let! response = handler ctx request
//
//           match response with
//           | Ok result -> return! json result next ctx
//           | Error error -> return! (setStatusCode (int HttpStatusCode.BadRequest) >=> json error) next ctx
//         with e ->
//           return!
//             (setStatusCode (int HttpStatusCode.InternalServerError)
//              >=> json {| Error = e.Message |})
//               next
//               ctx
//       }
//     )
//
// let post<'request, 'response, 'error>
//   (path: string)
//   (handler: HttpContext -> 'request -> Task<Result<'response, 'error>>)
//   =
//   json200
//     typeof<'request>
//     typeof<'response>
//     path
//     (fun next ctx ->
//       task {
//         try
//           let! request = ctx.BindJsonAsync<'request>()
//           let! response = handler ctx request
//
//           match response with
//           | Ok result -> return! json result next ctx
//           | Error error -> return! (setStatusCode (int HttpStatusCode.BadRequest) >=> json error) next ctx
//         with e ->
//           return!
//             (setStatusCode (int HttpStatusCode.InternalServerError)
//              >=> json {| Error = e.Message |})
//               next
//               ctx
//       }
//     )
