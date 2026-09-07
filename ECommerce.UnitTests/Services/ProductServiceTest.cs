using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using Moq;

namespace ECommerce.UnitTests.Services;

public class ProductServiceTests
{
private readonly Mock<IProductRepository> _repositoryMock;
private readonly ProductService _productService;

public ProductServiceTests()
{
    _repositoryMock = new Mock<IProductRepository>();

    _productService = new ProductService(
        _repositoryMock.Object);
}

[Fact]
public async Task GetProductsAsync_ReturnsProducts()
{
    // Arrange
    var categoryId = Guid.NewGuid();

    var products = new List<Product>
    {
        new Product(
            "MacBook Pro",
            "Apple laptop",
            "APPLE-MBP-001",
            1999.99m,
            25,
            categoryId),

        new Product(
            "iPhone",
            "Apple smartphone",
            "APPLE-IP-001",
            999.99m,
            50,
            categoryId)
    };

    _repositoryMock
        .Setup(repository =>
            repository.GetAllAsync(
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(products);

    // Act
    var result = await _productService
        .GetProductsAsync();

    // Assert
    Assert.Equal(2, result.Count);

    Assert.Equal(
        "MacBook Pro",
        result[0].Name);

    Assert.Equal(
        categoryId,
        result[0].CategoryId);

    Assert.Equal(
        "iPhone",
        result[1].Name);

    Assert.Equal(
        categoryId,
        result[1].CategoryId);
}

[Fact]
public async Task GetProductByIdAsync_ProductExists_ReturnsProduct()
{
    // Arrange
    var categoryId = Guid.NewGuid();

    var product = new Product(
        "MacBook Pro",
        "Apple laptop",
        "APPLE-MBP-001",
        1999.99m,
        25,
        categoryId);

    var productId = product.Id;

    _repositoryMock
        .Setup(repository =>
            repository.GetByIdAsync(
                productId,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(product);

    // Act
    var result = await _productService
        .GetProductByIdAsync(productId);

    // Assert
    Assert.NotNull(result);

    Assert.Equal(
        productId,
        result.Id);

    Assert.Equal(
        "MacBook Pro",
        result.Name);

    Assert.Equal(
        1999.99m,
        result.Price);

    Assert.Equal(
        categoryId,
        result.CategoryId);
}

[Fact]
public async Task GetProductByIdAsync_ProductDoesNotExist_ReturnsNull()
{
    // Arrange
    var productId = Guid.NewGuid();

    _repositoryMock
        .Setup(repository =>
            repository.GetByIdAsync(
                productId,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync((Product?)null);

    // Act
    var result = await _productService
        .GetProductByIdAsync(productId);

    // Assert
    Assert.Null(result);
}

[Fact]
public async Task CreateProductAsync_ValidRequest_CreatesProduct()
{
    // Arrange
    var categoryId = Guid.NewGuid();

    var request = new CreateProductRequest
    {
        Name = "MacBook Pro",
        Description = "Apple laptop",
        SKU = "APPLE-MBP-001",
        Price = 1999.99m,
        StockQuantity = 25,
        CategoryId = categoryId
    };

    _repositoryMock
        .Setup(repository =>
            repository.ExistsBySkuAsync(
                request.SKU,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    _repositoryMock
        .Setup(repository =>
            repository.CategoryExistsAsync(
                categoryId,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    // Act
    var result = await _productService
        .CreateProductAsync(request);

    // Assert
    Assert.NotNull(result);

    Assert.Equal(
        "MacBook Pro",
        result.Name);

    Assert.Equal(
        "APPLE-MBP-001",
        result.SKU);

    Assert.Equal(
        1999.99m,
        result.Price);

    Assert.Equal(
        25,
        result.StockQuantity);

    Assert.Equal(
        categoryId,
        result.CategoryId);

    _repositoryMock.Verify(
        repository => repository.AddAsync(
            It.IsAny<Product>(),
            It.IsAny<CancellationToken>()),
        Times.Once);

    _repositoryMock.Verify(
        repository => repository.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
        Times.Once);
}

[Fact]
public async Task CreateProductAsync_DuplicateSku_ThrowsException()
{
    // Arrange
    var categoryId = Guid.NewGuid();

    var request = new CreateProductRequest
    {
        Name = "MacBook Pro",
        Description = "Apple laptop",
        SKU = "APPLE-MBP-001",
        Price = 1999.99m,
        StockQuantity = 25,
        CategoryId = categoryId
    };

    _repositoryMock
        .Setup(repository =>
            repository.ExistsBySkuAsync(
                request.SKU,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<
        InvalidOperationException>(
        () => _productService
            .CreateProductAsync(request));

    Assert.Contains(
        "already exists",
        exception.Message);

    _repositoryMock.Verify(
        repository => repository.AddAsync(
            It.IsAny<Product>(),
            It.IsAny<CancellationToken>()),
        Times.Never);

    _repositoryMock.Verify(
        repository => repository.SaveChangesAsync(
            It.IsAny<CancellationToken>()),
        Times.Never);
}

[Fact]
public async Task CreateProductAsync_InvalidPrice_ThrowsException()
{
    // Arrange
    var categoryId = Guid.NewGuid();

    var request = new CreateProductRequest
    {
        Name = "Invalid Product",
        Description = "Invalid product",
        SKU = "INVALID-001",
        Price = -10m,
        StockQuantity = 10,
        CategoryId = categoryId
    };

    _repositoryMock
        .Setup(repository =>
            repository.ExistsBySkuAsync(
                request.SKU,
                It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<
        ArgumentException>(
        () => _productService
            .CreateProductAsync(request));

    Assert.Contains(
        "price",
        exception.Message,
        StringComparison.OrdinalIgnoreCase);
}

}
