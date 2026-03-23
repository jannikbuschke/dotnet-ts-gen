module TinyTypeGen.SignalR.Tests

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.SignalR

type MyDto =
  {
    X: string
    Y: int
  }

type IClientMethods =
  abstract member arst: MyDto -> Task<unit>
  abstract member message: string -> Task<unit>

type MyHub() =
  inherit Hub<IClientMethods>()
  member this.HelloWorld(x: MyDto, y: int) = this.Clients.All.arst x
  member this.Send(message: string) = this.Clients.All.message message

type Parameter =
  {
    Name: string
    Type: Type
    TypeName: string
  }

type Method =
  {
    Name: string
    Parameters: Parameter seq
    ReturnType: Type
    ReturnTypeName: string
  }

module Test =
  let getGenericArgumentIfHub (t: Type) =
    let rec findHubType (t: Type) =
      if t = null then
        None
      elif t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<Hub<_>> then
        Some(t.GetGenericArguments().[0])
      else
        findHubType t.BaseType

    findHubType t

  let extractMethods (t: Type) =
    t.GetMethods(
      Reflection.BindingFlags.Public
      ||| Reflection.BindingFlags.Instance
      ||| Reflection.BindingFlags.DeclaredOnly
    )
    |> Seq.map (fun m ->
      let returnTypeName = m.ReturnType.Name

      let parameters =
        m.GetParameters()
        |> Seq.mapi (fun i p ->
          {
            Name = (p.Name |> Option.ofObj |> Option.defaultValue (sprintf "p%d" i))
            Type = p.ParameterType
            TypeName = p.ParameterType.Name
          }
        )

      {
        Name = m.Name
        ReturnType = m.ReturnType
        ReturnTypeName = returnTypeName
        Parameters = parameters
      }
    )

  let renderParameters (parameters: Parameter seq) =
    parameters
    |> Seq.map (fun p -> sprintf "%s:%s" p.Name p.TypeName)
    |> String.concat ","

  let renderClientCallback m =
    let handlerParameters = m.Parameters |> renderParameters

    sprintf
      """
export type Handle_%s = (%s) => %s
function on_%s(connection:signalR.HubConnection, handler: Handle_%s){
  connection.on("%s",handler)
  return () => connection.off("%s",handler)
}
      """
      m.Name
      handlerParameters
      m.ReturnTypeName
      m.Name
      m.Name
      m.Name
      m.Name

  let renderClientCallbacks (t: Type) =
    let methods = extractMethods t
    let rendered = methods |> Seq.map renderClientCallback

    sprintf
      """
      // CLIENT
      %s
      """
      (rendered |> String.concat "\n")

  let renderServerMethod m =
    sprintf
      """    %s: (%s) => connection.invoke("%s",%s) as %s"""
      m.Name
      (m.Parameters |> renderParameters)
      m.Name
      (m.Parameters |> Seq.map (fun p -> p.Name) |> String.concat ",")
      m.ReturnTypeName

  let renderApi (t: Type) =
    let methods = extractMethods t
    let renderedMethods = methods |> Seq.map renderServerMethod |> String.concat "\n"

    sprintf
      """
//type IServerApi = {
//}
function createApi(connection: signalR.HubConnection) {
  return {
%s
  } // satisfies IServerApi
}
            """
      renderedMethods

  let generate (serverHub: Type) =
    let clientApi = getGenericArgumentIfHub serverHub

    match clientApi with
    | Some hub ->
      printfn "types %A" hub

      let clientApi = renderClientCallbacks hub
      let serverApi = renderApi serverHub
      // printfn "CLIENT:\n%s" clientApi
      printfn "SERVER:\n%s" serverApi

      // let serverMethods =
      //     t.GetMethods(
      //         Reflection.BindingFlags.Public
      //         ||| Reflection.BindingFlags.Instance
      //         ||| Reflection.BindingFlags.DeclaredOnly
      //     )

      // let params =
      //     serverMethods
      //     |> Seq.collect (fun x -> x.GetParameters())
      //     |> Seq.map (fun x -> x.ParameterType)

      // printfn "params %A" params
      // printfn "Server methods\n%A" serverMethods

      // let clientMethods =
      //     hub.GetMethods(
      //         Reflection.BindingFlags.Public
      //         ||| Reflection.BindingFlags.Instance
      //         ||| Reflection.BindingFlags.DeclaredOnly
      //     )
      //
      // printfn "Client methods\n%A" clientMethods

      Ok()
    | None -> Error "Not a hub type"
