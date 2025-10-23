namespace ApiMonetizationGateway;

public class Tier
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int MonthlyQuota { get; set; }
    public int RateLimitPerSecond { get; set; }
    public decimal Price { get; set; }
}