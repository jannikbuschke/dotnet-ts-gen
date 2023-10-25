[<AutoOpen>]
module TinyTypeGen.Signature

open System
open System.Text.Json.Serialization
open TypeCache
open System.Linq

// TODO: refactor and fix null warnings

/// Checks if a type is declared in a module
let isTypeDeclaredInModule (t: Type) =
  match t.DeclaringType with
  | null -> false
  | dt when dt.IsClass && dt.IsAbstract && dt.IsSealed ->
    // F# modules are static classes (abstract and sealed)
    true
  | _ -> false

let rec getModuleName (t: Type) =

  if t.Namespace <> null then

    let ns =
      if
        t.FullName <> null
        && not t.IsGenericType
        && not t.IsGenericParameter
        && t.FullName.Contains "+"
        && isTypeDeclaredInModule t
      then
        // static nested class
        let parts = t.FullName.Split("+")
        (parts.Take(parts.Length - 1) |> String.concat ".")
      // static nested class
      else if t.FullName <> null && t.FullName.Contains "+" && isTypeDeclaredInModule t then
        let parts = t.FullName.Split "+"
        // let result = sprintf "%s.%s" parts[0] parts[1]
        // result
        parts[0]
      else if t.DeclaringType <> null then
        getModuleName t.DeclaringType
      else
        t.Namespace

    let result = ns.Replace(".", "_")
    let result = result.Split("`").[0]

    if result.Contains "`" then
      failwith $"Modulename for '{t.Name}' resulted in '{result}' containing a `, which is not supported. This is a bug"

    result

  else
    "___"

let isAnonymousRecord (t: Type) = t.Name.StartsWith "<>f__AnonymousType"

let getDefinition (t: Type) =
  if t.IsGenericType && not t.IsGenericTypeDefinition then
    t.GetGenericTypeDefinition()
  else
    t

let getName (t: Type) (predefinedTypes: PreDefinedTypes) =
  let tg = getDefinition t

  let predefined =
    if predefinedTypes.ContainsKey tg then
      Some(predefinedTypes.Item tg)
    else
      None

  let getDefaultName =
    let name = t.Name.Split("`").[0]
    let name = name.Split("`").[0]

    if isAnonymousRecord t then
      name.Replace("<>f__", "f__")
    else
      name

  predefined |> Option.bind _.Name |> Option.defaultValue getDefaultName

/// ModulName * Name
let getSignature (t: Type) (predefinedTypes: PreDefinedTypes) =
  let modulName = getModuleName t

  let name = getName t predefinedTypes
  // TODO: add generic arguments
  modulName, name

// <T>
let rec getGenericParameters (callingModule: string) (t: Type) (getName: GetName) getPropertySignature =
  if t.IsArray then
    let elType = t.GetElementType()
    let modulName = getModuleName elType
    let name = getName elType
    let args = getGenericParameters callingModule elType getName getPropertySignature
    sprintf "<%s.%s%s>" modulName name args
  else if t.IsGenericType then
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

         if v.IsArray then
           $"Array{getGenericParameters callingModule v getName getPropertySignature}"
         else
           $"{name}{getGenericParameters callingModule v getName getPropertySignature}"
       )
       |> String.concat ",")
    + ">"
  else
    ""

let getFullTypeName (t: Type) = t.FullName

let isAnonymousRecordProperty (name: string) =
  name.StartsWith("<") && not (name.EndsWith(">"))

let stripAnonymousGenericName (name: string) =
  name.Substring(1, name.IndexOf(">") - 1)

let genericArgumentNames (t: System.Type) =
  t.GetGenericArguments()
  |> Seq.map (fun v ->
    // workaround for f# anonymous records, syntax is not yet well understood
    if isAnonymousRecordProperty v.Name then
      stripAnonymousGenericName v.Name
    else
      v.Name
  )
  |> Seq.toList

let genericArgumentList (t: Type) =
  match (genericArgumentNames t) with
  | [] -> ""
  | arguments -> sprintf "<%s>" (arguments |> String.concat ",")

let genericArgumentListAsParameters (t: Type) =
  "("
  + (t.GetGenericArguments()
     |> Array.map (fun v -> "default" + v.Name + ":" + v.Name)
     |> String.concat ",")
  + ")"

let rec getPropertySignature (callingModule: string | null) (t: Type) (getSignature: GetSignature) (getName: GetName) =
  let kind = getKind t

  let result =
    match kind with
    | TypeKind.Array ->
      getPropertySignature
        callingModule
        (typedefof<Collections.Generic.IEnumerable<_>>.MakeGenericType(t.GetElementType()))
        getSignature
        getName
    | _ ->
      let moduleName, name = getSignature t

      let name =
        if moduleName = callingModule || t.IsGenericParameter then
          name
        else
          moduleName + "." + name

      if t.IsGenericType then
        if kind = TypeKind.Union then
          let recordArgs =
            if (not (t.IsGenericTypeDefinition)) then
              let genericDefinition = t.GetGenericTypeDefinition()
              let gTypeArgs = genericDefinition.GetGenericArguments()
              gTypeArgs
              |> Array.mapi (fun i x ->
                let argi = t.GenericTypeArguments[i]
                let isRecordTrue = isRecord argi
                if isRecord argi then Some x.Name else None
              )
              |> Array.choose id
            else
              [||]
          let variant =
            if recordArgs.Length > 0 then
              ("_" + (String.concat "" recordArgs))
            else
              ""
          name
          + variant
          + (getGenericParameters
            callingModule
            t
            getName
            (fun t -> getPropertySignature callingModule t getSignature getName))
        else
          name
          + (getGenericParameters
            callingModule
            t
            getName
            (fun t -> getPropertySignature callingModule t getSignature getName))
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

let getPropertySignature2 (callingModule: string | null) (env: Env) (t: Type) =
  getPropertySignature callingModule t env.GetSignature env.GetName


let rec getDuPropertySignature (callingModule: string) (t: Type) (getName: GetName) (getSignatur: GetSignature) =
  let kind = getKind t

  match kind with
  | TypeKind.Array ->
    getDuPropertySignature
      callingModule
      (typedefof<Collections.Generic.IEnumerable<_>>.MakeGenericType(t.GetElementType()))
      getName
      getSignatur
  | _ ->
    let moduleName, name = getSignatur t

    let name =
      if moduleName = callingModule then
        name
      else
        moduleName + "." + name

    if t.IsGenericType then
      if t.IsGenericTypeDefinition then
        name + (genericArgumentList t)
      else
        name
        + (getGenericParameters callingModule t getName (getPropertySignature callingModule))
    else
      let modulName = getModuleName t
      let name = getName t

      if modulName = callingModule then
        name
      else
        modulName + "." + name
