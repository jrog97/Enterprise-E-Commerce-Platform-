using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class ShoppingCartConfiguration
    : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(
        EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.HasKey(cart => cart.Id);

        builder.Property(cart => cart.UserId)
            .IsRequired();

        builder.Property(cart => cart.CreatedAt)
            .IsRequired();

        builder.Property(cart => cart.UpdatedAt)
            .IsRequired();

        builder.HasIndex(cart => cart.UserId)
            .IsUnique();

        builder.HasOne(cart => cart.User)
            .WithOne(user => user.Cart)
            .HasForeignKey<ShoppingCart>(
                cart => cart.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cart => cart.Items)
            .WithOne(item => item.Cart)
            .HasForeignKey(item => item.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}