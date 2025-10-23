using ApiMonetizationGateway;
using ApiMonetizationGateway.Data;
using ApiMonetizationGateway.Middleware;
using ApiMonetizationGateway.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Config and Db
builder.Services.Configure<TierConfig>(builder.Configuration.GetSection("Tiers"));
builder.Services.AddMemoryCache();
builder.Services.AddDbContext<GatewayDbContext>(opts =>
    opts.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ??
                   "Data Source=gateway.db"));


// Services
builder.Services.AddScoped<IRateLimitService, RateLimitService>();
builder.Services.AddScoped<IUsageTrackingService, UsageTrackingService>();
builder.Services.AddHostedService<MonthlySummaryWorker>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseMiddleware<RateLimitMiddleware>();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();
    db.Database.EnsureCreated();
    // seed tiers/customers
    if (!db.Tiers.Any())
    {
        db.Tiers.AddRange(new Tier { Name = "Free", MonthlyQuota = 100, RateLimitPerSecond = 2, Price = 0 },
                          new Tier { Name = "Pro", MonthlyQuota = 100000, RateLimitPerSecond = 10, Price = 50 });
        db.SaveChanges();
    }
    if (!db.Customers.Any())
    {
        var free = db.Tiers.First(t => t.Name=="Free");
        var pro = db.Tiers.First(t => t.Name=="Pro");
        db.Customers.AddRange(new Customer { Id = "cust-free-1", Name = "Free Customer", TierId = free.Id },
                              new Customer { Id = "cust-pro-1", Name = "Pro Customer", TierId = pro.Id });
        db.SaveChanges();
    }
}

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Startup failed:");
    Console.WriteLine(ex);
}