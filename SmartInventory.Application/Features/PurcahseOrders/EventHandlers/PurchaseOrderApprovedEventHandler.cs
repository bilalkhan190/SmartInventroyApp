using MediatR;
using Microsoft.Extensions.Logging;
using SmartInventory.Application.Common.DomainEvents;
using SmartInventory.Domain.Events;

namespace SmartInventory.Application.Features.PurcahseOrders.EventHandlers;


public sealed class PurchaseOrderApprovedEventHandler
    : INotificationHandler<DomainEventNotification<PurchaseOrderApprovedEvent>>
{
    private readonly ILogger<PurchaseOrderApprovedEventHandler> _logger;

    public PurchaseOrderApprovedEventHandler(ILogger<PurchaseOrderApprovedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        DomainEventNotification<PurchaseOrderApprovedEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "Domain event: PO {OrderNo} ({PurchaseOrderId}) approved for supplier {SupplierId} at {OccurredOnUtc:u}",
            domainEvent.OrderNo,
            domainEvent.PurchaseOrderId,
            domainEvent.SupplierId,
            domainEvent.OccurredOnUtc);

        return Task.CompletedTask;
    }
}
