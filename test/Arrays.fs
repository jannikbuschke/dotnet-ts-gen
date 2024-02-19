module Test.Arrays

open Xunit

type A = { Title: string }
type MyRecord = { Field1: string []; Field2: A [] }

[<Fact>]
let deps ()=
  let init = TsGen.Collect.init TsGen.PredefinedTypes.defaultTypes
  let modules = init.collectModules [typeof<MyRecord>]
  let modules = modules |> List.map(fun m -> m, init.GetModuleDependencies m)
  let dependencies = init.getDependencies typedefof<MyRecord array>
  ()

let rendered, value = renderTypeAndValue typedefof<MyRecord>

[<Fact>]
let ``Render MyRecord with arrays`` () =

  Expect.similar
    rendered
    """
export type MyRecord = {
 field1: System_Collections_Generic.IEnumerable<System.String>
 field2: System_Collections_Generic.IEnumerable<A>
}
"""

[<Fact>]
let ``Render MyRecord with arrays value`` () =
  Expect.similar
    value
    """
export var defaultMyRecord: MyRecord = {
 field1: [],
 field2: []
}
"""
