using ECommerce.Application.DTOs.Auth;
using System.Net;
using System.Net.Http.Json;
using ECommerce.UnitTests.Integration;

namespace ECommerce.UnitTests;

public class AuthIntegrationTests
    : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;

    public AuthIntegrationTests(
        IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ThenLogin_ShouldReturnJwt()
    {
        await _factory.StartAsync();

        var client = _factory.CreateClient();

        var email =
            $"test-{Guid.NewGuid()}@example.com";

        var registerRequest = new RegisterRequest
        {
            FirstName = "Integration",
            LastName = "Test",
            Email = email,
            Password = "TestPassword123!"
        };

        var registerResponse =
            await client.PostAsJsonAsync(
                "/api/auth/register",
                registerRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            registerResponse.StatusCode);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = "TestPassword123!"
        };

        var loginResponse =
            await client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest);

        Assert.Equal(
            HttpStatusCode.OK,
            loginResponse.StatusCode);

        var authResponse =
            await loginResponse.Content
                .ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(authResponse);

        Assert.False(
            string.IsNullOrWhiteSpace(
                authResponse.AccessToken));
    }

    [Fact]
    public async Task Customer_ShouldNotCreateProduct()
    {
        await _factory.StartAsync();

        var client =
            await TestAuthenticationHelper
                .CreateCustomerClientAsync(_factory);

        var request = new
        {
            name = "Unauthorized Product",
            description = "Should not be created",
            sku = $"TEST-{Guid.NewGuid():N}",
            price = 99.99,
            stockQuantity = 10,
            categoryId = Guid.NewGuid()
        };

        var response =
            await client.PostAsJsonAsync(
                "/api/products",
                request);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }
}
