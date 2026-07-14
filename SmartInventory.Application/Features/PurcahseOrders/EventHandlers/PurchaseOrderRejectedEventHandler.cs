using MediatR;
using Microsoft.Extensions.Logging;
using SmartInventory.Application.Common.DomainEvents;
using SmartInventory.Domain.Events;

namespace SmartInventory.Application.Features.PurcahseOrders.EventHandlers;

public sealed class PurchaseOrderRejectedEventHandler
    : INotificationHandler<DomainEventNotification<PurchaseOrderRejectedEvent>>
{
    private readonly ILogger<PurchaseOrderRejectedEventHandler> _logger;

    public PurchaseOrderRejectedEventHandler(ILogger<PurchaseOrderRejectedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        DomainEventNotification<PurchaseOrderRejectedEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "Domain event: PO {OrderNo} ({PurchaseOrderId}) rejected. Reason: {Reason}",
            domainEvent.OrderNo,
            domainEvent.PurchaseOrderId,
            domainEvent.Reason ?? "(none)");

        return Task.CompletedTask;
    }
}
