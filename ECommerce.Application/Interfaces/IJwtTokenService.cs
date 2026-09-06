using ECommerce.Domain.Entities;
namespace ECommerce.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(
        ApplicationUser user,
        IEnumerable<string> roles,
        out DateTime expiresAt);
}