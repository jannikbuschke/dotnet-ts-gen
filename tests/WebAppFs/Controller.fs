namespace FsApi

open System
open Microsoft.AspNetCore.Mvc

[<CLIMutable>]
type Record = { Foo: int }
type Union =
  | CaseA of int
  | CaseB of string
  | CaseC of Record

[<ApiController>]
[<Route("api/controller")>]
type TestController() =
  [<HttpGet("get-string")>]
  member this.GetString() = "Hello, World!"
  [<HttpGet("get-record")>]
  member this.GetRecord() = { Foo = 42 }
  [<HttpGet("get-union")>]
  // [<ProducesResponseType(typeof<Union>, 200)>]
  member this.GetUnion() = Union.CaseA 42

  [<HttpGet("union-task")>]
  member this.GetUnionTask() =
    System.Threading.Tasks.Task.FromResult(Union.CaseA 42)

  [<HttpGet("get-version")>]
  member this.GetVersion() = Version("1.0.0.0")

  [<HttpGet("get-uri")>]
  member this.GetUri() = Uri("/foo")

  [<HttpPost("get-post-query")>]
  member this.PostQuery([<FromBody>] r: Record) = "Hello, World!"
