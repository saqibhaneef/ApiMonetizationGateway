using ApiMonetizationGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiMonetizationGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MonthlySummaryController : ControllerBase
    {
        private readonly IMonthlySummaryService monthlySummaryService;
        public MonthlySummaryController(IMonthlySummaryService monthlySummaryService)
        {
            this.monthlySummaryService = monthlySummaryService;
        }

        [HttpPost("GenerateSummaryManualy")]
        public async Task<IActionResult> GenerateSummaryManualy()
        {
            try
            {
                var usageTracking = HttpContext.RequestServices.GetRequiredService<IUsageTrackingService>();

                await usageTracking.SummarizePreviousMonthAsync();                

                return Ok("Summary generated if data exists.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while generating summary.");
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var summaries = await monthlySummaryService.GetMonthlySummaries();
            return Ok(summaries);
        }

        [HttpGet("GetByCustomer")]
        public async Task<IActionResult> GetByCustomer()
        {
            var customerId = Request.Headers["X-Customer-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(customerId))
            {
                return BadRequest("Missing X-Customer-Id header.");
            }

            var summaries = await this.monthlySummaryService.GetMonthlySummariesByCustomer(customerId);

            return Ok(summaries);
        }

    }
}
