using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Quantity)
            .IsRequired();

        builder.Property(item => item.UnitAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasIndex(item => new { item.PurchaseOrderId, item.ProductId })
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");

        builder.HasOne(item => item.Product)
            .WithMany(product => product.PurchaseOrderItems)
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
