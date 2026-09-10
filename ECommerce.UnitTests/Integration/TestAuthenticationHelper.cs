using System.Net.Http.Headers;
using System.Net.Http.Json;
using ECommerce.Application.DTOs.Auth;

namespace ECommerce.UnitTests.Integration;

public static class TestAuthenticationHelper
{
    public static async Task<HttpClient> CreateCustomerClientAsync(
        IntegrationTestFactory factory)
    {
        var client = factory.CreateClient();

        var email =
            $"customer-{Guid.NewGuid()}@example.com";

        var registerRequest = new RegisterRequest
        {
            FirstName = "Test",
            LastName = "Customer",
            Email = email,
            Password = "TestPassword123!"
        };

        var registerResponse =
            await client.PostAsJsonAsync(
                "/api/auth/register",
                registerRequest);

        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = "TestPassword123!"
        };

        var loginResponse =
            await client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest);

        loginResponse.EnsureSuccessStatusCode();

        var authResponse =
            await loginResponse.Content
                .ReadFromJsonAsync<AuthResponse>();

        if (authResponse == null)
        {
            throw new InvalidOperationException(
                "Authentication response was null.");
        }

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                authResponse.AccessToken);

        return client;
    }
}