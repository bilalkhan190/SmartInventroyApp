using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(category => category.Id);

        builder.Property(category => category.CategoryName)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(category => category.CategoryName)
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");
    }
}
