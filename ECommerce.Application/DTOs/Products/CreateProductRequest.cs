namespace ECommerce.Application.DTOs.Products;

public class CreateProductRequest
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string SKU { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
    public int StockQuantity { get; init; }
}