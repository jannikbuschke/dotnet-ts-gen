module TinyTypeGen.AspNetCore

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.Metadata
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Controllers
open Microsoft.AspNetCore.Mvc.Infrastructure
open Microsoft.AspNetCore.Routing
open Microsoft.Extensions.DependencyInjection
open System.Linq
open System.Collections.Generic
open System.Net

let verb =
  function
  | "POST"
  | "post" -> HttpVerb.POST
  | "GET"
  | "get" -> HttpVerb.GET
  | "PATCH"
  | "patch" -> HttpVerb.PATCH
  | "DELETE"
  | "delete" -> HttpVerb.DELETE
  | "PUT"
  | "put" -> HttpVerb.PUT
  | _ -> HttpVerb.GET

let getPredefinedTypes () =
  typedefof<ActionResult>, PredefinedType.New "any"

// get endpoints based controllers
let getLegacyEndpoints (services: IServiceProvider) =
  let adcp = services.GetRequiredService<IActionDescriptorCollectionProvider>()
  let descriptors = adcp.ActionDescriptors.Items.OfType<ControllerActionDescriptor>()

  let wrapperTypes =
    [ typeof<Task<_>>.GetGenericTypeDefinition()
      typeof<ValueTask<_>>.GetGenericTypeDefinition()
      typeof<Microsoft.AspNetCore.Mvc.ActionResult<_>>.GetGenericTypeDefinition() ]

  let rec toResultType (input: Type) =
    if
      input.IsGenericType
      && not input.IsGenericTypeDefinition
      && wrapperTypes.Contains(input.GetGenericTypeDefinition())
    then
      let args = input.GenericTypeArguments
      toResultType (args[0])
    else
      input

  let isCollection (t: Type) =
    t.GetInterfaces()
    |> Array.exists (fun i ->
      i.IsGenericType
      && (i.GetGenericTypeDefinition() = typedefof<IEnumerable<_>>
          || i.GetGenericTypeDefinition() = typedefof<ICollection<_>>
          || i.GetGenericTypeDefinition() = typedefof<IList<_>>))

  let getInputType (nameSpace: string) (name: string) (verb: HttpVerb) (parameters: Reflection.ParameterInfo list) =

    match parameters with
    | [] -> typeof<Unit>
    | [ p ] ->
      let t = p.ParameterType

      // if primitive or (collection and get) generate dynamic request type
      // or if enum
      if
        t.IsPrimitive
        || t.IsEnum && verb = HttpVerb.GET
        || isCollection t && verb = HttpVerb.GET
      then

        let builder = sprintf "%s_%s" nameSpace name |> DynamicType.createBuilder

        builder |> DynamicType.createProperty (p.Name, p.ParameterType, false)

        DynamicType.build builder
      else
        p.ParameterType
    | parameters ->
      let builder = sprintf "%s_%s" nameSpace name |> DynamicType.createBuilder

      parameters
      |> List.iter (fun x ->
        let isNullable =
          x.CustomAttributes
          |> Seq.exists (fun a -> a.AttributeType = typeof<Runtime.CompilerServices.NullableAttribute>)

        DynamicType.createProperty (x.Name, x.ParameterType, isNullable) builder)

      DynamicType.build builder

  descriptors
  |> Seq.filter (fun x -> x.AttributeRouteInfo <> null)
  |> Seq.choose (fun x ->
    let methods =
      x.EndpointMetadata.OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
      |> Seq.collect _.HttpMethods

    let nameSpaceForDynamics =
      x.ControllerTypeInfo.Namespace
      |> Option.ofObj
      |> Option.map (sprintf "%s.")
      |> Option.defaultValue ""

    // TODO: do handle more gracefully
    let method = methods.FirstOrDefault() |> nonNull
    let verb = verb method
    let parameters = x.MethodInfo.GetParameters() |> Seq.toList

    let name = sprintf "%s_%s_Request" nameSpaceForDynamics x.ActionName

    let returnTypeNullable =
      x.MethodInfo.ReturnParameter.CustomAttributes
      |> Seq.exists (fun a -> a.AttributeType = typeof<Runtime.CompilerServices.NullableAttribute>)

    let inputType = parameters |> getInputType nameSpaceForDynamics name verb

    let kind =
      if verb = HttpVerb.GET then
        EndpointKind.Query
      else
        EndpointKind.Mutation

    x.AttributeRouteInfo
    |> Option.ofObj
    |> Option.map _.Template
    |> Option.map (fun template ->
      ApiEndpoint.New(verb, sprintf "/%s" template, inputType, toResultType x.MethodInfo.ReturnType, kind)))
  |> Seq.distinctBy (fun x -> x.Route, x.Method)
  |> Seq.toList

