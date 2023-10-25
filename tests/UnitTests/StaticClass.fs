module Test.StaticClass

open System.Reflection
open Expecto
open TinyTypeGen.Render.StaticProperties

// --- type fixtures ---

type StaticClass() =
  static member StaticProperty = "static property value"

type MultiStaticClass() =
  static member Name = "hello"
  static member Count = 42
  static member Flag = true

type NoStaticClass() =
  member _.InstanceProp = "instance only"

type MixedClass() =
  member _.InstanceProp = "instance"
  static member StaticProp = "static"
  static member StaticInt = 7

// --- helpers ---

let propNames (props: PropertyInfo array) =
  props |> Array.map (fun p -> p.Name) |> Array.toList |> List.sort

let propValue (props: PropertyInfo array) name =
  props |> Array.find (fun p -> p.Name = name) |> (fun p -> p.GetValue(null))

// --- tests ---

let tests =
  testList
    "StaticClass"
    [
      testCase
        "staticProperties returns the single static property"
        (fun () ->
          let props = staticProperties typedefof<StaticClass>

          Expect.equal props.Length 1 "should have exactly one static property"
          Expect.equal props[0].Name "StaticProperty" "property name should match"
        )

      testCase
        "staticProperties returns correct value for string property"
        (fun () ->
          let props = staticProperties typedefof<StaticClass>
          let value = props[0].GetValue(null) :?> string

          Expect.equal value "static property value" "should equal static property value"
        )

      testCase
        "staticProperties returns all public static properties"
        (fun () ->
          let props = staticProperties typedefof<MultiStaticClass>

          Expect.equal props.Length 3 "should have three static properties"
          Expect.equal (propNames props) [ "Count"; "Flag"; "Name" ] "property names should match"
        )

      testCase
        "staticProperties returns correct values for multiple types"
        (fun () ->
          let props = staticProperties typedefof<MultiStaticClass>

          Expect.equal (propValue props "Name" :?> string) "hello" "Name should be 'hello'"
          Expect.equal (propValue props "Count" :?> int) 42 "Count should be 42"
          Expect.equal (propValue props "Flag" :?> bool) true "Flag should be true"
        )

      testCase
        "staticProperties returns empty array when no static properties exist"
        (fun () ->
          let props = staticProperties typedefof<NoStaticClass>

          Expect.equal props.Length 0 "should have no static properties"
          Expect.isEmpty props "array should be empty"
        )

      testCase
        "staticProperties only returns static properties, not instance properties"
        (fun () ->
          let props = staticProperties typedefof<MixedClass>

          Expect.equal props.Length 2 "should have exactly two static properties"
          let names = propNames props
          Expect.contains names "StaticProp" "StaticProp should be included"
          Expect.contains names "StaticInt" "StaticInt should be included"
          Expecto.Expect.isFalse (names |> List.contains "InstanceProp") "InstanceProp should not be included"
        )

      testCase
        "staticProperties returns correct values from mixed class"
        (fun () ->
          let props = staticProperties typedefof<MixedClass>

          Expect.equal (propValue props "StaticProp" :?> string) "static" "StaticProp value should be 'static'"
          Expect.equal (propValue props "StaticInt" :?> int) 7 "StaticInt value should be 7"
        )

      testCase
        "staticProperties are all marked IsStatic"
        (fun () ->
          let props = staticProperties typedefof<MultiStaticClass>

          for prop in props do
            Expect.isTrue prop.GetMethod.IsStatic $"property '{prop.Name}' getter should be static"
        )

      testCase
        "Render static class serializes as empty object"
        (fun () ->
          let v = StaticClass()
          let serialized = serializeWithCustomEncoding v

          Test.Expect.similar serialized "{}"
        )

      testCase
        "Render static class definition excludes static property"
        (fun () ->
          let rendered = definition typedefof<StaticClass>

          Test.Expect.similar rendered "export type StaticClass = {}"
        )

      testCase
        "Render mixed class definition only includes instance properties"
        (fun () ->
          let rendered = definition typedefof<MixedClass>

          Test.Expect.similar
            rendered
            "export type MixedClass = {
  instanceProp: System.String
}"

          Expecto.Expect.isFalse
            (rendered.Contains "staticProp")
            "static properties should not appear in the TypeScript type definition"
          Expecto.Expect.isFalse
            (rendered.Contains "staticInt")
            "static properties should not appear in the TypeScript type definition"
        )
    ]
