module TinyTypeGen.Collect2

open System.Collections.Generic
open System.Reflection
open System.Text.Json.Serialization.TypeCache
open Microsoft.FSharp.Reflection
open Config

let ignoreList = [ typedefof<FSharpFunc<_, _>> ]

let rec getGenericDefinitionAndArgumentsAsDependencies (t: System.Type) =
  if t.IsGenericTypeDefinition then
    failwith "t is a generic type definition but should not be"

  if not t.IsGenericType then
    failwith "t is not a generic type but should be"

  let genericDefinition = t.GetGenericTypeDefinition()

  if ignoreList |> List.contains genericDefinition then
    []
  else

    let args =
      t.GenericTypeArguments
      |> Seq.collect (fun v ->
        if v.IsGenericType && not v.IsGenericTypeDefinition then
          getGenericDefinitionAndArgumentsAsDependencies v
        else
          [ v ]
      )
      |> Seq.toList

    [ genericDefinition ] @ args

let getTypeOrGenericDefinitionAndArgumentsAsDependencies (t: System.Type) =
  if t.IsArray then
    [ t.GetElementType(); typedefof<IEnumerable<_>> ]
  else if not t.IsGenericType || (t.IsGenericType && t.IsGenericTypeDefinition) then
    [ t ]
  else
    getGenericDefinitionAndArgumentsAsDependencies t

//probably this should be recursive
let _getDependencies (defaultTypes: PreDefinedTypes) (t: System.Type) =

  match defaultTypes.TryGetValue t with
  | true, value -> value.Dependencies
  | _ ->
    let kind = getKind t

    let result =
      match kind with
      | TypeKind.Other
      | TypeKind.Record ->
        (t.GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
         |> Seq.collect (fun f -> getTypeOrGenericDefinitionAndArgumentsAsDependencies f.PropertyType)
         |> Seq.toList)
      | TypeKind.Union ->
        t
        |> FSharpType.GetUnionCases
        |> Seq.collect (fun c ->
          c.GetFields()
          |> Array.toList
          |> List.collect (fun f -> getTypeOrGenericDefinitionAndArgumentsAsDependencies f.PropertyType)
        )
        |> Seq.toList
      | TypeKind.Array -> [ t.GetElementType(); typedefof<IEnumerable<_>> ]
      | _ -> []

    let genericArgs =
      if not t.IsGenericType || (t.IsGenericType && t.IsGenericTypeDefinition) then
        []
      else
        getGenericDefinitionAndArgumentsAsDependencies t

    result @ genericArgs

let Collector (config: Configuration) =
  let defaultTypes: PreDefinedTypes = config.PredefinedTypes
  let allTypes = HashSet<System.Type>()
  let maxDepth = config.MaxRecursionDepth

  let filterIgnored (t: System.Type) =
    config.IgnoredNamespaces |> List.contains (t |> getModuleName) |> not
    || PredefinedTypes.isDefined defaultTypes t

  // this is used
  let getDependencies (t: System.Type) =
    _getDependencies defaultTypes t |> List.distinct |> List.filter filterIgnored

  let collectDependencies (t: System.Type) =
    let rec collectDependencies (depth: int) (t: System.Type) =
      if depth > maxDepth then
        failwith (sprintf "too deep (current type = %s)" t.FullName)

      if allTypes.Contains t then
        ()
      else
        allTypes.Add t |> ignore

        t |> getDependencies |> List.iter (collectDependencies (depth + 1))

        ()

    collectDependencies 0 t

  let collectModules (types: System.Type list) =
    (typedefof<obj> :: typedefof<System.Byte> :: types)
    |> List.iter collectDependencies

    allTypes
    |> Seq.filter (fun x -> x.Name.Contains("Exception") |> not)
    |> Seq.choose (fun x ->
      if x.IsGenericType && not x.IsGenericTypeDefinition then
        Some(x.GetGenericTypeDefinition())
      else if x.IsGenericParameter then
        None
      else
        Some x
    )
    |> Seq.groupBy getModuleName
    |> Seq.map (fun (moduleName, items) ->
      {
        Name = moduleName
        OriginalNamespacename = items |> Seq.tryHead |> Option.map _.Namespace |> Option.defaultValue null
        Types = (items |> Seq.distinct |> Seq.toList)
      }
    )
    |> Seq.toList

  let getRawDeps (n: TsModule) =
    n.Types
    |> List.filter (fun t ->
      (t.Name.Contains("Exception") |> not)
      && ((not t.IsGenericType) || (t.IsGenericType && t.IsGenericTypeDefinition))
    )
    |> List.collect getDependencies
    |> List.filter (fun x -> x.Name.Contains("Exception") |> not)

  fun (types: System.Type list) ->
    types
    |> collectModules
    |> List.map (fun m ->
      getRawDeps m
      |> List.filter (fun d ->
        let moduleName = getModuleName d
        moduleName <> m.Name
      )
      |> fun deps ->
          {
            TsModuleWithDeps.Dependencies = deps
            OriginalNamespacename = m.OriginalNamespacename
            Name = m.Name
            Types = m.Types
          }
    )
    |> List.filter (fun x -> not (config.IgnoredNamespaces |> List.contains x.Name))
    |> List.filter (fun x -> x.Types <> [])
