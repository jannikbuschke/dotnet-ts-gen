module TinyTypeGen.SignalR

open System
open System.Threading.Tasks
open TinyTypGen.Run

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

let getGenericArgumentIfHub (t: Type) =
  let rec findHubType (t: Type) =
    if t = null then
      None
    elif
      t.IsGenericType
      && t.GetGenericTypeDefinition() = typedefof<Microsoft.AspNetCore.SignalR.Hub<_>>
    then
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
      (m.GetParameters()
       |> Seq.mapi (fun i p ->
         {
           Name = (p.Name |> Option.ofObj |> Option.defaultValue (sprintf "p%d" i))
           Type = p.ParameterType
           TypeName = p.ParameterType.Name
         }
       ))

    {
      Name = m.Name
      ReturnType = m.ReturnType
      ReturnTypeName = returnTypeName
      Parameters = parameters
    }
  )

let renderParameters getSignature (parameters: Parameter seq) =
  (parameters
   |> Seq.map (fun p ->
     let ns = getSignature p.Type
     sprintf "%s: %s" p.Name ns
   )
   |> String.concat ", ")

let renderClientCallback (env: Env) m =
  let getPropSig = env.GetPropertySignature null
  let handlerParameters = m.Parameters |> renderParameters getPropSig

  sprintf
    """
export type %s_Handler = (%s) => void

const on%s = (connection: signalR.HubConnection) => (handler: %s_Handler) => {
  connection.on("%s", handler)
  return () => connection.off("%s", handler)
}
"""
    m.Name
    handlerParameters
    m.Name
    m.Name
    m.Name
    m.Name

let renderClientCallbacks (t: Type) (env: Env) =
  let methods = extractMethods t
  let rendered = methods |> Seq.map (renderClientCallback env)

  sprintf
    """
  %s
"""
    (rendered |> String.concat "\n")

let renderServerMethod (env: Env) m =
  let returnType =
    if
      m.ReturnType.IsGenericType
      && m.ReturnType.GetGenericTypeDefinition() = typedefof<Task>
      || m.ReturnType = typeof<Task>
    then
      "void"
    else if
      m.ReturnType.IsGenericType
      && m.ReturnType.GetGenericTypeDefinition() = typedefof<Task<_>>
    then
      m.ReturnType.GetGenericArguments().[0] |> env.GetPropertySignature null
    else
      m.ReturnType |> env.GetPropertySignature ""
  sprintf
    """    %s: (%s) => connection.invoke<%s>("%s",%s),"""
    m.Name
    (m.Parameters |> renderParameters (env.GetPropertySignature null))
    returnType
    m.Name
    (m.Parameters |> Seq.map _.Name |> String.concat ",")

let renderApi typeName (t: Type) (url: string) =
  let methods = extractMethods t

  let renderedMethods =
    methods |> Seq.map (renderServerMethod typeName) |> String.concat "\n"

  sprintf
    """
export function createApi(connection: signalR.HubConnection) {
  return {
%s
  }
}
"""
    renderedMethods

let renderConfigure (name: string) (hubType: Type) (url: string) =
  let methods = extractMethods hubType
  let callbacks =
    methods
    |> Seq.map (fun x -> sprintf "    on%s: on%s(connection)" x.Name x.Name)
    |> String.concat ",\n"
  sprintf
    """
export const configure%s = (
  customize?: (v: signalR.HubConnectionBuilder) => signalR.HubConnectionBuilder,
) => {
  const builder = new signalR.HubConnectionBuilder().withUrl("%s")
  const connection = (customize ? customize(builder) : builder).build()

  return {
    connection: connection,
    api: createApi(connection),
%s
  }
}
"""
    name
    url
    callbacks

let withLogging f a =
  let result = f a
  result

let createHub (env: Env) getDeps (t: Type, path: string, url: string) =
  let clientApi = getGenericArgumentIfHub t
  match clientApi with
  | Some hub ->
    let clientApi = renderClientCallbacks hub
    let serverApi = renderApi env t url
    let configure = renderConfigure t.Name hub url
    let content = sprintf "%s\n%s\n%s" (clientApi env) serverApi configure
    let deps =
      fun x ->
        let ioTypes =
          x
          |> extractMethods
          |> Seq.collect (fun m -> m.ReturnType :: (m.Parameters |> Seq.map _.Type |> Seq.toList))
          |> Seq.toList
        let deps = ioTypes |> Seq.collect getDeps |> Seq.toList
        (ioTypes @ deps) |> List.distinct
    let deps = [ hub; t ] |> List.collect (withLogging deps) |> List.distinct // (deps hub) @ (deps t)
    let result: Hub =
      {
        Content = content
        Deps = deps
        Path = path
      }

    Ok result
  | None -> Error "Not a hub type"
