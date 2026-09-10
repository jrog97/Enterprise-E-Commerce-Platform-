namespace ECommerce.Application.Constants;

public static class CacheKeys
{
    public const string Products = "products:all";

    public static string Product(Guid id)
        => $"products:{id}";
}