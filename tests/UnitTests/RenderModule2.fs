module Test.RenderModule2

open System.Collections.Generic

type RecordId = | RecordId of System.Guid

type OtherRecord =
  {
    Id: RecordId
    keyValue: KeyValuePair<string, int>
    keyValues: IEnumerable<KeyValuePair<string, int>>
  }

type ComplexRecord =
  {
    Id: System.Guid
    Name: string
    Number: int
    Items: int list
    Obj: obj
    Option: RecordId option
    Result: Result<int, string>
  }

module X =
  type Foo = { ComplexRecord: ComplexRecord }

  module Y =
    type Bar =
      {
        ComplexRecord: ComplexRecord
        Foo: Foo
      }

let types =
  [ typedefof<ComplexRecord>; typeof<bool>; typeof<X.Y.Bar>; typeof<X.Foo> ]

// [<Fact>]
// let ``RenderModule`` () =
//     let m = renderModule typeof<X.Y.Bar>
// Expect.similar m ""
