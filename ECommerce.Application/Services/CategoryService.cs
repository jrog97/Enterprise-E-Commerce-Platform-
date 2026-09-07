using ECommerce.Application.DTOs.Categories;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>>
        GetCategoriesAsync(
            CancellationToken cancellationToken = default)
    {
        var categories =
            await _categoryRepository.GetAllAsync(
                cancellationToken);

        return categories
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CategoryDto?>
        GetCategoryByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        var category =
            await _categoryRepository.GetByIdAsync(
                id,
                cancellationToken);

        return category is null
            ? null
            : MapToDto(category);
    }

    public async Task<CategoryDto>
        CreateCategoryAsync(
            CreateCategoryRequest request,
            CancellationToken cancellationToken = default)
    {
        var name = request.Name.Trim();

        if (await _categoryRepository.ExistsByNameAsync(
                name,
                cancellationToken))
        {
            throw new InvalidOperationException(
                $"A category named '{name}' already exists.");
        }

        var category = new Category(
            name,
            request.Description.Trim());

        await _categoryRepository.AddAsync(
            category,
            cancellationToken);

        await _categoryRepository.SaveChangesAsync(
            cancellationToken);

        return MapToDto(category);
    }

    private static CategoryDto MapToDto(
        Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}