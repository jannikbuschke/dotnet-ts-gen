module TsGen.Collect

open System.Collections.Generic
open System.Reflection
open System.Text.Json.Serialization.TypeCache
open Microsoft.FSharp.Reflection

type Collector(defaultTypes:PredefinedTypes.PreDefinedTypes)=
  let ignoreList = [typedefof<FSharpFunc<_, _>>]
  let allTypes = System.Collections.Generic.HashSet<System.Type>()

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
            [ v ])
        |> Seq.toList

      [ genericDefinition ] @ args

  let getTypeOrGenericDefinitionAndArgumentsAsDependencies (t: System.Type) =
    if not t.IsGenericType
                    || (t.IsGenericType
                        && t.IsGenericTypeDefinition) then
                   [ t ]
                 else
                   getGenericDefinitionAndArgumentsAsDependencies t
  //maybe this should be recursive
  let _getDependencies (t: System.Type) =

    // Microsoft_FSharp_Core.FSharpFunc
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
             getTypeOrGenericDefinitionAndArgumentsAsDependencies f.PropertyType
           )
           |> Seq.toList)
        | TypeKind.Union ->
          let x =
            (FSharpType.GetUnionCases t)
            |> Seq.collect (fun c ->
              c.GetFields()
              |> Array.toList
              |> List.collect (fun f ->
                printfn " collect for type %s" f.PropertyType.FullName
                getTypeOrGenericDefinitionAndArgumentsAsDependencies f.PropertyType
                )
            )
            |> Seq.toList

          x
        | TypeKind.Array ->
          [ t.GetElementType()
            typedefof<IEnumerable<_>> ]
        | _ -> []

      let genericArgs =
        if not t.IsGenericType
           || (t.IsGenericType && t.IsGenericTypeDefinition) then
          []
        else
          getGenericDefinitionAndArgumentsAsDependencies t

      result @ genericArgs

  member this.getDependencies (t: System.Type) = _getDependencies t |> List.distinct

  member this.getFilteredDeps (moduleName: string) (t: System.Type) =
    this.getDependencies t
    |> List.filter (fun v ->
      let name = getModuleName v
      name = moduleName)

  member this.collectDependencies (t: System.Type) =
    let rec collectDependencies (depth: int) (t: System.Type) =
      if depth > 100 then
        failwith (sprintf "too deep (current type = %s)" t.FullName)

      if allTypes.Contains t then
        ()
      else
        allTypes.Add t |> ignore

        t
        |> this.getDependencies
        |> List.iter (collectDependencies (depth + 1))

        ()

    collectDependencies 0 t

  member this.collectModules (types: System.Type list) =
    (typedefof<obj> :: typedefof<System.Byte> :: types)
    |> List.iter (this.collectDependencies)

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
        TopologicalSort.topologicalSort (this.getFilteredDeps moduleName) (items |> List.distinct)

      let sorted2 = sorted |> List.distinct

      { Name = moduleName
        Types = (items |> List.distinct) })

  member this.getRawDeps (n: TsModule) =
    let d0 =
      n.Types
      |> List.filter (fun t ->
        (not t.IsGenericType)
        || (t.IsGenericType && t.IsGenericTypeDefinition))
      |> List.collect this.getDependencies
    let deps =
      n.Types
      |> List.filter (fun t ->
        (not t.IsGenericType)
        || (t.IsGenericType && t.IsGenericTypeDefinition))
      |> List.collect this.getDependencies
    deps

  member this.getModuleDependencies (n: TsModule) =
    // let d0 =
    //   n.Types
    //   |> List.filter (fun t ->
    //     (not t.IsGenericType)
    //     || (t.IsGenericType && t.IsGenericTypeDefinition))
    //   |> List.collect getDependencies
    let deps = this.getRawDeps n |> List.map getModuleName

    (deps
     |> List.distinct
     |> List.filter (fun v -> v <> n.Name))

  member this.GetModulesWithDependencies(types: System.Type list) =
    let modules = this.collectModules types
    modules |> List.map(fun m->
     let deps = this.getRawDeps m
     { TsModuleWithDeps.Dependencies = deps; Name = m.Name;Types = m.Types } )

let init (defaultTypes: PredefinedTypes.PreDefinedTypes) =

  let collector = new Collector(defaultTypes)

  {| GetRawDeps = collector.getRawDeps
     GetModuleDependencies = collector.getModuleDependencies
     getDependencies = collector.getDependencies
     collectModules = collector.collectModules |}

let getDependencies a = (init PredefinedTypes.defaultTypes).getDependencies a