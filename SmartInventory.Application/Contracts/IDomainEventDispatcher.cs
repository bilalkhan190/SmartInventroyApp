using SmartInventory.Domain.Common;

namespace SmartInventory.Application.Contracts;

/// <summary>
/// Publishes domain events after a successful SaveChanges.
/// Implemented in Application using MediatR notifications.
/// </summary>
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}
