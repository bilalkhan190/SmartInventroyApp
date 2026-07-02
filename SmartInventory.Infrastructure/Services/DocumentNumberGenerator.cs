using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Contracts;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Domain.Entities;
using SmartInventory.Domain.Enums;

namespace SmartInventory.Infrastructure.Services;

public sealed class DocumentNumberGenerator : IDocumentNumberGenerator
{
    private readonly IApplicationDbContext _context;

    public DocumentNumberGenerator(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> NextPurchaseOrderNoAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;

        var sequence = await _context.DocumentSequences
            .FirstOrDefaultAsync(
                entry => entry.DocumentType == DocumentTypes.PurchaseOrder && entry.Year == year,
                cancellationToken);

        if (sequence is null)
        {
            sequence = new DocumentSequences
            {
                DocumentType = DocumentTypes.PurchaseOrder,
                Year = year,
                Prefix = "PO",
                LastNumber = 1
            };
            _context.DocumentSequences.Add(sequence);
        }
        else
        {
            sequence.LastNumber++;
        }

        return $"{sequence.Prefix}-{year}-{sequence.LastNumber:D4}";
    }

    public async Task<string> NextGrnNoAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;

        var sequence = await _context.DocumentSequences
            .FirstOrDefaultAsync(
                entry => entry.DocumentType == DocumentTypes.GoodsReceiptNote && entry.Year == year,
                cancellationToken);

        if (sequence is null)
        {
            sequence = new DocumentSequences
            {
                DocumentType = DocumentTypes.GoodsReceiptNote,
                Year = year,
                Prefix = "GRN",
                LastNumber = 1
            };
            _context.DocumentSequences.Add(sequence);
        }
        else
        {
            sequence.LastNumber++;
        }

        return $"{sequence.Prefix}-{year}-{sequence.LastNumber:D4}";
    }
}
