using SmartInventory.Infrastructure.Identity;

namespace SmartInventory.Presentation.Authentication;

public sealed class PasswordService
{
    public string Hash(string password) => PasswordHashing.Hash(password);

    public bool Verify(string password, string passwordHash) =>
        PasswordHashing.Verify(password, passwordHash);
}
