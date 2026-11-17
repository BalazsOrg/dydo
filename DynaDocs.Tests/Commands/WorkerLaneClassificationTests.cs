namespace DynaDocs.Tests.Commands;

using DynaDocs.Commands;

/// <summary>
/// The worker lane classifies every dydo command in one place (GuardCommand.WorkerLane.cs): worker-allowed
/// or on the explicit top-level-only list. These tests hold that classification to the commands the real
/// root registers, so a new command fails here until its author decides, and to the guard's actual
/// answers and refusal text, so the lists cannot drift from what the guard does.
/// </summary>
public class WorkerLaneClassificationTests
{
    private static readonly string[] Allowed = GuardCommand.WorkerAllowedDydoSubcommands;
    private static readonly string[] TopLevelOnly = GuardCommand.TopLevelOnlyDydoSubcommands;
    private static readonly string[] Classified = [.. Allowed, .. TopLevelOnly];

    /// <summary>
    /// What is wrong with a classification of <paramref name="registered"/>: a command on neither list
    /// (a new command nobody decided), a list entry nothing registers (a retired or misspelt command), and
    /// a command on both lists (contradictory decisions).
    /// </summary>
    internal static List<string> Problems(IEnumerable<string> registered, IEnumerable<string> allowed, IEnumerable<string> topLevelOnly)
    {
        var registeredSet = registered.ToHashSet(StringComparer.Ordinal);
        var allowedSet = allowed.ToHashSet(StringComparer.Ordinal);
        var topLevelSet = topLevelOnly.ToHashSet(StringComparer.Ordinal);
        var problems = new List<string>();
        problems.AddRange(registeredSet.Except(allowedSet).Except(topLevelSet).Order(StringComparer.Ordinal)
            .Select(name => $"'{name}' is registered but classified on neither list: add it to WorkerAllowedDydoSubcommands or TopLevelOnlyDydoSubcommands in GuardCommand.WorkerLane.cs"));
        problems.AddRange(allowedSet.Union(topLevelSet).Except(registeredSet).Order(StringComparer.Ordinal)
            .Select(name => $"'{name}' is classified but no registered command has that name"));
        problems.AddRange(allowedSet.Intersect(topLevelSet).Order(StringComparer.Ordinal)
            .Select(name => $"'{name}' is on both the worker-allowed and the top-level-only list"));
        return problems;
    }

    private static List<string> RegisteredNames() =>
        DydoRootCommand.Create().Subcommands.Select(command => command.Name).ToList();

    [Fact]
    public void EveryRegisteredCommand_IsClassifiedExactlyOnce()
    {
        var problems = Problems(RegisteredNames(), Allowed, TopLevelOnly);

        Assert.True(problems.Count == 0, string.Join("\n", problems));
    }

    [Fact]
    public void ListsHaveNoDuplicateEntries()
    {
        Assert.Equal(Allowed.Length, Allowed.Distinct().Count());
        Assert.Equal(TopLevelOnly.Length, TopLevelOnly.Distinct().Count());
    }

    [Fact]
    public void HelpAndVersion_AreRegisteredAndWorkerAllowed()
    {
        // help and version are real subcommands on the root, not only the -h/--version options.
        Assert.Contains("help", RegisteredNames());
        Assert.Contains("version", RegisteredNames());
        Assert.Contains("help", Allowed);
        Assert.Contains("version", Allowed);
    }

    [Fact]
    public void AllowedOptionWords_AreOptionsTheRootAcceptsAndNotCommands()
    {
        var root = DydoRootCommand.Create();
        var rootAliases = root.Options.SelectMany(option => option.Aliases.Append(option.Name)).ToHashSet();

        Assert.All(GuardCommand.WorkerAllowedDydoOptions, word => Assert.Contains(word, rootAliases));
        Assert.Empty(GuardCommand.WorkerAllowedDydoOptions.Intersect(RegisteredNames()));
    }

    [Fact]
    public void SurveyAndStatechart_AreWorkerAllowed()
    {
        // DYD-963: the admiral check-in's surveyor runs survey and crew run statechart.
        Assert.Contains("survey", Allowed);
        Assert.Contains("statechart", Allowed);
        Assert.DoesNotContain("survey", TopLevelOnly);
        Assert.DoesNotContain("statechart", TopLevelOnly);
    }

    [Fact]
    public void Archive_IsRegisteredAndWorkerAllowed()
    {
        Assert.Contains("archive", RegisteredNames());
        Assert.Contains("archive", Allowed);
        Assert.DoesNotContain("archive", TopLevelOnly);
    }

    // The red case: the check fails on a registry with a command nobody classified.
    [Fact]
    public void Problems_NamesANewUnclassifiedCommand()
    {
        var problems = Problems([.. Classified, "frobnicate"], Allowed, TopLevelOnly);

        var problem = Assert.Single(problems);
        Assert.Contains("'frobnicate' is registered but classified on neither list", problem);
    }

    [Fact]
    public void Problems_NamesAClassifiedCommandNothingRegisters()
    {
        var problems = Problems(Classified.Where(name => name != "survey"), Allowed, TopLevelOnly);

        Assert.Equal(["'survey' is classified but no registered command has that name"], problems);
    }

    [Fact]
    public void Problems_NamesACommandOnBothLists()
    {
        var problems = Problems(Classified, [.. Allowed, "init"], TopLevelOnly);

        Assert.Equal(["'init' is on both the worker-allowed and the top-level-only list"], problems);
    }

    // The lists are what the guard does, not a second opinion.
    [Fact]
    public void Guard_AllowsEveryWorkerAllowedCommand_AndBlocksEveryTopLevelOnlyOne()
    {
        Assert.All(Allowed, name => Assert.Null(GuardCommand.FindWorkerBlockedDydoCommand($"dydo {name}")));
        Assert.All(TopLevelOnly, name => Assert.Equal(name, GuardCommand.FindWorkerBlockedDydoCommand($"dydo {name}")));
        Assert.All(GuardCommand.WorkerAllowedDydoOptions, word => Assert.Null(GuardCommand.FindWorkerBlockedDydoCommand($"dydo {word}")));
    }

    [Fact]
    public void Refusal_ListsExactlyTheClassification()
    {
        var lines = GuardCommand.WorkerRefusalLines("sync");

        Assert.Equal(3, lines.Length);
        Assert.Equal(
            "BLOCKED: Sub-agents may run only these dydo commands: " + string.Join(", ", Allowed) + " (plus -h/--help/--version).",
            lines[0]);
        Assert.Equal("  Not allowed here: dydo sync", lines[1]);
        Assert.Equal(
            "  These belong to the top-level session that spawned you: " + string.Join(", ", TopLevelOnly) + ".",
            lines[2]);
    }
}
