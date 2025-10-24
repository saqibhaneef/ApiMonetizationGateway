using ApiMonetizationGateway;
using ApiMonetizationGateway.Data;
using ApiMonetizationGateway.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

public class RateLimitServiceTests
{
    private RateLimitService CreateService(out GatewayDbContext db)
    {
        var options = new DbContextOptionsBuilder<GatewayDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        db = new GatewayDbContext(options);

        var tier = new Tier { Id = 1, Name = "Free", MonthlyQuota = 3, RateLimitPerSecond = 2, Price = 0 };
        db.Tiers.Add(tier);
        db.Customers.Add(new Customer { Id = "cust1", Name = "Test Cust", TierId = tier.Id });
        db.SaveChanges();

        var cache = new MemoryCache(new MemoryCacheOptions());
        return new RateLimitService(cache, db);
    }

    [Fact]
    public async Task Allows_Request_Within_Limit()
    {
        var service = CreateService(out var db);
        var result = await service.ValidateRequestAsync("cust1", "/api/test");
        Assert.True(result.Allowed);
    }

    [Fact]
    public async Task Blocks_Over_PerSecond_Limit()
    {
        var service = CreateService(out var db);
        await service.ValidateRequestAsync("cust1", "/api/test");
        await service.ValidateRequestAsync("cust1", "/api/test");
        var result = await service.ValidateRequestAsync("cust1", "/api/test");
        Assert.False(result.Allowed);
        Assert.Contains("Rate limit exceeded", result.Message);
    }

    [Fact]
    public async Task Blocks_When_Monthly_Quota_Reached()
    {
        var service = CreateService(out var db);
        // Fill up logs manually
        db.ApiUsageLogs.AddRange(
            new ApiUsageLog { CustomerId = "cust1", Endpoint = "/api/test", Timestamp = DateTime.UtcNow },
            new ApiUsageLog { CustomerId = "cust1", Endpoint = "/api/test", Timestamp = DateTime.UtcNow },
            new ApiUsageLog { CustomerId = "cust1", Endpoint = "/api/test", Timestamp = DateTime.UtcNow }
        );
        db.SaveChanges();

        var result = await service.ValidateRequestAsync("cust1", "/api/test");
        Assert.False(result.Allowed);
        Assert.Contains("Monthly quota exceeded", result.Message);
    }
}
