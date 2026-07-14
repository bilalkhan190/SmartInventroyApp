namespace SmartInventory.Domain.Common;

/// <summary>
/// Thrown when a business rule is broken (e.g. approving a non-pending PO).
/// Application handlers catch this and return a friendly error.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
