module TsGen.PredefinedTypes

open System.Text.Json.Serialization

type PredefinedValues =
  { InlineDefaultValue: string option
    Name: string option
    Definition: string option
    Signature: string option
    Dependencies: System.Type list }

let emptyPredefinedValues =
  { InlineDefaultValue = Some "''"
    Name = None
    Definition = None
    Signature = None
    Dependencies = [] }

type PreDefinedTypes = System.Collections.Generic.IDictionary<System.Type, PredefinedValues>

let defaultTypes =
  [ (typeof<string>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "''"
         Definition = Some "string" })
    (typeof<System.Byte>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "0"
         Definition = Some "number" })

    (typedefof<Option<_>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "null"
         Definition = Some "T | null" })

    (typeof<System.Guid>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some $"'{(System.Guid.Empty.ToString())}'"
         Definition = Some "`${string}-${string}-${string}-${string}-${string}`" })
    (typeof<int>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "0"
         Definition = Some "number" })
    (typeof<uint32>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "0"
         Definition = Some "number" })
    (typeof<int64>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "0"
         Definition = Some "number" })
    (typeof<float>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "0.0"
         Definition = Some "number" })
    (typeof<double>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "0.0"
         Definition = Some "number" })
    (typeof<bigint>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "0"
         Definition = Some "number" })
    (typeof<bool>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "false"
         Definition = Some "boolean" })
    (typeof<obj>, { emptyPredefinedValues with InlineDefaultValue = Some "{}" })
    (typeof<unit>,
     { emptyPredefinedValues with
         Definition = Some("{}")
         InlineDefaultValue = Some "({})" })
    (typedefof<System.Tuple<_, _>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "[defaultT1,defaultT2]"
         Definition = Some "[T1,T2]" })
    (typedefof<System.Collections.Generic.IEnumerable<_>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "[]"
         Definition = Some "Array<T>" })
    (typedefof<System.Collections.Generic.IList<_>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "[]"
         Definition = Some "Array<T>" })
    (typedefof<System.Collections.Generic.IReadOnlyCollection<_>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "[]"
         Definition = Some "Array<T>" })
    (typedefof<System.Collections.Generic.IReadOnlyList<_>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "[]"
         Definition = Some "Array<T>" })
    (typedefof<System.Collections.Generic.IReadOnlySet<_>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "[]"
         Definition = Some "Array<T>" })
    (typedefof<System.Collections.Generic.List<_>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "[]"
         Definition = Some "Array<T>" })
    (typedefof<Skippable<_>>,
     { emptyPredefinedValues with
         Definition = Some "T | undefined"
         InlineDefaultValue = Some "undefined" })
    (typedefof<obj>, { emptyPredefinedValues with InlineDefaultValue = Some "{}" })
    (typedefof<System.DateTime>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "\"0001-01-01T00:00:00\""
         Definition = Some "`${number}-${number}-${number}T${number}:${number}:${number}`" })
    (typedefof<System.DateTimeOffset>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "\"0000-00-00T00:00:00+00:00\""
         Definition = Some "`${number}-${number}-${number}T${number}:${number}:${number}${\"+\"|\"-\"}${number}:${number}`" })
    (typedefof<System.TimeSpan>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "\"00:00:00\""
         Definition = Some "`${number}:${number}:${number}`" })
    (typedefof<System.Collections.Generic.KeyValuePair<_, _>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "({Key:defaultTKey,Value:defaultTValue})"
         Definition = Some "({Key:TKey,Value:TValue})" })
    (typedefof<System.Collections.Generic.Dictionary<_, _>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "({})"
         Definition = Some "{ [key: string]: TValue }" })
    (typedefof<System.Type>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "({})"
         Definition = Some "{  }" })
    (typedefof<System.Nullable<_>>,
     { emptyPredefinedValues with
         InlineDefaultValue = Some "null"
         Definition = Some "T | null" }) ]
  |> dict

// let addPredefinedType (t: System.Type, v: PredefinedValues) = defaultTypes.Add(t, v)
