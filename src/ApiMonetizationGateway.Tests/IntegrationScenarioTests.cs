using ApiMonetizationGateway.Controllers;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

public class IntegrationScenarioTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string url = $"/api/{nameof(MonetizationController).Replace("Controller", "")}/{nameof(MonetizationController.PaidApi)}";
    public IntegrationScenarioTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FreeTier_Should_Return_429_After_Exceeding_RateLimit()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Customer-Id", "cust-free-1");

        // Free tier allows only 2 requests per second
        
        var response1 = await client.GetAsync(url);
        var response2 = await client.GetAsync(url);
        var response3 = await client.GetAsync(url);
        var response4 = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        Assert.Equal(HttpStatusCode.TooManyRequests, response4.StatusCode);
    }

    [Fact]
    public async Task ProTier_Should_Allow_Three_Requests_Within_Same_Second()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Customer-Id", "cust-pro-1");

        // Pro tier allows up to 10 per second — all should succeed
        var response1 = await client.GetAsync(url);
        var response2 = await client.GetAsync(url);
        var response3 = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
    }
}
