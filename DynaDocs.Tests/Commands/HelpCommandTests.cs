namespace DynaDocs.Tests.Commands;

using DynaDocs.Commands;

/// <summary>`dydo help` names every command the CLI registers, so a new command cannot ship undocumented.</summary>
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands.md dydo help: it names every registered command whose name does not start with `_`")]
public sealed class HelpCommandTests
{
    [Fact]
    public void Help_ListsEveryRegisteredCommand()
    {
        var registered = DydoRootCommand.Create().Subcommands
            .Select(command => command.Name)
            .Where(name => !name.StartsWith('_'))
            .ToList();

        var (code, stdout, _) = DydoCli.Run("help");

        Assert.Equal(0, code);
        var listed = stdout.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.TrimStart().Split(' ')[0])
            .ToHashSet();
        Assert.NotEmpty(registered);
        Assert.All(registered, name => Assert.Contains(name, listed));
    }
}
