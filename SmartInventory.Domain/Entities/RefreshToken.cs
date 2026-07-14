using System.ComponentModel.DataAnnotations.Schema;

namespace SmartInventory.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }

    public User User { get; set; } = null!;

    [NotMapped]
    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;

    public void Revoke(string? replacedByTokenHash = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByTokenHash = replacedByTokenHash;
        MarkUpdated();
    }
}
