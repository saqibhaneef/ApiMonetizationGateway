
using ApiMonetizationGateway.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Services
{
    public class ApiUsageLogsService : IApiUsageLogsService
    {
        private readonly GatewayDbContext dbGatewayDbContext;

        public ApiUsageLogsService(GatewayDbContext dbGatewayDbContex)
        {
                this.dbGatewayDbContext = dbGatewayDbContex;
        }
        
        public async Task<List<ApiUsageLog>> GetUsageLogs()
        {
            return await this.dbGatewayDbContext.ApiUsageLogs.ToListAsync();
        }

        public async Task<List<ApiUsageLog>> GetUsageLogsByCustomer(string customerId)
        {
            return await this.dbGatewayDbContext.ApiUsageLogs.Where(x=>x.CustomerId == customerId).ToListAsync();
        }
    }
}
