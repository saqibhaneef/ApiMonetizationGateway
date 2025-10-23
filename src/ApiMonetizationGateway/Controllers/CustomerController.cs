using ApiMonetizationGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiMonetizationGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService customerService;
        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> Customers()
        {
            var customers = await customerService.GetCustomers();
            return Ok(customers);
        }
    }
}
