using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations;

public class InboxEventConfiguration
    : IEntityTypeConfiguration<InboxEvent>
{
    public void Configure(
        EntityTypeBuilder<InboxEvent> builder)
    {
        builder.HasKey(x => x.EventId);

        builder.Property(x => x.EventId)
            .ValueGeneratedNever();

        builder.Property(x => x.EventType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.ProcessedAt)
            .IsRequired();

        builder.HasIndex(x => x.ProcessedAt);
    }
}