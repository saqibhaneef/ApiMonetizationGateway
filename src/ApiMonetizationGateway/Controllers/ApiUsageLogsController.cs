using ApiMonetizationGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiMonetizationGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUsageLogsController : ControllerBase
    {
        private readonly IApiUsageLogsService apiUsageLogsService;
        public ApiUsageLogsController(IApiUsageLogsService apiUsageLogsService)
        {
            this.apiUsageLogsService = apiUsageLogsService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var usageLogs = await this.apiUsageLogsService.GetUsageLogs();
            return Ok(usageLogs);
        }

        [HttpGet("GetByCustomer")]
        public async Task<IActionResult> GetByCustomer()
        {
            var customerId = Request.Headers["X-Customer-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(customerId))
            {
                return BadRequest("Missing X-Customer-Id header.");
            }

            var usageLogs = await this.apiUsageLogsService.GetUsageLogsByCustomer(customerId);

            return Ok(usageLogs);
        }
    }
}
