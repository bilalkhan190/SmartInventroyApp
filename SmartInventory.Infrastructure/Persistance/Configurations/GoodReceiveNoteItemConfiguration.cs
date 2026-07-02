using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Infrastructure.Persistance.Configurations;

public sealed class GoodReceiveNoteItemConfiguration : IEntityTypeConfiguration<GoodReceiveNoteItem>
{
    public void Configure(EntityTypeBuilder<GoodReceiveNoteItem> builder)
    {
        builder.ToTable("GoodReceiveNoteItems");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.OrderedQuantity)
            .IsRequired();

        builder.Property(item => item.ReceivedQuantity)
            .IsRequired();

        builder.Property(item => item.AcceptedQuantity)
            .IsRequired();

        builder.Property(item => item.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(item => item.BatchNo)
            .HasMaxLength(100);

        builder.Property(item => item.Remarks)
            .HasMaxLength(500);

        builder.HasIndex(item => new { item.GoodReceiveNoteId, item.ProductId })
            .IsUnique()
            .HasFilter("[DeletedAt] IS NULL");

        builder.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
