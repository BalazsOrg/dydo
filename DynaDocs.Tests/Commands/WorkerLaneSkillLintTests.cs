namespace DynaDocs.Tests.Commands;

using System.Text.RegularExpressions;
using DynaDocs.Commands;

/// <summary>
/// Skill lint: a skill must not tell a sub-agent to run a dydo command the worker lane refuses.
///
/// <para>Which prose is addressed to a sub-agent (the rule, in three lines):</para>
/// <list type="number">
/// <item>Every markdown file under <c>skills/roles/crew/</c>: crew are spawned as sub-agents.</item>
/// <item>Every markdown file under <c>skills/engineering/</c>: the method skills crew load and drive
/// (statechart, pr, verify-dydo, diagnosing-bugs and the rest), except verify-dydo's feature maps
/// (<c>verify-dydo/resources/features/</c>). Those describe what the driver does in its disposable
/// project and the hostile command text the guard is tested with (<c>dydo init claude</c>,
/// <c>dydo ${X} mode set</c>): fixture and corpus, not an instruction to the verifier, whose own
/// instructions are verify-dydo's SKILL.md, scanned like the rest.</item>
/// <item>One section of an officer file under <c>skills/roles/officers/</c>: a <c>## </c> section that
/// says <c>assume no officer role</c>, the line a section carries when a sub-agent is told to read it
/// alone (the admiral check-in's "Survey - the surveyor"). The rest of an officer file is addressed to
/// the officer's own top-level session and stays out, as does <c>skills/productivity/</c>, whose skills
/// the user or an officer invokes at top level (update-dydo, overnight-mode, catch-up).</item>
/// </list>
///
/// <para>What counts as telling it to run a command: a code span or fenced-code line in that prose that
/// the guard's own worker-lane scan (<see cref="GuardCommand.FindWorkerBlockedDydoCommand"/>) would
/// refuse. The lint asks the guard, so no command name is listed here; a dydo word in plain prose, a
/// <c>dydo/</c> path and <c>dydo.json</c> are not commands.</para>
///
/// <para>Skills live only under <c>skills/</c>: the installed host copies (<c>.claude/skills</c>,
/// <c>.agents/skills</c>) are links <c>setup-skills.mjs</c> projects to it, the Scaffold ships none, and
/// the statechart payload embedded in the build is hash-checked against the same files.</para>
/// </summary>
public partial class WorkerLaneSkillLintTests
{
    private const string SubAgentSectionMarker = "assume no officer role";

    private static string SkillsRoot()
    {
        for (var dir = AppContext.BaseDirectory; dir != null; dir = Path.GetDirectoryName(dir))
        {
            var candidate = Path.Combine(dir, "skills");
            if (Directory.Exists(Path.Combine(candidate, "roles", "crew"))) return candidate;
        }
        throw new DirectoryNotFoundException($"Could not find skills/ walking up from {AppContext.BaseDirectory}");
    }

    private static IEnumerable<string> Markdown(string directory) =>
        Directory.EnumerateFiles(directory, "*.md", SearchOption.AllDirectories).Order(StringComparer.Ordinal);

    /// <summary>The sections of an officer file a sub-agent reads: those that tell it to assume no officer role.</summary>
    internal static IEnumerable<string> SubAgentSections(string officerMarkdown) =>
        SectionStartRegex().Split(officerMarkdown)
            .Where(section => section.Contains(SubAgentSectionMarker, StringComparison.OrdinalIgnoreCase));

    private static IEnumerable<(string Source, string Prose)> SubAgentProse(string skills)
    {
        var featureMaps = Path.Combine(skills, "engineering", "verify-dydo", "resources", "features") + Path.DirectorySeparatorChar;
        foreach (var directory in new[] { Path.Combine(skills, "roles", "crew"), Path.Combine(skills, "engineering") })
            foreach (var file in Markdown(directory).Where(file => !file.StartsWith(featureMaps, StringComparison.Ordinal)))
                yield return (Path.GetRelativePath(skills, file), File.ReadAllText(file));

        foreach (var file in Markdown(Path.Combine(skills, "roles", "officers")))
            foreach (var section in SubAgentSections(File.ReadAllText(file)))
                yield return (Path.GetRelativePath(skills, file) + " (sub-agent section)", section);
    }

    /// <summary>Each inline code span and each fenced-code line of <paramref name="markdown"/>, whitespace collapsed.</summary>
    internal static IEnumerable<string> CodeText(string markdown)
    {
        var inFence = false;
        var outside = new List<string>();
        foreach (var line in markdown.Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("```", StringComparison.Ordinal) || trimmed.StartsWith("~~~", StringComparison.Ordinal))
                inFence = !inFence;
            else if (inFence)
            {
                if (trimmed.Length > 0) yield return trimmed;
            }
            else
                outside.Add(line);
        }

