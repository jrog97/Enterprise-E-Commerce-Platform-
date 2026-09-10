using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Constants;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;

    public ProductService(
        IProductRepository productRepository,
         ICacheService cacheService)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(
    CancellationToken cancellationToken = default)
{
    var cachedProducts =
        await _cacheService.GetAsync<List<ProductDto>>(
            CacheKeys.Products,
            cancellationToken);

    if (cachedProducts != null)
    {
        return cachedProducts;
    }

    var products =
        await _productRepository.GetAllAsync(
            cancellationToken);

    var productDtos =
        products.Select(MapToDto).ToList();

    await _cacheService.SetAsync(
        CacheKeys.Products,
        productDtos,
        TimeSpan.FromMinutes(5),
        cancellationToken);

    return productDtos;
}

    public async Task<ProductDto?> GetProductByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
{
    var cacheKey = CacheKeys.Product(id);

    var cachedProduct =
        await _cacheService.GetAsync<ProductDto>(
            cacheKey,
            cancellationToken);

    if (cachedProduct != null)
    {
        return cachedProduct;
    }

    var product =
        await _productRepository.GetByIdAsync(
            id,
            cancellationToken);

    if (product == null)
    {
        return null;
    }

    var productDto = MapToDto(product);

    await _cacheService.SetAsync(
        cacheKey,
        productDto,
        TimeSpan.FromMinutes(5),
        cancellationToken);

    return productDto;
}

    public async Task<ProductDto> CreateProductAsync(
    CreateProductRequest request,
    CancellationToken cancellationToken = default)
{
    if (request.Price <= 0)
    {
        throw new ArgumentException(
            "Price must be greater than zero.",
            nameof(request.Price));
    }

    var sku = request.SKU.Trim();

    if (await _productRepository.ExistsBySkuAsync(
            sku,
            cancellationToken))
    {
        throw new InvalidOperationException(
            $"A product with SKU '{sku}' already exists.");
    }

    if (!await _productRepository.CategoryExistsAsync(
        request.CategoryId,
        cancellationToken))
{
    throw new ArgumentException(
        "The specified category does not exist.");
}

    var product = new Product(
    request.Name.Trim(),
    request.Description.Trim(),
    sku,
    request.Price,
    request.StockQuantity,
    request.CategoryId);

    await _productRepository.AddAsync(
        product,
        cancellationToken);

    await _productRepository.SaveChangesAsync(
        cancellationToken);
    
    await _cacheService.RemoveAsync(
    CacheKeys.Products,
    cancellationToken);

    await _cacheService.RemoveAsync(
    CacheKeys.Product(product.Id),
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
        CategoryId = product.CategoryId,
       CategoryName = product.Category?.Name ?? string.Empty,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt
    };
}
}