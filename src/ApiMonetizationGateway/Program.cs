using ApiMonetizationGateway;
using ApiMonetizationGateway.Data;
using ApiMonetizationGateway.Middleware;
using ApiMonetizationGateway.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// ✅ Swagger config with custom headers
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Monetization Gateway", Version = "v1" });

    // Add custom header definitions
    c.AddSecurityDefinition("X-Customer-Id", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Customer-Id",
        Type = SecuritySchemeType.ApiKey,
        Description = "Customer ID header (required)"
    });

    c.AddSecurityDefinition("X-User-Id", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-User-Id",
        Type = SecuritySchemeType.ApiKey,
        Description = "User ID header (optional)"
    });

    // Apply them globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-Customer-Id"
                }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-User-Id"
                }
            },
            Array.Empty<string>()
        }
    });
});

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
if (builder.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}




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