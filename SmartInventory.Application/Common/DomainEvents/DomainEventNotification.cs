using MediatR;
using SmartInventory.Domain.Common;

namespace SmartInventory.Application.Common.DomainEvents;

/// <summary>
/// Bridges pure Domain events → MediatR so Application handlers can react
/// without Domain depending on MediatR.
/// </summary>
public sealed class DomainEventNotification<TEvent> : INotification
    where TEvent : IDomainEvent
{
    public DomainEventNotification(TEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public TEvent DomainEvent { get; }
}
