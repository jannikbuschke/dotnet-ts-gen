module TsGen.NodaTime

open System
open System.Collections.Generic
open TsCodeGen.PredefinedTypes

let addNodaTimeSupport (dict: Dictionary<Type, PredefinedValues>) =
    [ (typedefof<NodaTime.LocalDate>,
       { emptyPredefinedValues with
           InlineDefaultValue = Some "\"2022-12-18\""
           Definition = Some "`${number}-${number}-${number}`" })
      (typedefof<NodaTime.LocalTime>,
       { emptyPredefinedValues with
           InlineDefaultValue = Some "\"00:00:00\""
           Definition = Some "`${number}:${number}:${number}`" })
      (typedefof<NodaTime.Instant>,
       { emptyPredefinedValues with
           InlineDefaultValue = Some "\"9999-12-31T23:59:59.999999999Z\""
           Definition = Some "`${number}-${number}-${number}T${number}:${number}:${number}.${number}Z`" })
      (typedefof<NodaTime.Duration>,
       { emptyPredefinedValues with
           InlineDefaultValue = Some "\"0:00:00\""
           Definition = Some "`${number}:${number}:${number}`" }) ]
    |> List.iter (fun (k, v) -> dict.Add(k, v))
