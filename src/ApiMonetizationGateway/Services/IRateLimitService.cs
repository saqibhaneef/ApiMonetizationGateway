using System.Threading.Tasks;

namespace ApiMonetizationGateway.Services;

public interface IRateLimitService
{
    Task<RateLimitResult> ValidateRequestAsync(string customerId, string endpoint, string? userId = null);
}