using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ApiMonetizationGateway.Services;

public class RateLimitService : IRateLimitService
{
    private readonly IMemoryCache cache;
    private readonly Data.GatewayDbContext db;

    public RateLimitService(IMemoryCache cache, Data.GatewayDbContext db)
    {
        this.cache = cache;
        this.db = db;
    }

    public async Task<RateLimitResult> ValidateRequestAsync(string customerId, string endpoint, string? userId = null)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return new RateLimitResult(false, "Missing X-Customer-Id header");

        var customer = await this.db.Customers.Include(c => c.Tier).FirstOrDefaultAsync(c => c.Id == customerId);
        if (customer is null || customer.Tier is null)
            return new RateLimitResult(false, "Customer or tier not found");

        var tier = customer.Tier;
        var now = DateTime.UtcNow;

        // Per-second rate limiting using memory cache key with 1-second sliding window
        var key = $"rl:{customerId}:{endpoint}:{now:yyyyMMddHHmmss}";
        var current = this.cache.GetOrCreate<int>(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
            return 0;
        });

        if (current >= tier.RateLimitPerSecond)
            return new RateLimitResult(false, "Rate limit exceeded");

        this.cache.Set(key, current + 1, TimeSpan.FromSeconds(1));

        // Monthly quota check in DB (UTC month)
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthlyCount = await this.db.ApiUsageLogs.CountAsync(u => u.CustomerId == customerId && u.Timestamp >= monthStart);
        if (monthlyCount >= tier.MonthlyQuota)
            return new RateLimitResult(false, "Monthly quota exceeded");

        // Log usage
        this.db.ApiUsageLogs.Add(new ApiUsageLog
        {
            CustomerId = customerId,
            UserId = userId,
            Endpoint = endpoint,
            Timestamp = now
        });
        await this.db.SaveChangesAsync();

        return new RateLimitResult(true, "Allowed");
    }
}