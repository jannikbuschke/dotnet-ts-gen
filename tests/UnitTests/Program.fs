module Tests.Program

open Expecto

[<EntryPoint>]
let main argv =
  let allTests =
    testList
      "All"
      [
        Test.Arrays.tests
        Test.Collections.tests
        Test.Dictionaries.tests
        Test.HashSet.tests
        Test.AnonymousRecord.tests
        Test.EmptyClass.tests
        Test.FSharpMap.tests
        Test.FsFunc.tests
        Test.IgnoreNamespaces.tests
        Test.Json.tests
        Test.ModuleTypes.tests
        Test.Record.tests
        Test.Generics2.tests
        Test.Object.tests
        Test.RecursiveType.tests
        Test.StaticClass.tests
        Test.Unit.tests
        Test.Uri.tests
        Test.ValueTuple.tests
        Test.Dependencies.tests
        Test.Unions.SingleCaseSingleField.tests
        Test.Unions.SingleCaseMultiField.tests
        Test.Unions.MultipleCases.tests
        Test.Unions.MultipleCasesMultipleFields.tests
        Test.Unions.FSharpResult.tests
        Test.Unions.FSharpOption.tests
        Test.Unions.Skippable.tests
        Test.Unions.SingleCaseMultipleFields.tests
        Test.Unions.RecordWithOption.tests
        Test.Unions.RecordWithResult.tests
        Test.Unions.GenericDu.tests
        Test.Unions.GenericDuWithUnwrapRecordCase.tests
        Test.Generics.PaginatedResult.tests
        Test.Generics.PropertyWithGenericArray.tests
        Test.DateOnly.tests
        Test.TimeOnly.tests
        Test.DateTime.tests
        Test.TheasoftTests.tests
        Test.PropertyCasingTests.tests
      ]

  runTestsWithCLIArgs [] argv allTests
