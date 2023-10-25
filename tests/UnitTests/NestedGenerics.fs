module Test.Generics2

open Expecto
open Test

type GenericRecord<'t> = { GenericProperty: 't }

type Localizable<'a> =
  {
    Value: 'a
    Localizations: Map<string, 'a>
  }

type Localizable2<'a> =
  {
    Value: 'a
    Localizations: Map<string, 'a>
  }

// recursive generic

type TreeNodeBase<'t> =
  {
    NodeId: string
    Name: string
    Value: 't
    Children: TreeNodeBase<'t> list
    ParentId: string option
    PruneCondition: bool option
  }

let tests =
  testList
    "NestedGenerics"
    [
      testCase
        "not render typedef Localizable<string>"
        (fun () ->
          // runtime type, should not be rendered (only generic typedefinition)
          typeof<Localizable<string>> |> definition |> Expect.similar ""
        )
      testCase
        "render typedefof Localizable<string>"
        (fun () ->
          typedefof<Localizable<string>>
          |> definition
          |> Expect.similar
            """
export type Localizable<a> = {
  value: a
  localizations: Microsoft_FSharp_Collections.FSharpMap<System.String,a>
}
"""
        )
      testCase
        "render recursive generic"
        (fun () ->
          typedefof<TreeNodeBase<string>>
          |> definition
          |> Expect.equal
            """
export type TreeNodeBase<t> = {
 nodeId: System.String
 name: System.String
 value: t
 children: Microsoft_FSharp_Collections.FSharpList<TreeNodeBase<t>>
 parentId: Microsoft_FSharp_Core.FSharpOption<System.String>
 pruneCondition: Microsoft_FSharp_Core.FSharpOption<System.Boolean>
}
"""
        )
    ]
