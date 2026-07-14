namespace SmartInventory.Domain.Common;

/// <summary>
/// Marker for something important that already happened in the domain.
/// Domain layer stays free of MediatR / infrastructure.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
