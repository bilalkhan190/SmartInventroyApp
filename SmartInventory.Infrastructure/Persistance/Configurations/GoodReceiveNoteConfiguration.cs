using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class GoodReceiveNoteConfiguration : IEntityTypeConfiguration<GoodReceiveNote>
{
    public void Configure(EntityTypeBuilder<GoodReceiveNote> builder)
    {
        builder.ToTable("GoodReceiveNotes");

        builder.HasKey(note => note.Id);

        builder.Property(note => note.GrnNo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(note => note.ReceiveDate)
            .IsRequired();

        builder.HasIndex(note => note.GrnNo)
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");

        builder.HasIndex(note => note.PurchaseOrderId);
        builder.HasIndex(note => note.SupplierId);

        builder.HasOne(note => note.Supplier)
            .WithMany()
            .HasForeignKey(note => note.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(note => note.PurchaseOrder)
            .WithMany()
            .HasForeignKey(note => note.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(note => note.GoodReceiveNoteItems)
            .WithOne(item => item.GoodReceiveNote)
            .HasForeignKey(item => item.GoodReceiveNoteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
