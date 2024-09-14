module IntegrationTests

open WebApplication2
open Xunit
open Microsoft.AspNetCore.Mvc.Testing


[<Fact>]
let Test1 () =
  let t = Test.CreateDynamicType()
  let factory = WebApplicationFactory<WebApplication2.Program>()
  let httpEndpoints = TinyTypeGen.AspNetCore.getEndpoints (factory.Services)
  let builder = new TinyTypeGen.GeneratorBuilder()
  builder.AddEndpoints(httpEndpoints |> Seq.toArray)

  let generator = builder.Build()

  generator.RenderTypesToDirectory("./@client/")
  generator.RenderApiToFile("./@client/api.ts")

  let inputs = httpEndpoints |> List.map _.Request
  let properties = inputs |> List.map (fun x -> x.GetProperties())
  Assert.Equal(5, httpEndpoints.Length)
  ()
