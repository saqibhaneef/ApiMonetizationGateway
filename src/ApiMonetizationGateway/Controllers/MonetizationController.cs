using Microsoft.AspNetCore.Mvc;

namespace ApiMonetizationGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MonetizationController : ControllerBase
{
    [HttpGet("PaidApi")]
    public IActionResult PaidApi()
    {
        return Ok(new { message = "Hello from internal service", timestamp = DateTime.UtcNow });
    }
}