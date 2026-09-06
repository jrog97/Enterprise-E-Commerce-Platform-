using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
namespace ECommerce.Infrastructure.Authentication;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        IServiceProvider services)
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles =
        [
            "Customer",
            "Admin",
            "Employee"
        ];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(
                    new IdentityRole<Guid>(role));
            }
        }
    }
}