using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(product => product.Sku)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(product => product.Description)
            .HasMaxLength(1000);

        builder.Property(product => product.Quantity)
            .IsRequired();

        builder.Property(product => product.ReorderLevel)
            .IsRequired();

        builder.HasIndex(product => product.Sku)
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");

        builder.HasOne(product => product.Category)
            .WithMany()
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(product => product.StockMovements)
            .WithOne(movement => movement.Product)
            .HasForeignKey(movement => movement.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
