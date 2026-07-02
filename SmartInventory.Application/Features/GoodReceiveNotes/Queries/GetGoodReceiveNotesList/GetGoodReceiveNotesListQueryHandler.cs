using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;

namespace SmartInventory.Application.Features.GoodReceiveNotes.Queries.GetGoodReceiveNotesList;

public sealed class GetGoodReceiveNotesListQueryHandler
    : IRequestHandler<GetGoodReceiveNotesListQuery, HandlerResult<IReadOnlyList<GoodReceiveNoteListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetGoodReceiveNotesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<IReadOnlyList<GoodReceiveNoteListDto>>> Handle(
        GetGoodReceiveNotesListQuery request,
        CancellationToken cancellationToken)
    {
        var notes = await _context.GoodReceiveNotes
            .AsNoTracking()
            .Where(note => note.DeletedAt == null)
            .OrderByDescending(note => note.ReceiveDate)
            .Select(note => new GoodReceiveNoteListDto
            {
                Id = note.Id,
                GrnNo = note.GrnNo,
                SupplierName = note.Supplier.Name,
                PurchaseOrderNo = note.PurchaseOrder.OrderNo,
                ReceiveDate = note.ReceiveDate,
                ItemCount = note.GoodReceiveNoteItems.Count(item => item.DeletedAt == null),
                TotalAcceptedQuantity = note.GoodReceiveNoteItems
                    .Where(item => item.DeletedAt == null)
                    .Sum(item => item.AcceptedQuantity)
            })
            .ToListAsync(cancellationToken);

        return HandlerResult<IReadOnlyList<GoodReceiveNoteListDto>>.Success(notes);
    }
}
