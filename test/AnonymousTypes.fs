module Test.AnonymousRecord

open Expecto
open Xunit

type AnonymousRecord = {| Id: string; Name:string |}

type RecordWithAnonymousField = {
  Field: {| Id: string; Name:string |}
}

[<Fact>]
let ``Render AnonymousRecord`` () =
  let rendered = renderTypeDef typeof<AnonymousRecord>

  Expect.similar
    rendered
    """
export type f__AnonymousType650789795 = {
 id: System.String
 name: System.String
}
"""

[<Fact>]
let ``Render Record with AnonymousRecord field`` () =
  let rendered = renderTypeDef typedefof<RecordWithAnonymousField>

  Expect.similar
    rendered
    """
export type RecordWithAnonymousField = {
 field: ___.f__AnonymousType650789795
}
"""
