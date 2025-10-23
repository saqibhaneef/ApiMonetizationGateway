namespace ApiMonetizationGateway.Services;

public record RateLimitResult(bool Allowed, string Message);