module Test.RecordWithList

open Expecto
open Xunit

type MyOtherRecord = { Id: string }

type MyRecord =
  { Items: MyOtherRecord list
    Numbers: int list }


[<Fact>]
let ``Render Record with list`` () =

  let rendered, value = renderTypeAndValue typedefof<MyRecord>

  Expect.similar
    rendered
    """
export type MyRecord = {
  items: Microsoft_FSharp_Collections.FSharpList<MyOtherRecord>
  numbers: Microsoft_FSharp_Collections.FSharpList<System.Int32>
}
"""

  Expect.similar
    value
    """
export var defaultMyRecord: MyRecord = {
 items: [],
 numbers: []
}
"""
