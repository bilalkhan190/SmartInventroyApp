namespace SmartInventory.Domain.Entities;

public sealed class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
