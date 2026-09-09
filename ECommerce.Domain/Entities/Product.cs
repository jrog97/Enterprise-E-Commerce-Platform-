namespace ECommerce.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string SKU { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public int Version { get; private set; }

    private Product()
    {
    }

    public Product(
    string name,
    string description,
    string sku,
    decimal price,
    int stockQuantity,
    Guid categoryId)
{
    Id = Guid.NewGuid();
    Name = name;
    Description = description;
    SKU = sku;
    Price = price;
    StockQuantity = stockQuantity;
    CategoryId = categoryId;
    IsActive = true;
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
}

    public void Update(
        string name,
        string description,
        decimal price)
    {
        Name = name;
        Description = description;
        Price = price;

        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        StockQuantity += quantity;

        UpdatedAt = DateTime.UtcNow;
    }

    public bool RemoveStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        if (StockQuantity < quantity)
        {
            return false;
        }
    
        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;

        return true;
    }
}