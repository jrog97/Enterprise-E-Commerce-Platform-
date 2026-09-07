namespace ECommerce.Application.DTOs.Categories;

public class CreateCategoryRequest
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}