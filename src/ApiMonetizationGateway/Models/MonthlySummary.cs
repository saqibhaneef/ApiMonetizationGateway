using System;

namespace ApiMonetizationGateway;

public class MonthlySummary
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = default!;
    public int Year { get; set; }
    public int Month { get; set; }
    public int RequestCount { get; set; }
    public decimal Price { get; set; }
}