using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FSharp.Core;
using WebApplication5;

namespace CSharpWebapp.Controllers3;

public static class StaticClass
{
    public record ResponseDto2(int[] Ids);
}

public record ResponsDto(int[] Ids);

[Route("fsharp")]
public class HomeController : ControllerBase
{

    [HttpGet("get-result")]
    public FSharpResult<string, string> Index3() => FSharpResult<string, string>.NewOk("Ok");

    [HttpGet("get-result-async")]
    public Task<FSharpResult<string, string>> Index4() => Task.FromResult(FSharpResult<string, string>.NewOk("Ok"));
}
