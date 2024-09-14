using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers;

public record RequestDto(string Name, int Age);

[Route("api")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpPost("post")]
    public ActionResult Post()
    {
        return Ok();
    }

    [HttpGet("get")]
    public ActionResult Index()
    {
        return Ok("Hello world");
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
    public ActionResult CompltextRequest([FromQuery] RequestDelegate r)
    {
        return Ok();
    }
}
