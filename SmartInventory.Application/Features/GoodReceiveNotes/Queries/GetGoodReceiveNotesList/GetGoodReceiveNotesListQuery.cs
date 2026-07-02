using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.GoodReceiveNotes.Queries.GetGoodReceiveNotesList;

public sealed record GetGoodReceiveNotesListQuery
    : IRequest<HandlerResult<IReadOnlyList<GoodReceiveNoteListDto>>>;
