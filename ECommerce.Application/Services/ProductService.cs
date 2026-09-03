using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(
        IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        var products = await _productRepository
            .GetAllAsync(cancellationToken);

        return products
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ProductDto?> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var product = await _productRepository
            .GetByIdAsync(id, cancellationToken);

        return product is null
            ? null
            : MapToDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var sku = request.SKU.Trim();

        if (await _productRepository.ExistsBySkuAsync(
                sku,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A product with SKU '{sku}' already exists.");
        }

        if (request.Price <= 0)
        {
            throw new ArgumentException(
                "Product price must be greater than zero.");
        }

        if (request.StockQuantity < 0)
        {
            throw new ArgumentException(
                "Stock quantity cannot be negative.");
        }

        var product = new Product(
            request.Name.Trim(),
            request.Description.Trim(),
            sku,
            request.Price,
            request.StockQuantity);

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(product);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            SKU = product.SKU,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}