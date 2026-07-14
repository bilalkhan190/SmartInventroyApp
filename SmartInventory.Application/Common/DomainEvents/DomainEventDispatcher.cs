using MediatR;
using SmartInventory.Application.Contracts;
using SmartInventory.Domain.Common;

namespace SmartInventory.Application.Common.DomainEvents;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var notification = (INotification)Activator.CreateInstance(
                notificationType,
                domainEvent)!;

            await _publisher.Publish(notification, cancellationToken);
        }
    }
}
