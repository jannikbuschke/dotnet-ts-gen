module TinyTypeGen.PredefinedTypes

open System
open System.Text.Json
open System.Text.Json.Nodes
open System.Text.Json.Serialization

let emptyPredefinedValues =
  {
    InlineDefaultValue = Some "''"
    Name = None
    Definition = None
    Signature = None
    Dependencies = []
  }

let tryPredefinedType (defaultTypes: PreDefinedTypes) (propertyType: Type) =
  match
    defaultTypes.TryGetValue(
      if propertyType.IsGenericType && not propertyType.IsGenericTypeDefinition then
        propertyType.GetGenericTypeDefinition()
      else
        propertyType
    )
  with
  | true, value -> Some value
  | _ -> None

let isDefined (defaultTypes: PreDefinedTypes) (propertyType: Type) =
  match tryPredefinedType defaultTypes propertyType with
  | Some _ -> true
  | None -> false

let defaultTypes =
  [
    typeof<string>, PredefinedType.New "string"
    typeof<Uri>, PredefinedType.New "string"
    typeof<Version>, PredefinedType.New "string"
    typeof<Byte>, PredefinedType.New "number"
    typeof<Guid>, PredefinedType.New "`${string}-${string}-${string}-${string}-${string}`"
    typeof<int>, PredefinedType.New "number"
    typeof<single>, PredefinedType.New "number"
    typeof<decimal>, PredefinedType.New "number"
    typeof<int16>, PredefinedType.New "number"
    typeof<uint16>, PredefinedType.New "number"
    typeof<int8>, PredefinedType.New "number"
    typeof<uint8>, PredefinedType.New "number"
    typeof<uint32>, PredefinedType.New "number"
    typeof<uint64>, PredefinedType.New "number"
    typeof<int64>, PredefinedType.New "number"
    typeof<Int128>, PredefinedType.New "number"
    typeof<Char>, PredefinedType.New "string"
    typeof<float>, PredefinedType.New "number"
    typeof<double>, PredefinedType.New "number"
    typeof<bigint>, PredefinedType.New "number"
    typeof<bool>, PredefinedType.New "boolean"
    typeof<obj>, { emptyPredefinedValues with InlineDefaultValue = Some "{}" }
    typeof<unit>, PredefinedType.New "null"
    typedefof<System.ValueTuple<_, _>>,
    { emptyPredefinedValues with
        Signature = Some "T1, T2"
        Definition = Some "[T1,T2]"
    }
    typedefof<System.ValueTuple<_, _, _>>,
    { emptyPredefinedValues with
        // Signature = Some "T1, T2"
        Name = Some "ValueTuple3"
        Definition = Some "[T1,T2,T3]"
    }
    typedefof<System.Tuple<_, _>>,
    { emptyPredefinedValues with
        Name = Some "Tuple"
        Definition = Some "[T1,T2]"
    }
    typedefof<System.Tuple<_, _, _>>,
    { emptyPredefinedValues with
        Name = Some "Tuple3"
        Definition = Some "[T1,T2,T3]"
    }
    typedefof<Collections.Generic.HashSet<_>>, PredefinedType.New "Array<T>"
    typedefof<Collections.Generic.IEnumerable<_>>, { emptyPredefinedValues with Definition = Some "Array<T>" }
    typedefof<Collections.Generic.IList<_>>, { emptyPredefinedValues with Definition = Some "Array<T>" }
    typedefof<Collections.ObjectModel.ReadOnlyCollection<_>>,
    { emptyPredefinedValues with Definition = Some "Array<T>" }
    typedefof<Collections.Generic.IReadOnlyCollection<_>>, { emptyPredefinedValues with Definition = Some "Array<T>" }
    typedefof<Collections.Generic.IReadOnlyList<_>>, { emptyPredefinedValues with Definition = Some "Array<T>" }
    typedefof<Collections.Generic.IReadOnlySet<_>>, { emptyPredefinedValues with Definition = Some "Array<T>" }
    typedefof<Collections.Generic.ICollection<_>>, { emptyPredefinedValues with Definition = Some "Array<T>" }
    typedefof<Collections.Generic.List<_>>, { emptyPredefinedValues with Definition = Some "Array<T>" }
    typedefof<Skippable<_>>, { emptyPredefinedValues with Definition = Some "T | undefined" }
    typedefof<DateTime>, PredefinedType.New "`${number}-${number}-${number}T${number}:${number}:${number}`"
    typedefof<DateTimeOffset>,
    PredefinedType.New "`${number}-${number}-${number}T${number}:${number}:${number}${\"+\"|\"-\"}${number}:${number}`"
    typedefof<TimeSpan>, PredefinedType.New "`${string}:${string}:${string}`"
    typedefof<DateOnly>, PredefinedType.New "`${number}-${number}-${number}`"
    typedefof<TimeOnly>, PredefinedType.New "`${number}:${number}:${number}`"
    typedefof<Collections.Generic.KeyValuePair<_, _>>,
    { emptyPredefinedValues with Definition = Some "({key:TKey,value:TValue})" }
    typedefof<Collections.Generic.Dictionary<_, _>>, PredefinedType.New "{ [key: string]: TValue }"
    typedefof<Collections.Generic.IDictionary<_, _>>, PredefinedType.New "{ [key: string]: TValue }"
    typedefof<Collections.Generic.IReadOnlyDictionary<_, _>>, PredefinedType.New "{ [key: string]: TValue }"
    typedefof<Type>, PredefinedType.New "{  }"
    typedefof<Nullable<_>>, PredefinedType.New "T | null"
    typedefof<JsonArray>, PredefinedType.New "JsonElement[];"
    typedefof<JsonNode>,
    { emptyPredefinedValues with
        Definition =
          Some
            " | string
 | number
 | boolean
 | null
 | { [key: string]: JsonElement }
 | JsonElement[];
        "
    }
    typedefof<JsonElement>,
    { emptyPredefinedValues with
        Definition =
          Some
            " | string
 | number
 | boolean
 | null
 | { [key: string]: JsonElement }
 | JsonElement[];
        "
    }
    typedefof<JsonNode>,
    { emptyPredefinedValues with
        Definition = Some "{ [key: string]: JsonElement };"
        Dependencies = [ typedefof<JsonElement> ]

    }
    typedefof<JsonDocument>,
    { emptyPredefinedValues with
        Definition = Some "{ [key: string]: JsonElement };"
        Dependencies = [ typedefof<JsonElement> ]

    }
  ]
  |> dict
  |> System.Collections.Generic.Dictionary
