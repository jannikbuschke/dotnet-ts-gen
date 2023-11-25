module Test.Unions.RecordWithOption

open System
open Expecto
open Xunit
open Test

type Record = { Id: string }

type RecordWithOption =
  { Id: Guid
    NumberOption: Record option }

[<Fact>]
let ``Render record with option`` () =

  let rendered, value = renderTypeAndValue typedefof<RecordWithOption>

  Expect.similar
    rendered
    """
export type RecordWithOption = {
  id: System.Guid
  numberOption: Microsoft_FSharp_Core.FSharpOption<Record>
}
"""

  Expect.similar
    value
    """
export var defaultRecordWithOption: RecordWithOption = {
 id: '00000000-0000-0000-0000-000000000000',
 numberOption: null
}
"""
