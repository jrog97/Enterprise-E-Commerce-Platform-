using ECommerce.Infrastructure.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace ECommerce.UnitTests.Integration;

public class IntegrationTestFactory
    : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder("postgres:17")
            .WithDatabase("ecommerce_test")
            .WithUsername("ecommerce_test")
            .WithPassword("ecommerce_test_password")
            .Build();

    private readonly RedisContainer _redis =
        new RedisBuilder("redis:7.0")
            .Build();

    private bool _started;

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        // Override Redis connection for integration tests
        builder.ConfigureAppConfiguration(
            (_, configuration) =>
            {
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:Redis"] =
                            _redis.GetConnectionString()
                    });
            });

        // Override PostgreSQL connection for integration tests
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<
                DbContextOptions<ECommerceDbContext>>();

            services.AddDbContext<ECommerceDbContext>(
                options =>
                {
                    options.UseNpgsql(
                        _postgres.GetConnectionString());
                });
        });
    }

    public async Task StartAsync()
    {
        if (_started)
        {
            return;
        }

        await _postgres.StartAsync();
        await _redis.StartAsync();

        // Starting Services starts the ASP.NET application.
        // Program.cs will apply migrations and seed Identity.
        _ = Services;

        _started = true;
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<ECommerceDbContext>();

        await dbContext.Database.EnsureDeletedAsync();

        await dbContext.Database.MigrateAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await _redis.DisposeAsync();
        await _postgres.DisposeAsync();

        await base.DisposeAsync();
    }
}