using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(
        IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts(
        CancellationToken cancellationToken)
    {
        var products = await _productService
            .GetProductsAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _productService
            .GetProductByIdAsync(
                id,
                cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

[HttpPost]
public async Task<ActionResult<ProductDto>> CreateProduct(
    CreateProductRequest request,
    CancellationToken cancellationToken)
{
    var product = await _productService
        .CreateProductAsync(
            request,
            cancellationToken);

    return CreatedAtAction(
        nameof(GetProduct),
        new { id = product.Id },
        product);
}
}