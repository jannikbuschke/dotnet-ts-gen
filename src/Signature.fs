[<AutoOpen>]
module TsGen.Signature

open System.Reflection
open System.Text.Json.Serialization
open TsGen
open Microsoft.FSharp.Reflection
open TypeCache

let getModuleName (t: System.Type) =
  if t.Namespace <> null then
    t.Namespace.Replace(".", "_")
  else
    "___"

let getName (t: System.Type) =
  let name = t.Name.Split("`").[0]

  if t.IsArray then
    name.Replace("[]", "") + "Array"
  else
    name

let getSignature (t: System.Type) =
  let modulName = getModuleName t
  let name = getName t
  // TODO: add generic arguments
  modulName, name

// <T>
let rec getGenericParameters (callingModule: string) (t: System.Type) =
  if t.IsGenericType then
    "<"
    + (t.GenericTypeArguments
       |> Seq.map (fun v ->
         let modulName = getModuleName v
         let name = getName v

         let name =
           if modulName = callingModule then
             name
           else
             modulName + "." + name

         $"{name}{getGenericParameters callingModule v}")
       |> String.concat ",")
    + ">"
  else
    ""

// (defaultT:T)
let rec getGenericParameterValues (callingModule: string) (t: System.Type) =
  if t.IsGenericType then
    "("
    + (t.GenericTypeArguments
       |> Seq.map (fun v ->
         let modulName = getModuleName v
         let name = "default" + (getName v)

         let name =
           if modulName = callingModule then
             name
           else
             modulName + "." + name

         $"{name}{getGenericParameterValues callingModule v}")
       |> String.concat ",")
    + ")"
  else
    ""

let getFullTypeName (t: System.Type) = t.FullName

let genericArgumentList (t: System.Type) =
  match t.GetGenericArguments() |> Seq.toList with
  | [] -> ""
  | arguments ->
    "<"
    + (arguments
       |> List.map (fun v -> v.Name)
       |> String.concat ",")
    + ">"

let genericArgumentListAsParameters (t: System.Type) =
  "("
  + (t.GetGenericArguments()
     |> Array.map (fun v -> "default" + v.Name + ":" + v.Name)
     |> String.concat ",")
  + ")"

let genericArgumentListAsParametersCall (t: System.Type) =
  "("
  + (t.GetGenericArguments()
     |> Array.map (fun v -> "default" + v.Name)
     |> String.concat ",")
  + ")"

let rec getPropertySignature (callingModule: string) (t: System.Type) =
  let kind = getKind t

  match kind with
  | TypeKind.Array -> getPropertySignature callingModule (typedefof<System.Collections.Generic.IEnumerable<_>>.MakeGenericType (t.GetElementType()))
  | _ ->
    let moduleName, name = getSignature t

    let name =
      if moduleName = callingModule then
        name
      else
        moduleName + "." + name

    if t.IsGenericType then
      name + (getGenericParameters callingModule t)
    else
      let modulName = getModuleName t
      let name = getName t

      if modulName = callingModule then
        name
      else
        modulName + "." + name


let getAnonymousFunctionSignatureForDefaultValue (t: System.Type) =
  let genericArguments = genericArgumentList t
  let parameters = genericArgumentListAsParameters t
  genericArguments + parameters

let getNamedFunctionSignatureForDefaultValue (t: System.Type) =
  let name = getName t
  let genericArguments = genericArgumentList t
  let signature = name + genericArguments
  signature
