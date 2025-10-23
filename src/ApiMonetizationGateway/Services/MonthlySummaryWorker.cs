using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiMonetizationGateway.Services;

public class MonthlySummaryWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MonthlySummaryWorker> _logger;

    public MonthlySummaryWorker(IServiceScopeFactory scopeFactory, ILogger<MonthlySummaryWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Runs continuously while the application is active
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;

                // Run on the 1st of every month
                if (now.Day == 1)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var svc = scope.ServiceProvider.GetRequiredService<IUsageTrackingService>();

                    await svc.SummarizePreviousMonthAsync();
                    _logger.LogInformation("Monthly summary executed successfully at {Time}", DateTime.UtcNow);

                    // Avoid re-running the same day
                    await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in MonthlySummaryWorker");
            }

            // Check again every hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
