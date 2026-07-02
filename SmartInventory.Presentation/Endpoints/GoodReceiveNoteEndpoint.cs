using MediatR;
using SmartInventory.Application.Features.GoodReceiveNotes.Command.CreateGoodReceiveNote;
using SmartInventory.Application.Features.GoodReceiveNotes.Queries.GetGoodReceiveNotesList;

namespace SmartInventory.Presentation.Endpoints;

public sealed class GoodReceiveNoteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/good-receive-notes")
            .WithTags("Good Receive Notes");

        group.MapGet("/", GetGoodReceiveNotesList)
            .WithName("GetGoodReceiveNotes")
            .WithSummary("Get all good receive notes");

        group.MapPost("/", CreateGoodReceiveNote)
            .WithName("CreateGoodReceiveNote")
            .WithSummary("Create a new good receive note");
    }

    private static async Task<IResult> GetGoodReceiveNotesList(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetGoodReceiveNotesListQuery(), cancellationToken);
        return result.ToResult(value => Results.Ok(value));
    }

    private static async Task<IResult> CreateGoodReceiveNote(
        CreateGoodReceiveNoteRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateGoodReceiveNoteCommand(
                request.PurchaseOrderId,
                request.ReceiveDate,
                request.Items),
            cancellationToken);

        return result.ToResult(
            value => Results.Created($"/api/good-receive-notes/{value.Id}", value));
    }
}

public sealed record CreateGoodReceiveNoteRequest(
    Guid PurchaseOrderId,
    DateTime? ReceiveDate,
    IReadOnlyList<GoodReceiveNoteItemInfo> Items);
