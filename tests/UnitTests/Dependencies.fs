module Test.Dependencies

open TinyTypeGen
open Expecto

type X = { ids: int array }

let tests =
  testList
    "Dependencies"
    [
      testCase
        "enum list should have IEnumerable dependency"
        (fun () ->
          let generator = GeneratorBuilder().AddTypes([| typeof<X> |]).Build()
          let modules = generator.GetModules()
          let m = modules |> Seq.find (fun x -> x.Name = "Test_Dependencies")
          // generator.GetRawModuleDependencies
          ()
        )
    ]
