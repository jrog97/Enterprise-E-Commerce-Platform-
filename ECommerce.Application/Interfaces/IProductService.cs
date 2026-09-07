using ECommerce.Application.DTOs.Products;

namespace ECommerce.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<ProductDto> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);
}