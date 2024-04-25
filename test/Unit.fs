module Test.Unit

open Expecto
open Xunit

let rendered, value = renderTypeAndValue typeof<unit>

[<Fact>]
let ``Unit`` () =
  Expect.similar
    rendered
    """
export type Unit = null
"""

[<Fact>]
let ``Unit value`` () =
  Expect.similar
    value
    """
export var defaultUnit: Unit = null
"""
