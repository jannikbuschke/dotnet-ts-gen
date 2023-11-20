module Test.StronglyTypedId

open Expecto
open Xunit

type MyRecordId = MyRecordId of System.Guid
type MyRecord = { Id: MyRecordId }

let typedef, value = renderTypeAndValue typedefof<MyRecordId>

[<Fact>]
let ``Render StronglyTypedId`` () =
    Expect.similar
        typedef
        """
export var MyRecordId_AllCases = [ "MyRecordId" ] as const
export var defaultMyRecordId_Case_MyRecordId = '00000000-0000-0000-0000-000000000000'
export var defaultMyRecordId = defaultMyRecordId_Case_MyRecordId as MyRecordId
"""

[<Fact>]
let ``Render StronglyTypedId value`` () =
    Expect.similar
        value
        """
export type MyRecordId_Case_MyRecordId = System.Guid
export type MyRecordId = MyRecordId_Case_MyRecordId
export type MyRecordId_Case = "MyRecordId"
"""

let typedef2, renderedValue2 = renderTypeAndValue typedefof<MyRecord>

[<Fact>]
let ``Render Record with StronglyTypedId`` () =
    Expect.similar
        typedef2
        """
export type MyRecord = {
  id: MyRecordId
}
"""

[<Fact>]
let ``Render Record with StronglyTypedId value`` () =
    Expect.similar
        renderedValue2
        """
export var defaultMyRecord: MyRecord = {
  id: defaultMyRecordId
}
"""
