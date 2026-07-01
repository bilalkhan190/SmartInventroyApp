using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(supplier => supplier.Id);

        builder.Property(supplier => supplier.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(supplier => supplier.ContactName)
            .HasMaxLength(200);

        builder.Property(supplier => supplier.Email)
            .HasMaxLength(256);

        builder.Property(supplier => supplier.Phone)
            .HasMaxLength(50);

        builder.Property(supplier => supplier.Address)
            .HasMaxLength(500);

        builder.HasIndex(supplier => supplier.Name)
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");
    }
}