let getEndpoints (services: IServiceProvider) t =
  let endpointDataSource = services.GetRequiredService<EndpointDataSource>()

  endpointDataSource.Endpoints
  |> Seq.choose (fun e ->
    let routeEndpoint = e :?> RouteEndpoint

    let methods =
      routeEndpoint.Metadata.OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
      |> Seq.collect _.HttpMethods


    // TODO: do handle more gracefully
    let method = methods.FirstOrDefault() |> Option.ofObj
    let verb = method |> Option.map verb // verb method
    let route = routeEndpoint.RoutePattern
    let routeText = route.RawText |> Option.ofObj

    let accepts =
      e.Metadata.GetMetadata<IAcceptsMetadata>()
      |> Option.ofObj
      |> Option.bind (fun x -> x.RequestType |> Option.ofObj)

    let isSuccessStatusCode code = code >= 200 && code < 300
    // for now only one success type is supported
    let produces =
      e.Metadata.OfType<ProducesResponseTypeMetadata>()
      |> Seq.choose (fun x ->
        if isSuccessStatusCode x.StatusCode then
          x.Type |> Option.ofObj
        else
          None)
      |> Seq.tryHead

    printfn " "
    printfn "===="
    printfn "%A => %A" accepts produces


    let producesErrors =
      e.Metadata.OfType<ProducesResponseTypeMetadata>()
      |> Seq.choose (fun x ->
        if not (isSuccessStatusCode x.StatusCode) then
          x.Type
          |> Option.ofObj
          |> Option.map (fun x' -> x', enum<HttpStatusCode> x.StatusCode)
        else
          None)
      |> Seq.toList

    match verb, routeText, accepts, produces with
    | Some verb, Some routeText, Some accepts, Some produces ->
      Some(e, verb, routeText, accepts, produces, producesErrors)
    | _, _, _, _ -> None)
  |> Seq.map (fun (routeEndpoint, verb, routeText, accepts, produces, errors) ->
    printfn "[%A=>%A]\n" accepts.Name produces.Name

    // let routeEndpoint = e :?> RouteEndpoint

    // let methods =
    //   routeEndpoint.Metadata.OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
    //   |> Seq.collect _.HttpMethods
    //
    //
    // // TODO: do handle more gracefully
    // let method = methods.FirstOrDefault() |> nonNull
    // let verb = verb method
    // let route = routeEndpoint.RoutePattern
    // let routeText = route.RawText |> Option.ofObj
    // let accepts =
    //   e.Metadata.GetMetadata<IAcceptsMetadata>()
    //   |> Option.ofObj
    //   |> Option.bind (fun x -> x.RequestType |> Option.ofObj)
    // let produces =
    //   e.Metadata.GetMetadata<ProducesResponseTypeMetadata>()
    //   |> Option.ofObj
    //   |> Option.bind (fun x -> x.Type |> Option.ofObj)

    // let tags = e.Metadata.GetMetadata<TagsAttribute>() |> Option.ofObj
    // let itags = e.Metadata.GetMetadata<ITagsMetadata>() |> Option.ofObj
    // printfn "Tags %A" tags
    // printfn "Tagsi %A" itags

    let kind =
      t
      |> Option.map (fun t -> t verb routeText (accepts, produces))
      |> Option.defaultValue EndpointKind.Mutation

    ApiEndpoint.New(verb, routeText, accepts, produces, kind, errors)

  )
  |> Seq.toList

let getEndpointsBasedOnEndpointDataSource (services: IServiceProvider) = getEndpoints services None