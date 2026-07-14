using MediatR;
using Microsoft.Extensions.Logging;
using SmartInventory.Application.Common.DomainEvents;
using SmartInventory.Domain.Events;

namespace SmartInventory.Application.Features.PurcahseOrders.EventHandlers;

public sealed class PurchaseOrderSubmittedForApprovalEventHandler
    : INotificationHandler<DomainEventNotification<PurchaseOrderSubmittedForApprovalEvent>>
{
    private readonly ILogger<PurchaseOrderSubmittedForApprovalEventHandler> _logger;

    public PurchaseOrderSubmittedForApprovalEventHandler(
        ILogger<PurchaseOrderSubmittedForApprovalEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(
        DomainEventNotification<PurchaseOrderSubmittedForApprovalEvent> notification,
        CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "Domain event: PO {OrderNo} ({PurchaseOrderId}) submitted for approval",
            domainEvent.OrderNo,
            domainEvent.PurchaseOrderId);

        return Task.CompletedTask;
    }
}
