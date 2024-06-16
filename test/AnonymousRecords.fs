module Test.AnonymousRecord

open Expecto
open Xunit

type AnonymousRecord = {| Id: string; Name:string |}

type RecordWithAnonymousField = {
  Field: {| Id: string; Name:string |}
}

[<Fact>]
let ``Render AnonymousRecord`` () =
  let rendered = renderTypeDef typedefof<AnonymousRecord>

  Expect.similar
    rendered
    """
export type f__AnonymousType650789795<Id,Name> = {
 id: Id
 name: Name
}
"""

[<Fact>]
let ``Render Record with AnonymousRecord field`` () =
  let modules = renderModules [typeof<RecordWithAnonymousField> ]
  let rendered = renderTypeDef typedefof<RecordWithAnonymousField>

  Expect.similar
    rendered
    """
export type RecordWithAnonymousField = {
 field: ___.f__AnonymousType650789795<System.String,System.String>
}
"""
