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
module Serialization =
    open System.Text.Json.Serialization

    // let options = DefaultSerialize.JsonFSharpOptions
    //
    // let options2 =
    //     JsonFSharpOptions
    //         .Default()
    //         // Add any .WithXXX() calls here to customize the format
    //         .ToJsonSerializerOptions()
    //
    // let serialize v = JsonSerializer.Serialize(v, options2)
    //
    // let deserialize<'t> (v: string) =
    //     JsonSerializer.Deserialize<'t>(v, options2)

    let serializeWithOptions<'t> (options: JsonFSharpOptions) (v: 't) =
        JsonSerializer.Serialize<'t>(v, options.ToJsonSerializerOptions())

    let deserializeWithOptions<'t> (options: JsonFSharpOptions) (v: string) =
        JsonSerializer.Deserialize<'t>(v, options.ToJsonSerializerOptions())

[<AutoOpen>]
module Helpers =

    let configureFor t =
        Config.withDefaults () |> Config.forTypes [ t ] |> Config.build

    let renderModules t =
        let config = Config.withDefaults () |> Config.forTypes t |> Config.build

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
