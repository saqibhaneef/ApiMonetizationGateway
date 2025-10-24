
using ApiMonetizationGateway.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly GatewayDbContext dbGatewayDbContext;

        public CustomerService(GatewayDbContext gatewayDbContext)
        {
            this.dbGatewayDbContext = gatewayDbContext;
        }

        public async Task<List<Customer>> GetCustomers()
        {
            var customers =  await this.dbGatewayDbContext.Customers.Include(x=>x.Tier).ToListAsync();

            return customers;
        }
    }
}
