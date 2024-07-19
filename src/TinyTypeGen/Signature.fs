[<AutoOpen>]
module TsGen.Signature

open System
open System.Reflection
open System.Text.Json.Serialization
open TsGen
open Microsoft.FSharp.Reflection
open TypeCache
open System.Linq

/// Checks if a type is declared in a module
let isTypeDeclaredInModule (t: Type) =
  match t.DeclaringType with
  | null -> false
  | dt when dt.IsClass && dt.IsAbstract && dt.IsSealed ->
    // F# modules are static classes (abstract and sealed)
    true
  | _ -> false

let getModuleName (t: System.Type) =
  let name = t.Name
  let fullname = t.FullName
  let ns = t.Namespace

  if t.Namespace <> null then
// <<<<<<< Updated upstream
    let ns =
      if t.FullName <> null
         && not t.IsGenericType
         && not t.IsGenericParameter
         && t.FullName.Contains "+"
         && isTypeDeclaredInModule t then
        // static nested class
        let name = t.Name
        let fullname = t.FullName
        let declaringType = t.DeclaringType
        let parts = t.FullName.Split("+")
        (parts.Take(parts.Length - 1) |> String.concat ".")
      else if t.FullName <> null
         && t.FullName.Contains "+"
         && isTypeDeclaredInModule t then
        // static nested class
        let name = t.Name
        let fullname = t.FullName
        let declaringType = t.DeclaringType
        let parts = t.FullName.Split("+")
        (parts.Take(parts.Length - 1) |> String.concat ".")
      else
        t.Namespace

      // let x = []
      // x |> List.fold

    let result = ns.Replace(".", "_")

    result
// =======
//     // if t.FullName = null
//     // then t.Namespace.Replace(".", "_")
//     // else
//     if t.FullName.Contains "+" then
//       // static nested class
//       let parts = t.FullName.Split("+")
//       parts.Take(parts.Length - 1) |> String.concat "_"
//     else
//       t.Namespace.Replace(".", "_")
// >>>>>>> Stashed changes
  else
    "___"

let isAnonymousRecord (t: System.Type) = t.Name.StartsWith "<>f__AnonymousType"

let getName (t: System.Type) =
  let name = t.Name.Split("`").[0]
  // let name =
  //   if t.FullName <> null
  //      && not t.IsGenericType
  //      && not t.IsGenericParameter
  //      && t.FullName.Contains "+"
  //      && isTypeDeclaredInModule t then
  //     // include declaring type in name
  //     let name = t.DeclaringType.Name + "_" + t.Name
  //     name
  //   // static nested class
  //   // let name = t.Name
  //   // let fullname = t.FullName
  //   // let declaringType = t.DeclaringType
  //   // let parts = t.FullName.Split("+")
  //   // (parts.Take(parts.Length - 1) |> String.concat ".")
  //   else
  //     t.Name //.Split("`").[0]

  let name = name.Split("`").[0]

  if t.IsArray then
    name.Replace("[]", "") + "Array"
  elif isAnonymousRecord t then
    name.Replace("<>f__", "f__")
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
    // let genericArgs = t.GetGenericArguments()
    "<"
    + (t.GetGenericArguments()
       |> Seq.map (fun v ->
         let modulName = getModuleName v
         let name = getName v

         let name =
           if modulName = callingModule || v.IsGenericParameter then
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

let isAnonymousRecordProperty (name: string) =
  name.StartsWith("<") && not (name.EndsWith(">"))

let stripAnonymousGenericName (name: string) =
  name.Substring(1, name.IndexOf(">") - 1)

let genericArgumentList (t: System.Type) =
  let result =
    let args = t.GetGenericArguments() |> Seq.toList

    match args with
    | [] -> ""
    | arguments ->
      "<"
      + (arguments
         |> List.map (fun v ->
           let name = v.Name
           // workaround for f# anonymous records, syntax is not yet well understood
           if isAnonymousRecordProperty name then
             stripAnonymousGenericName name
           else
             name)
         |> String.concat ",")
      + ">"

  result

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

let rec getDuPropertySignature (callingModule: string) (t: System.Type) =
  let kind = getKind t

  match kind with
  | TypeKind.Array ->
    getDuPropertySignature
      callingModule
      (typedefof<System.Collections.Generic.IEnumerable<_>>.MakeGenericType (t.GetElementType()))
  | _ ->
    let moduleName, name = getSignature t

    let name =
      if moduleName = callingModule then
        name
      else
        moduleName + "." + name

    if t.IsGenericType then
      if t.IsGenericTypeDefinition then
        name + (genericArgumentList t)
      else
        name + (getGenericParameters callingModule t)
    else
      let modulName = getModuleName t
      let name = getName t

      if modulName = callingModule then
        name
      else
        modulName + "." + name

let rec getPropertySignature (callingModule: string) (t: System.Type) =
  let kind = getKind t

  // if t.IsGenericTypeParameter then
  //   t.Name
  // else
  let result =
    match kind with
    | TypeKind.Array ->
      getPropertySignature
        callingModule
        (typedefof<System.Collections.Generic.IEnumerable<_>>.MakeGenericType (t.GetElementType()))
    | _ ->
      let moduleName, name = getSignature t

      let name =
        if moduleName = callingModule || t.IsGenericParameter then
          name
        else
          moduleName + "." + name

      if t.IsGenericType then
        name + (getGenericParameters callingModule t)
      else
        // weird
        let modulName = getModuleName t
        let name = getName t

        if modulName = callingModule || t.IsGenericParameter then
          name
        else
          modulName + "." + name

  if isAnonymousRecordProperty result then
    stripAnonymousGenericName result
  else
    result

let getAnonymousFunctionSignatureForDefaultValue (t: System.Type) =
  let genericArguments = genericArgumentList t
  let parameters = genericArgumentListAsParameters t
  genericArguments + parameters

let getNamedFunctionSignatureForDefaultValue (t: System.Type) =
  let name = getName t
  let genericArguments = genericArgumentList t
  let signature = name + genericArguments
  signature