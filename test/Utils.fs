namespace Test

open System.Text.RegularExpressions
open Expecto

module Regex =
    let replace (pattern: string) (replacement: string) (input: string) =
        Regex.Replace(input, pattern, replacement)

module Expect =
    let eq actual expected =
        "Should be equal" |> Expect.equal actual expected

    let private normalizeLineFeeds = Regex.replace @"(\r\n|\r|\n)" "\n"

    let private removeSuccessiveLineFeeds = Regex.replace @"[\n]{2,}" "\n"

    let private removeSuccessiveWhiteSpace = Regex.replace @"[ ]{2,}" " "

    let private trim (v: string) = v.Trim()

    let private clean =
        normalizeLineFeeds
        >> removeSuccessiveLineFeeds
        >> removeSuccessiveWhiteSpace
        >> trim

    let similar actual expected =
        "Should be equal" |> Expect.equal (actual |> clean) (expected |> clean)

[<AutoOpen>]
module Helpers =

    let configureFor t =
        TsGen.Config.withDefaults ()
        |> TsGen.Config.forTypes [ t ]
        |> TsGen.Config.build

    let renderModules t =
        let config =
            TsGen.Config.withDefaults () |> TsGen.Config.forTypes t |> TsGen.Config.build

        config.renderTypes ()

    let renderModule t =
        let config = configureFor t
        let renderedTypes = config.renderTypes ()
        renderedTypes |> List.find (fun (m, _) -> m.Name = t.Namespace) |> snd

    let renderTypeDef t =
        let config = configureFor t
        config.renderType t

    let renderValue t =
        let config = configureFor t
        config.renderValue t

    let renderTypeAndValue t = renderTypeDef t, renderValue t
