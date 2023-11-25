module TsGen.Collect

open System.Reflection
open System.Text.Json.Serialization.TypeCache
open Microsoft.FSharp.Reflection

let init (defaultTypes: PredefinedTypes.PreDefinedTypes) =

  let rec getGenericDefinitionAndArgumentsAsDependencies (t: System.Type) =
    if t.IsGenericTypeDefinition then
      failwith "t is a generic type definition but should not be"

    if not t.IsGenericType then
      failwith "t is not a generic type but should be"

    let genericDefinition = t.GetGenericTypeDefinition()

    let args =
      t.GenericTypeArguments
      |> Seq.collect (fun v ->
        if v.IsGenericType && not v.IsGenericTypeDefinition then
          getGenericDefinitionAndArgumentsAsDependencies v
        else
          [ v ])
      |> Seq.toList

    [ genericDefinition ] @ args

  let _getDependencies (t: System.Type) =
    match defaultTypes.TryGetValue t with
    | true, value -> value.Dependencies
    | _ ->
      let kind = getKind t

      let result =
        match kind with
        | TypeKind.Other
        | TypeKind.Record ->
          (t.GetProperties(BindingFlags.Public ||| BindingFlags.Instance)
           |> Seq.collect (fun f ->
             if not f.PropertyType.IsGenericType
                || (f.PropertyType.IsGenericType
                    && f.PropertyType.IsGenericTypeDefinition) then
               [ f.PropertyType ]
             else
               getGenericDefinitionAndArgumentsAsDependencies f.PropertyType)
           |> Seq.toList)


        | TypeKind.Union ->
          let x =
            (FSharpType.GetUnionCases t)
            |> Seq.collect (fun c ->
              c.GetFields()
              |> Array.map (fun f -> f.PropertyType)
              |> Array.toList)
            |> Seq.toList

          x
        | TypeKind.Array -> [ t.GetElementType() ]
        | _ -> []

      let genericArgs =
        if not t.IsGenericType
           || (t.IsGenericType && t.IsGenericTypeDefinition) then
          []
        else
          getGenericDefinitionAndArgumentsAsDependencies t

      result @ genericArgs

  let getDependencies (t: System.Type) = _getDependencies t |> List.distinct

  let getFilteredDeps (moduleName: string) (t: System.Type) =
    getDependencies t
    |> List.filter (fun v ->
      let name = getModuleName v
      name = moduleName)

  let allTypes = System.Collections.Generic.HashSet<System.Type>()

  let rec collectDependenciesTransitively (depth: int) (t: System.Type) =
    if depth > 20 then failwith "too deep"

    if allTypes.Contains t then
      ()
    else
      allTypes.Add t |> ignore

      t
      |> getDependencies
      |> List.iter (collectDependenciesTransitively (depth + 1))

      ()

  let collectModules (types: System.Type list) =
    (typedefof<obj> :: typedefof<System.Byte> :: types)
    |> List.iter (collectDependenciesTransitively 0)

    allTypes
    |> Seq.toList
    |> List.groupBy getModuleName
    |> List.map (fun (moduleName, items) ->
      let items =
        items
        |> List.map (fun v ->
          if v.IsGenericType && not v.IsGenericTypeDefinition then
            v.GetGenericTypeDefinition()
          else
            v)

      let sorted, _ =
        TopologicalSort.topologicalSort (getFilteredDeps moduleName) (items |> List.distinct)

      let sorted2 = sorted |> List.distinct
      { Name = moduleName; Types = sorted2 })

  let getModuleDependencies (n: TsModule) =
    let deps =
      n.Types
      |> List.filter (fun t ->
        (not t.IsGenericType)
        || (t.IsGenericType && t.IsGenericTypeDefinition))
      |> List.collect getDependencies
      |> List.map getModuleName

    (deps
     |> List.distinct
     |> List.filter (fun v -> v <> n.Name))

  {| GetModuleDependencies = getModuleDependencies
     getDependencies = getDependencies
     collectModules = collectModules |}
