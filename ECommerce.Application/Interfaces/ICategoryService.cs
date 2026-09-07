using ECommerce.Application.DTOs.Categories;

namespace ECommerce.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<CategoryDto?> GetCategoryByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<CategoryDto> CreateCategoryAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default);
}