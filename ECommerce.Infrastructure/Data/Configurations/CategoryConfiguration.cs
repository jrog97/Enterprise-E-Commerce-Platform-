using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class CategoryConfiguration
    : IEntityTypeConfiguration<Category>
{
    public void Configure(
        EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(category => category.Name)
            .IsUnique();

        builder.Property(category => category.Description)
            .HasMaxLength(1000);

        builder.Property(category => category.IsActive)
            .IsRequired();

        builder.Property(category => category.CreatedAt)
            .IsRequired();

        builder.Property(category => category.UpdatedAt)
            .IsRequired();

        builder.HasMany(category => category.Products)
            .WithOne(product => product.Category)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}