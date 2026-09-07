using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(
        EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(product => product.Description)
            .HasMaxLength(2000);

        builder.Property(product => product.SKU)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(product => product.SKU)
            .IsUnique();

        builder.Property(product => product.Price)
            .HasPrecision(18, 2);

        builder.Property(product => product.StockQuantity)
            .IsRequired();

        builder.Property(product => product.IsActive)
            .IsRequired();

        builder.Property(product => product.CreatedAt)
            .IsRequired();

        builder.Property(product => product.UpdatedAt)
            .IsRequired();

        // Product → Category relationship
        builder.HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}