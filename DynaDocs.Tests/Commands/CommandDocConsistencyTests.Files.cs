namespace DynaDocs.Tests.Commands;

using System.CommandLine;

public partial class CommandDocConsistencyTests
{
    private static string FindRepoFile(string relativePath)
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            var candidate = Path.Combine(dir, relativePath);
            if (File.Exists(candidate)) return candidate;
            dir = Path.GetDirectoryName(dir);
        }
        throw new FileNotFoundException(
            $"Could not find {relativePath} walking up from {AppContext.BaseDirectory}");
    }

    private static string FindRepoDir(string relativePath)
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            var candidate = Path.Combine(dir, relativePath);
            if (Directory.Exists(candidate)) return candidate;
            dir = Path.GetDirectoryName(dir);
        }
        throw new DirectoryNotFoundException(
            $"Could not find {relativePath} walking up from {AppContext.BaseDirectory}");
    }

    private static HashSet<string> GetAllNames(Option opt)
    {
        var names = new HashSet<string>(opt.Aliases) { opt.Name };
        return names;
    }

    private static List<Option> GetUserOptions(Command cmd) =>
        cmd.Options.Where(o => !GetAllNames(o).Any(BuiltInOptionNames.Contains)).ToList();

    private static string ReadCommandReference(string home)
    {
        var directory = Path.GetDirectoryName(FindRepoFile(Path.Combine(home, "reference", "dydo-commands.md")))!;
        return File.ReadAllText(Path.Combine(directory, "dydo-commands.md")) + "\n" +
            string.Join("\n", Directory.GetFiles(directory, "dydo-commands-*.md").Order(StringComparer.Ordinal)
                .Select(File.ReadAllText));
    }
}
