module Test.PropertyCasingTests

open System.Text.Json
open Expecto
open Test
open TinyTypeGen

type PersonRecord =
  {
    FirstName: string
    LastName: string
    Age: int
  }

type Wrapper<'t> =
  {
    Value: 't
    IsValid: bool
  }

[<RequireQualifiedAccess>]
type Status =
  | Active
  | Inactive
  | Pending of string

type NamedFieldsDu =
  | Circle of Radius: float
  | Rectangle of Width: float * Height: float

// Parses the top-level property names from a flat JSON object.
let parsePropertyNames (json: string) =
  use doc = JsonDocument.Parse json
  doc.RootElement.EnumerateObject() |> Seq.map _.Name |> Seq.toList

let samplePerson =
  {
    FirstName = "Alice"
    LastName = "Smith"
    Age = 30
  }

let tests =
  testList
    "PropertyCasing"
    [

      testCase
        "Record, camelCase (default)"
        (fun () ->
          typedefof<PersonRecord>
          |> definition
          |> Expect.equal
            """
export type PersonRecord = {
  firstName: System.String
  lastName: System.String
  age: System.Int32
}
"""
        )

      testCase
        "Record, PascalCase"
        (fun () ->
          typedefof<PersonRecord>
          |> definitionWithCasing PropertyCasing.PascalCase
          |> Expect.equal
            """
export type PersonRecord = {
  FirstName: System.String
  LastName: System.String
  Age: System.Int32
}
"""
        )

      testCase
        "Record, snake_case"
        (fun () ->
          typedefof<PersonRecord>
          |> definitionWithCasing PropertyCasing.SnakeCase
          |> Expect.equal
            """
export type PersonRecord = {
  first_name: System.String
  last_name: System.String
  age: System.Int32
}
"""
        )

      testCase
        "Generic record, camelCase (default)"
        (fun () ->
          typedefof<Wrapper<string>>
          |> definition
          |> Expect.equal
            """
export type Wrapper<t> = {
  value: t
  isValid: System.Boolean
}
"""
        )

      testCase
        "Generic record, PascalCase"
        (fun () ->
          typedefof<Wrapper<string>>
          |> definitionWithCasing PropertyCasing.PascalCase
          |> Expect.equal
            """
export type Wrapper<t> = {
  Value: t
  IsValid: System.Boolean
}
"""
        )

      testCase
        "Generic record, snake_case"
        (fun () ->
          typedefof<Wrapper<string>>
          |> definitionWithCasing PropertyCasing.SnakeCase
          |> Expect.equal
            """
export type Wrapper<t> = {
  value: t
  is_valid: System.Boolean
}
"""
        )

      // ── Discriminated unions ─────────────────────────────────────────
      // DU case names and field labels are not properties and are not
      // affected by PropertyCasing — all three casing modes produce the
      // same DU output.

      testCase
        "DU, fieldless and single-field cases (default encoding)"
        (fun () ->
          typedefof<Status>
          |> definition
          |> Expect.stringStart
            """
export type Status_Case_Active = { Case: "Active",  }
export type Status_Case_Inactive = { Case: "Inactive",  }
export type Status_Case_Pending = { Case: "Pending", Fields: System.String }
export type Status = Status_Case_Active | Status_Case_Inactive | Status_Case_Pending
"""
        )

      testCase
        "DU, PascalCase does not change case names or field labels"
        (fun () ->
          typedefof<Status>
          |> definitionWithCasing PropertyCasing.PascalCase
          |> Expect.stringStart
            """
export type Status_Case_Active = { Case: "Active",  }
export type Status_Case_Inactive = { Case: "Inactive",  }
export type Status_Case_Pending = { Case: "Pending", Fields: System.String }
export type Status = Status_Case_Active | Status_Case_Inactive | Status_Case_Pending
"""
        )

      testCase
        "DU, snake_case does not change case names or field labels"
        (fun () ->
          typedefof<Status>
          |> definitionWithCasing PropertyCasing.SnakeCase
          |> Expect.stringStart
            """
export type Status_Case_Active = { Case: "Active",  }
export type Status_Case_Inactive = { Case: "Inactive",  }
export type Status_Case_Pending = { Case: "Pending", Fields: System.String }
export type Status = Status_Case_Active | Status_Case_Inactive | Status_Case_Pending
"""
        )

      testCase
        "DU with named fields, camelCase (default)"
        (fun () ->
          typedefof<NamedFieldsDu>
          |> definition
          |> Expect.stringStart
            """
export type NamedFieldsDu_Case_Circle = { Case: "Circle", Fields: System.Double }
export type NamedFieldsDu_Case_Rectangle = { Case: "Rectangle", Fields: { width: System.Double, height: System.Double } }
"""
        )

      // ── JSON serializer cross-validation ─────────────────────────────
      // Each test serializes a value with the JsonNamingPolicy that corresponds
      // to a PropertyCasing mode, then checks that every property name the
      // serializer actually produces appears in the TypeScript definition.
      // This validates that our PropertyCasing and JsonNamingPolicy stay in sync.

      testCase
        "CamelCase: JsonNamingPolicy.CamelCase keys match TS property names"
        (fun () ->
          let opts = JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
          let jsonKeys = JsonSerializer.Serialize(samplePerson, opts) |> parsePropertyNames
          // jsonKeys = ["firstName"; "lastName"; "age"]
          let ts = typedefof<PersonRecord> |> definition
          jsonKeys |> List.iter (fun k -> ts |> Expect.stringContains k)
        )

      testCase
        "PascalCase: no naming policy keys match TS property names"
        (fun () ->
          // System.Text.Json with no PropertyNamingPolicy preserves the
          // original PascalCase names that F# records have by convention.
          let opts = JsonSerializerOptions()
          let jsonKeys = JsonSerializer.Serialize(samplePerson, opts) |> parsePropertyNames
          // Expect.equal jsonKeys ["FirstName"; "LastName"; "Age"] "keys"
          let ts = typedefof<PersonRecord> |> definitionWithCasing PropertyCasing.PascalCase
          jsonKeys |> List.iter (fun k -> ts |> Expect.stringContains k)
        )

      testCase
        "SnakeCase: JsonNamingPolicy.SnakeCaseLower keys match TS property names"
        (fun () ->
          let opts =
            JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower)
          let jsonKeys = JsonSerializer.Serialize(samplePerson, opts) |> parsePropertyNames
          // jsonKeys = ["first_name"; "last_name"; "age"]
          let ts = typedefof<PersonRecord> |> definitionWithCasing PropertyCasing.SnakeCase
          jsonKeys |> List.iter (fun k -> ts |> Expect.stringContains k)
        )

      testCase
        "DU: Case discriminator value in JSON matches string literal in TS"
        (fun () ->
          // FSharp.SystemTextJson serializes the union case name as the "Case" field.
          // TinyTypeGen emits the same string as a literal in the TS case type.
          // This test verifies the two agree for each case.
          let tsDefinition = typedefof<Status> |> definition

          let cases =
            [
              Status.Active, "Active"
              Status.Inactive, "Inactive"
              Status.Pending "test", "Pending"
            ]

          for value, expectedCaseName in cases do
            let json = serializeWithCustomEncoding value
            // e.g. {"Case":"Pending","Fields":"test"}
            json |> Expect.stringContains (sprintf "\"%s\"" expectedCaseName)
            tsDefinition |> Expect.stringContains (sprintf "\"%s\"" expectedCaseName)
        )
    ]
