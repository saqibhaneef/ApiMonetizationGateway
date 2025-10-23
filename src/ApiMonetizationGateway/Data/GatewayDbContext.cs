using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Data;

public class GatewayDbContext : DbContext
{
    public GatewayDbContext(DbContextOptions<GatewayDbContext> opts) : base(opts) { }

    public DbSet<Tier> Tiers => Set<Tier>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<ApiUsageLog> ApiUsageLogs => Set<ApiUsageLog>();
    public DbSet<MonthlySummary> MonthlySummaries => Set<MonthlySummary>();
}