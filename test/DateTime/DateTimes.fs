module Test.DateTime

open System
open Expecto
open Xunit

type SimpleRecord =
  { Id: Guid
    Name: string
    Number: int
    Obj: obj }


[<Fact>]
let ``DateTime definition`` () =

  let definition, value = renderTypeAndValue typedefof<DateTime>
  Expect.similar definition """export type DateTime = `${number}-${number}-${number}T${number}:${number}:${number}`"""

// [<Fact>]
// let ``DateTime value`` () =
//
//   Expect.similar value """export var defaultDateTime: DateTime = "0001-01-01T00:00:00" """
