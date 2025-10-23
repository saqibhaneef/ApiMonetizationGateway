using System;

namespace ApiMonetizationGateway;

public class ApiUsageLog
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = default!;
    public string? UserId { get; set; }
    public string Endpoint { get; set; } = default!;
    public DateTime Timestamp { get; set; }
}