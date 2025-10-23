namespace ApiMonetizationGateway;

public class Customer
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public int TierId { get; set; }
    public Tier? Tier { get; set; }
}