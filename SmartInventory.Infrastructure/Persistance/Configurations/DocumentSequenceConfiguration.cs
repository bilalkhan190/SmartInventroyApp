using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

internal sealed class DocumentSequenceConfiguration : IEntityTypeConfiguration<DocumentSequences>
{
    public void Configure(EntityTypeBuilder<DocumentSequences> builder)
    {
        builder.ToTable("DocumentSequences");

        builder.HasKey(sequence => sequence.DocumentSequenceId);

        builder.Property(sequence => sequence.Prefix)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(sequence => new { sequence.DocumentType, sequence.Year })
            .IsUnique();
    }
}
