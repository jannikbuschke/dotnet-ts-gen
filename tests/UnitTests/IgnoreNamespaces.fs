module Test.IgnoreNamespaces

open TinyTypeGen
open Expecto

type X = { X: Microsoft.Win32.Registry }

let tests =
  testList
    "IgnoreNamespaces"
    [
      testCase
        "should ignore"
        (fun () ->
          let modules =
            GeneratorBuilder().AddToIgnoredNamespaces("Microsoft_Win32").AddTypes([| typeof<X> |]).Build().GetModules()

          Expecto.Expect.isNone
            (modules |> Seq.tryFind (fun x -> x.Name = "Microsoft_Win32"))
            "Should have expected module"
        )
      testCase
        "should not ignore"
        (fun () ->
          let modules = GeneratorBuilder().AddTypes([| typeof<X> |]).Build().GetModules()

          Expecto.Expect.isSome
            (modules |> Seq.tryFind (fun x -> x.Name = "Microsoft_Win32"))
            "Should have expected module"
        )
    ]
