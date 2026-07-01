namespace CaBootstrap.CLI.Models;

public sealed class ProjectInfo
{
    public required string Name { get; init; }
    public required string FilePath { get; init; }
    public required string DirectoryPath { get; init; }
    public required ProjectRole Role { get; init; }

    public string RoleKey => Role.ToString();
}
