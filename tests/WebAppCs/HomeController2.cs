using Microsoft.AspNetCore.Mvc;
using WebApplication5;

namespace CSharpWebapp.Controllers2;

public static class StaticClass
{
    public record ResponseDto2(int[] Ids);
}

public record ResponsDto(int[] Ids);

[Route("api2")]
public class HomeController : ControllerBase
{

    [HttpGet("get2")]
    public StaticClass.ResponseDto2 Index2() => new([]);

    [HttpGet("get")]
    public ResponsDto Index() => new([]);

    [HttpGet("get3")]
    public ModuleClass.Dto Index3() => new();
}
