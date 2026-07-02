using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");

        builder.HasKey(inventory => inventory.Id);

        builder.Property(inventory => inventory.CurrentStockQuantity)
            .IsRequired();

        builder.HasIndex(inventory => inventory.ProductId)
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");

        builder.HasOne(inventory => inventory.Product)
            .WithOne(product => product.ProductInventory)
            .HasForeignKey<Inventory>(inventory => inventory.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
