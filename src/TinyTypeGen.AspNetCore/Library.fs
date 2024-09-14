module TinyTypeGen.AspNetCore

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc.ApiExplorer
open Microsoft.AspNetCore.Mvc.Controllers
open Microsoft.AspNetCore.Mvc.Infrastructure
open Microsoft.Extensions.DependencyInjection
open System.Linq

let verb method =
  match method with
  | "POST"
  | "post" -> TsGen.HttpVerb.POST
  | "GET"
  | "get" -> TsGen.HttpVerb.GET
  | "PATCH"
  | "patch" -> TsGen.HttpVerb.PATCH
  | "DELETE"
  | "delete" -> TsGen.HttpVerb.DELETE
  | "PUT"
  | "put" -> TsGen.HttpVerb.PUT
  | _ -> TsGen.HttpVerb.GET

let getEndpoints (services: IServiceProvider) =
  let adcp = services.GetRequiredService<IActionDescriptorCollectionProvider>()
  let descriptors = adcp.ActionDescriptors.Items.OfType<ControllerActionDescriptor>()

  let wrapperTypes =
    [ typeof<Task<_>>.GetGenericTypeDefinition ()
      typeof<ValueTask<_>>.GetGenericTypeDefinition ()
      typeof<Microsoft.AspNetCore.Mvc.ActionResult<_>>.GetGenericTypeDefinition () ]

  let rec toResultType (input: Type) =
    if (input.IsGenericType
        && not input.IsGenericTypeDefinition
        && (wrapperTypes.Contains(input.GetGenericTypeDefinition()))) then
      let args = input.GenericTypeArguments
      toResultType (args[0])
    else
      input

  let getInputType (nameSpace: string) (name: string) (parameters: Reflection.ParameterInfo list) =

    match parameters with
    | [] -> typeof<Unit>
    | [ p ] ->
      if p.ParameterType.IsPrimitive then
        let builder = TinyTypeGen.DynamicType.createBuilder (sprintf "%s%s" nameSpace name)

        builder
        |> TinyTypeGen.DynamicType.createProperty (p.Name, p.ParameterType)

        TinyTypeGen.DynamicType.build builder
      else
        p.ParameterType
    | parameters ->
      let builder = TinyTypeGen.DynamicType.createBuilder (sprintf "%s%s" nameSpace name)

      parameters
      |> List.iter (fun x ->
        TinyTypeGen.DynamicType.createProperty (x.Name, x.ParameterType) builder
        |> ignore)

      TinyTypeGen.DynamicType.build builder

  let apiEndpoints =
    descriptors
    |> Seq.filter (fun x -> x.AttributeRouteInfo <> null)
    |> Seq.map (fun x ->
      let methods =
        x.EndpointMetadata.OfType<Microsoft.AspNetCore.Routing.HttpMethodMetadata>()
        |> Seq.collect (fun v -> v.HttpMethods)

      let nameSpaceForDynamics =
        x.ControllerTypeInfo.Namespace
        |> Option.ofObj
        |> Option.map (fun x -> sprintf "%s." x)
        |> Option.defaultValue ""

      let method = methods.FirstOrDefault()
      let verb = verb method
      let parameters = x.MethodInfo.GetParameters() |> Seq.toList
      let name = sprintf "%s_Request" x.ActionName

      let inputType =
        parameters
        |> getInputType nameSpaceForDynamics name

      // let inputType = x.MethodInfo.GetParameters().FirstOrDefault()
      // |> Option.ofObj
      // |> Option.map _.ParameterType
      // |> Option.defaultValue (typeof<Unit>)
      { TsGen.ApiEndpoint.Request = inputType
        TsGen.ApiEndpoint.Response = toResultType (x.MethodInfo.ReturnType)
        TsGen.ApiEndpoint.Method = verb
        TsGen.ApiEndpoint.Route = "/" + x.AttributeRouteInfo.Template })
    |> Seq.distinctBy (fun x -> (x.Route, x.Method))
    |> Seq.toList

  apiEndpoints
