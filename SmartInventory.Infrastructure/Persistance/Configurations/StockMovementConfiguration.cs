using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");

        builder.HasKey(movement => movement.Id);

        builder.Property(movement => movement.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(movement => movement.Quantity)
            .IsRequired();

        builder.Property(movement => movement.Reason)
            .HasMaxLength(500);

        builder.HasIndex(movement => movement.ProductId);

        builder.HasIndex(movement => new { movement.ProductId, movement.CreatedAt });
    }
}
