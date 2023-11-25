namespace Test

open System.Text.Json
open System.Text.RegularExpressions
open Expecto
open TsGen

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
    
    let serializeWithEncoding<'t> (encoding: JsonUnionEncoding) (v: 't) =
        serializeWithOptions (JsonFSharpOptions(encoding)) v
    
    let deserializeWithEncoding<'t> (encoding: JsonUnionEncoding) (v: string) =
        deserializeWithOptions<'t> (JsonFSharpOptions(encoding)) v

[<AutoOpen>]
module Helpers =

    let configureFor unionEncoding (t: System.Type) =
        Config.withDefaults ()
        |> Config.withJsonUnionEncoding unionEncoding
        |> Config.forTypes [ t ]
        |> Config.build

    let renderModules t =
        let config = Config.withDefaults () |> Config.forTypes t |> Config.build

        config.renderTypes ()

    let renderModuleWithCustomEncoding unionEncoding t =
        let config = configureFor unionEncoding t
        let renderedTypes = config.renderTypes ()
        renderedTypes |> List.find (fun (m, _) -> m.Name = t.Namespace) |> snd

    let renderModule t =
        renderModuleWithCustomEncoding Gen.defaultJsonUnionEncoding t

    let renderCustomTypeDef jsonEncoding (t: System.Type) =
        let config = configureFor jsonEncoding t
        config.renderType t

    let renderTypeDef t =
        renderCustomTypeDef Gen.defaultJsonUnionEncoding t

    let renderValueWithCustomEncoding encoding t =
        let config = configureFor encoding t
        config.renderValue t

    let renderValue t =
        renderValueWithCustomEncoding Gen.defaultJsonUnionEncoding t

    let renderCustomTypeAndValue unionEncoding t =
        renderCustomTypeDef unionEncoding t, renderValueWithCustomEncoding unionEncoding t

    let renderTypeAndValue t =
        renderCustomTypeAndValue Gen.defaultJsonUnionEncoding t
