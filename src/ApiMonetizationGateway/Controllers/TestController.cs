using Microsoft.AspNetCore.Mvc;

namespace ApiMonetizationGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok(new { message = "Hello from internal service", timestamp = DateTime.UtcNow });
    }
}