using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.GoodReceiveNotes;

namespace SmartInventory.Application.Features.GoodReceiveNotes.Command.CreateGoodReceiveNote;

public sealed record CreateGoodReceiveNoteCommand(
    Guid PurchaseOrderId,
    DateTime? ReceiveDate,
    IReadOnlyList<GoodReceiveNoteItemInfo> Items) : IRequest<HandlerResult<GoodReceiveNoteDto>>;

public sealed record GoodReceiveNoteItemInfo(
    Guid ProductId,
    int ReceivedQuantity,
    int AcceptedQuantity,
    string? BatchNo = null,
    DateTime? ExpiredAt = null,
    string? Remarks = null);
