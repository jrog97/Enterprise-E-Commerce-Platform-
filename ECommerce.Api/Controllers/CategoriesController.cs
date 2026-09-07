using ECommerce.Application.DTOs.Categories;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(
        ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<
        ActionResult<IReadOnlyList<CategoryDto>>>
        GetCategories(
            CancellationToken cancellationToken)
    {
        var categories =
            await _categoryService.GetCategoriesAsync(
                cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>>
        GetCategory(
            Guid id,
            CancellationToken cancellationToken)
    {
        var category =
            await _categoryService.GetCategoryByIdAsync(
                id,
                cancellationToken);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(category);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>>
        CreateCategory(
            CreateCategoryRequest request,
            CancellationToken cancellationToken)
    {
        var category =
            await _categoryService.CreateCategoryAsync(
                request,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.Id },
            category);
    }
}