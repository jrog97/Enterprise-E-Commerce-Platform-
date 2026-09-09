using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(
        EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);

        builder.Property(order => order.UserId)
            .IsRequired();

        builder.Property(order => order.Status)
            .IsRequired();

        builder.Property(order => order.Subtotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.Tax)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.Total)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            .IsRequired();

        builder.Property(order => order.UpdatedAt)
            .IsRequired();

        builder.HasOne(order => order.User)
            .WithMany(user => user.Orders)
            .HasForeignKey(order => order.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(order => order.PaymentStatus)
             .IsRequired();

        builder.Property(order => order.PaymentTransactionId)
            .HasMaxLength(200);
    }
}