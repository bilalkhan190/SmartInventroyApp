using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(user => user.NormalizedEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(user => user.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.HasIndex(user => user.NormalizedEmail)
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");
    }
}
