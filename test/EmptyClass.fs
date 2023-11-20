module Test.EmptyClass

open System.Text.Json.Serialization
open Expecto
open Xunit

type EmptyRecord =
    { Skip: Skippable<unit> }

    static member Instance = { Skip = Skippable.Skip }

[<Fact>]
let ``Empty record`` () =

    let rendered, value = renderTypeAndValue typeof<EmptyRecord>

    Expect.similar
        rendered
        """
export type EmptyRecord = {
   skip: System_Text_Json_Serialization.Skippable<Microsoft_FSharp_Core.Unit>
}
"""

    Expect.similar
        value
        """
export var defaultEmptyRecord: EmptyRecord = {
  skip: undefined
}
"""

type EmptyClass() =
    let x = "foo"

let rendered, value = renderTypeAndValue typeof<EmptyClass>

[<Fact>]
let ``Empty class`` () =
    Expect.similar
        rendered
        """
export type EmptyClass = {

}
"""

[<Fact>]
let ``Empty class value`` () =
    Expect.similar
        value
        """
export var defaultEmptyClass: EmptyClass = {
}
"""
