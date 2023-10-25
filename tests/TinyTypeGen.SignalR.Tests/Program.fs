module Program

open TinyTypeGen.SignalR.Tests

[<EntryPoint>]
let main argv =
  TinyTypeGen.SignalR.Tests.Test.generate typeof<MyHub>
  0
