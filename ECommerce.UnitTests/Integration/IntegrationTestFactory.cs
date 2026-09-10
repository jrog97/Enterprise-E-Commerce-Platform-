
using ECommerce.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

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

    private bool _started;

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
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
        await _postgres.DisposeAsync();

        await base.DisposeAsync();
    }
}
