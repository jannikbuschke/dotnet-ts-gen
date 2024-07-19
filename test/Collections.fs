module Test.Collections

open Expecto
open Xunit

type OtherRecord = { Id: string }

type MyRecord =
  { SimpleList: int list
    ComplexList: OtherRecord list }

let recordDefinition, recordValue = renderTypeAndValue typedefof<MyRecord>

[<Fact>]
let ``Record with FSharpList property definition`` () =

  Expect.similar
    recordDefinition
    """
export type MyRecord = {
  simpleList: Microsoft_FSharp_Collections.FSharpList<System.Int32>
  complexList: Microsoft_FSharp_Collections.FSharpList<OtherRecord>
}
"""

// [<Fact>]
// let ``Record with FSharpList property value`` () =
//   Expect.similar
//     recordValue
//     """
// export var defaultMyRecord: MyRecord = {
//   simpleList: [],
//   complexList: []
// }
// """

let definition, value = renderTypeAndValue typedefof<List<string>>

[<Fact>]
let ``FSharpList definition definition`` () =
  Expect.similar definition """export type FSharpList<T> = Array<T>"""

// [<Fact>]
// let ``FSharpList definition value`` () =
//   Expect.similar value """export var defaultFSharpList: <T>(t:T) => FSharpList<T> = <T>(t:T) => []"""

//
// [<Fact>]
// let ``Render IEnumerable of KeyValue pairs`` () =
//   let modules =
//     generateModules
//       [ typeof<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>>> ]
//
//   let getModule name =
//     modules |> List.find (fun v -> v.Name = name)
//
//   let fs = getModule (NamespaceName "System.Collections.Generic")
//
//   let rendered = renderModule fs
//   // let rendered = renderTypeAndValue typeof<System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string,string>>>
//
//   Expect.similar
//     rendered
//     """
// //////////////////////////////////////
// // This file is auto generated //
// //////////////////////////////////////
// import {TsType} from "./"
// export type KeyValuePair<TKey,TValue> = {Key:TKey,Value:TValue}
// export var defaultKeyValuePair: <TKey,TValue>(tKey:TKey,tValue:TValue) => KeyValuePair<TKey,TValue> = <TKey,TValue>(tKey:TKey,tValue:TValue) => ({Key:tKey,Value:tValue})
// export type IEnumerable<T> = Array<T>
// export var defaultIEnumerable: <T>(t:T) => IEnumerable<T> = <T>(t:T) => []
// """
//
// type RecordWithKeyValueList =
//   { KeysAndValues: System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, int>> }
//
// [<Fact>]
// let ``Render IEnumerable of KeyValue pairs property`` () =
//   let rendered = renderTypeAndValue2 typeof<RecordWithKeyValueList>
//
//   Expect.similar
//     rendered
//     """export type RecordWithKeyValueList = {
//   keysAndValues: System_Collections_Generic.IEnumerable<System_Collections_Generic.KeyValuePair<System.String,System.Int32>>
// }
// export var defaultRecordWithKeyValueList: RecordWithKeyValueList = {
//   keysAndValues: [],
// }
// """
// // keysAndValues: System_Collections_Generic.defaultIEnumerable<System_Collections_Generic.KeyValuePair<System.defaultString,System.defaultInt32>>(System_Collections_Generic.defaultKeyValuePair),
