using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using NamespaceA;

namespace CSharpWebapp.Controllers;

public record RequestDto(string Name, int Age);

[Route("api")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpPost("index")]
    public ActionResult IndexPost()
    {
        return Ok();
    }
    [HttpGet("index")]
    public string IndexGet()
    {
        return "";
    }

    [HttpPost("post")]
    public ActionResult Post()
    {
        return Ok();
    }

    [HttpGet("get-dynamic")]
    public ExpandoObject GetExpando()
    {
        return new ExpandoObject();
    }

    [HttpGet("get-dynamic")]
    public DynamicObject GetDynamic()
    {
        return null;
    }
 
    [HttpGet("get")]
    public ActionResult Index()
    {
        return Ok("Hello world");
    }

    [HttpGet("get-class-with-many-deps")]
    public ClassWithManyDeps ClassA()
    {
        return null;
    }


    [HttpGet("multiple-query-parameters2")]
    public ActionResult MultipleQueryParameters2(string name, int age)
    {
        if (!this.ModelState.IsValid)
        {
            return BadRequest(this.ModelState);
        }

        return Ok(new { name, age });
    }

    [HttpGet("multiple-query-parameters")]
    public ActionResult MultipleQueryParameters([FromQuery] string name, [FromQuery] int age)
    {
        if (!this.ModelState.IsValid)
        {
            return BadRequest(this.ModelState);
        }

        return Ok(new { name, age });
    }

    [HttpGet("complex-request-object")]
    public ActionResult CompltexRequest([FromQuery] RequestDto r)
    {
        return Ok();
    }
}
