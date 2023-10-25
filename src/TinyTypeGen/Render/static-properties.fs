module TinyTypeGen.Render.StaticProperties

open System.Reflection

let staticProperties (t: System.Type) =
  t.GetProperties(BindingFlags.Static ||| BindingFlags.Public)

let renderStaticTypeToString (t: System.Type) =
  let properties = t.GetProperties(BindingFlags.Static)
  let props = properties |> Array.map (fun x -> x.GetValue(null))
  printfn "%A" props
  // properties
  "hello"
