module Test.Unions.RecordWithResult

open System
open Expecto
open Xunit
open Test

type RecordWithResult =
  { Id: Guid
    Result: Result<int, string> }

[<Fact>]
let ``Result property`` () =
  let rendered, value = renderTypeAndValue typedefof<RecordWithResult>

  Expect.similar
    rendered
    """
export type RecordWithResult = {
  id: System.Guid
  result: Microsoft_FSharp_Core.FSharpResult<System.Int32,System.String>
}
"""

  Expect.similar
    value
    """
export var defaultRecordWithResult: RecordWithResult = {
  id: '00000000-0000-0000-0000-000000000000',
  result: Microsoft_FSharp_Core.defaultFSharpResult(System.defaultInt32,System.defaultString)
}
"""