        foreach (Match span in InlineCodeRegex().Matches(string.Join('\n', outside)))
            yield return WhitespaceRegex().Replace(span.Groups[1].Value, " ").Trim();
    }

    /// <summary>The code in <paramref name="prose"/> that tells a reader to run a dydo command the worker lane refuses.</summary>
    internal static List<string> Violations(string prose) =>
        CodeText(prose)
            .Select(code => (Code: code, Word: GuardCommand.FindWorkerBlockedDydoCommand(code)))
            .Where(found => found.Word != null)
            .Select(found => $"`{found.Code}` (dydo {found.Word})")
            .ToList();

    [Fact]
    public void SubAgentProse_TellsNoSubAgentToRunAWorkerRefusedCommand()
    {
        var skills = SkillsRoot();
        var violations = SubAgentProse(skills)
            .SelectMany(source => Violations(source.Prose).Select(violation => $"{source.Source}: {violation}"))
            .ToList();

        Assert.True(violations.Count == 0,
            "Sub-agent prose names a dydo command the worker lane refuses. Reword it, or classify the command "
            + "as worker-allowed in GuardCommand.WorkerLane.cs if a sub-agent should run it:\n  "
            + string.Join("\n  ", violations));
    }

    [Fact]
    public void SubAgentProse_ReachesTheCrewTheSurveyorAndTheEngineeringSkills()
    {
        // A lint that scans nothing passes; hold it to the prose it exists for.
        var sources = SubAgentProse(SkillsRoot()).ToList();
        var commands = sources.SelectMany(source => CodeText(source.Prose).Select(code => (source.Source, Code: code))).ToList();

        Assert.Contains(sources, source => source.Source.Replace('\\', '/').StartsWith("roles/crew/code-writer/"));
        Assert.Contains(commands, command => command.Source.Contains("check-in.md (sub-agent section)")
            && command.Code.StartsWith("dydo survey ", StringComparison.Ordinal));
        Assert.Contains(commands, command => command.Source.Replace('\\', '/').StartsWith("engineering/statechart/")
            && command.Code.StartsWith("dydo statechart ", StringComparison.Ordinal));
    }

    [Fact]
    public void SubAgentSections_LeaveTheRestOfTheCheckInToTheAdmiral()
    {
        var checkIn = File.ReadAllText(Path.Combine(SkillsRoot(), "roles", "officers", "admiral", "resources", "check-in.md"));

        var section = Assert.Single(SubAgentSections(checkIn));
        Assert.StartsWith("## Survey", section);
        Assert.DoesNotContain("## Tick", section);
        Assert.DoesNotContain("## Fixed dispatch", section);
    }

    [Theory]
    [InlineData("Run `dydo init claude` first.", "dydo init")]
    [InlineData("Run `dydo check && dydo index` first.", "dydo index")]
    [InlineData("```sh\ndydo check\n  dydo map\n```", "dydo map")]
    [InlineData("Run `dydo\n  guard` first.", "dydo guard")]
    [InlineData("Run `./dydo adopt 3.7.0`.", "dydo adopt")]
    [InlineData("Run `dydo sync`: a command that does not exist is not worker-allowed either.", "dydo sync")]
    public void Violations_NamesATopLevelOnlyCommandInCode(string prose, string expected)
    {
        var violation = Assert.Single(Violations(prose));

        Assert.Contains(expected, violation);
    }

    [Theory]
    [InlineData("Run `dydo survey P-DYD-46` and `dydo statechart install-skill --root /p`.")]
    [InlineData("Run `dydo check`, `dydo mode show`, `dydo pr-ready --body f.md`, `dydo --help`.")]
    [InlineData("```sh\ndydo statechart SPEC.tla --skill s\n```")]
    [InlineData("Plain prose names dydo init and dydo map outside code.")]
    [InlineData("Read `dydo/guides/adding-a-command.md` and `dydo.json`; dydo check passes.")]
    public void Violations_IgnoresAllowedCommandsPathsAndProse(string prose)
    {
        Assert.Empty(Violations(prose));
    }

    // The red case for the decision rule: only a section marked for a sub-agent is its prose.
    [Fact]
    public void SubAgentSections_TakesOnlyTheMarkedSection()
    {
        const string officerFile = """
            # Check-in

            ## Tick
            Run `dydo init claude`.

            ## Survey - the surveyor
            You are the surveyor. Read this section alone; assume no officer role.
            Run `dydo map`.

            ## Fixed dispatch
            Run `dydo adopt 1`.
            """;

        var sections = SubAgentSections(officerFile).ToList();

        var section = Assert.Single(sections);
        Assert.Equal(["`dydo map` (dydo map)"], Violations(section));
        Assert.DoesNotContain("dydo init", section);
        Assert.DoesNotContain("dydo adopt", section);
    }

    [GeneratedRegex(@"(?m)^(?=## )")]
    private static partial Regex SectionStartRegex();

    [GeneratedRegex(@"(?<!`)`([^`]+)`(?!`)")]
    private static partial Regex InlineCodeRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
