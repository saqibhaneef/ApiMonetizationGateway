using ApiMonetizationGateway.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Services;

public class UsageTrackingService : IUsageTrackingService
{
    private readonly GatewayDbContext _db;
    public UsageTrackingService(GatewayDbContext db)
    {
        _db = db;
    }

    public async Task SummarizePreviousMonthAsync()
    {
        var now = DateTime.UtcNow;
        var prev = now.AddMonths(-1);
        var monthStart = new DateTime(prev.Year, prev.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        var groups = await _db.ApiUsageLogs
            .Where(u => u.Timestamp >= monthStart && u.Timestamp < monthEnd)
            .GroupBy(u => u.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                Count = g.Count()
            }).ToListAsync();

        foreach (var g in groups)
        {
            var tier = await _db.Customers.Where(c => c.Id == g.CustomerId).Select(c => c.Tier!).FirstOrDefaultAsync();
            var price = tier != null ? tier.Price : 0;
            _db.MonthlySummaries.Add(new MonthlySummary
            {
                CustomerId = g.CustomerId,
                Year = monthStart.Year,
                Month = monthStart.Month,
                RequestCount = g.Count,
                Price = price
            });
        }

        await _db.SaveChangesAsync();
    }
}