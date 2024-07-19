module Test.Generics

open Expecto
open Xunit

type GenericRecord<'t> = { GenericProperty: 't }

type Localizable<'a> =
  { Value: 'a
    Localizations: Map<string, 'a> }

[<Fact>]
let ```not render typedef Localizable<string>`` () =
  // runtime type, should not be rendered (only generic typedefinition)
  let rendered, _ = renderTypeAndValue typeof<Localizable<string>>

  Expect.similar rendered ""

let rendered, value = renderTypeAndValue typedefof<Localizable<string>>

[<Fact>]
let ```render typedefof Localizable<string>`` () =
  Expect.similar
    rendered
    """
export type Localizable<a> = {
  value: a
  localizations: Microsoft_FSharp_Collections.FSharpMap<System.String,a>
}
"""

// [<Fact>]
// let ```render value Localizable<string>`` () =
//   Expect.similar
//     value
//     """
// export var defaultLocalizable: <a>(defaulta:a) => Localizable<a> = <a>(defaulta:a) => ({
//  value: defaulta,
//  localizations: ({})
// })
// """

type Localizable2<'a> =
  { Value: 'a
    Localizations: Map<string, 'a> }
