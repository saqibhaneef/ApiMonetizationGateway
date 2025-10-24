
using ApiMonetizationGateway.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Services
{
    public class MonthlySummaryService : IMonthlySummaryService
    {
        private readonly GatewayDbContext dbGatewayDbContext;

        public MonthlySummaryService(GatewayDbContext gatewayDbContext)
        {
            this.dbGatewayDbContext = gatewayDbContext;
        }

        public async Task<List<MonthlySummary>> GetMonthlySummaries()
        {
            return await this.dbGatewayDbContext.MonthlySummaries.ToListAsync();
        }

        public async Task<List<MonthlySummary>> GetMonthlySummariesByCustomer(string customerId)
        {
            return await this.dbGatewayDbContext.MonthlySummaries.Where(x => x.CustomerId == customerId).ToListAsync();
        }
    }
}
