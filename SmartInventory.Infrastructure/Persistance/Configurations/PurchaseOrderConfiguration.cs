using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.OrderNo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(order => order.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(order => order.OrderDate)
            .IsRequired();

        builder.Property(order => order.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(order => order.OrderNo)
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");

        builder.HasIndex(order => order.SupplierId);

        builder.HasOne(order => order.Supplier)
            .WithMany(supplier => supplier.PurchaseOrders)
            .HasForeignKey(order => order.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(order => order.Items)
            .WithOne(item => item.PurchaseOrder)
            .HasForeignKey(item => item.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
