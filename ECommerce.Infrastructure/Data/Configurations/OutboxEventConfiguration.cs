using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class OutboxEventConfiguration
    : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(
        EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ProcessedAt);

        builder.Property(x => x.RetryCount)
            .IsRequired();

        builder.Property(x => x.Error)
            .HasMaxLength(2000);

        builder.HasIndex(x => x.ProcessedAt);

        builder.HasIndex(x => x.CreatedAt);
        builder.Property(x => x.NextAttemptAt)
        .IsRequired();
        builder.HasIndex(x => new
        {
            x.ProcessedAt,
            x.NextAttemptAt
        });
    }
}