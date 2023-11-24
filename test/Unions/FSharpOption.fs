module Test.FSharpTypes

open Expecto
open Xunit

let rendered, value = renderTypeAndValue typedefof<Option<string>>

[<Fact>]
let ``Render FSharp Option`` () =

    Expect.similar
        rendered
        """
export type FSharpOption<T> = T | null
"""

[<Fact>]
let ``Render FSharp Option value`` () =
    Expect.similar
        value
        """
export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null
"""
