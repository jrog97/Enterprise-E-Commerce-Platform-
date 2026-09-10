using System.Net;
using Xunit;

namespace ECommerce.UnitTests.Integration;

[Collection("Integration Tests")]
public class ProductIntegrationTests
{
    private readonly IntegrationTestFactory _factory;

    public ProductIntegrationTests(
        IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProducts_ShouldReturnSuccess()
    {
        await _factory.StartAsync();

        var client = _factory.CreateClient();

        var response =
            await client.GetAsync("/api/products");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
}