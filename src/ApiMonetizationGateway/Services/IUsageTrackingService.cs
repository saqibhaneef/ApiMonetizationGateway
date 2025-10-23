using System.Threading.Tasks;

namespace ApiMonetizationGateway.Services;

public interface IUsageTrackingService
{
    Task SummarizePreviousMonthAsync();
}