using System.Xml.Linq;

namespace CaBootstrap.CLI.Utilities;

public static class ProjectFolderRegistrar
{
    public static bool RegisterFolder(string projectFilePath, string folderName)
    {
        if (!File.Exists(projectFilePath))
        {
            return false;
        }

        var folderInclude = $"{folderName.TrimEnd('\\', '/')}\\";
        var content = File.ReadAllText(projectFilePath);

        if (content.Contains($"Include=\"{folderInclude}\"", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var document = XDocument.Parse(content, LoadOptions.PreserveWhitespace);
        var project = document.Root ?? throw new InvalidOperationException("Invalid project file.");

        var itemGroup = project.Elements("ItemGroup")
            .FirstOrDefault(group => group.Elements("Folder").Any());

        if (itemGroup is null)
        {
            itemGroup = new XElement("ItemGroup");
            project.Add(itemGroup);
        }

        itemGroup.Add(new XElement("Folder", new XAttribute("Include", folderInclude)));
        document.Save(projectFilePath);
        return true;
    }
}
