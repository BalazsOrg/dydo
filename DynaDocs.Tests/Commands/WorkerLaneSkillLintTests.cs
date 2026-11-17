namespace DynaDocs.Tests.Commands;

using System.Text.RegularExpressions;
using DynaDocs.Commands;

/// <summary>
/// Skill lint: a skill must not tell a sub-agent to run a dydo command the worker lane refuses.
///
/// <para>Which prose is addressed to a sub-agent:</para>
/// <list type="number">
/// <item>Every markdown file under <c>skills/roles/crew/</c>: crew are spawned as sub-agents.</item>
/// <item>Every markdown file under <c>skills/engineering/</c>: the method skills crew load and drive
/// (statechart, pr, verify-dydo, diagnosing-bugs and the rest), except verify-dydo's feature maps
/// (<c>verify-dydo/resources/features/</c>). Those describe what the driver does in its disposable
/// project and the hostile command text the guard is tested with (<c>dydo init claude</c>,
/// <c>dydo ${X} mode set</c>): fixture and corpus, not an instruction to the verifier, whose own
/// instructions are verify-dydo's SKILL.md, scanned like the rest.</item>
/// <item>Officer files under <c>skills/roles/officers/</c>, by where the line <c>assume no officer role</c>
/// sits, the line a file carries when a sub-agent is told to read it alone. In the text before the first
/// <c>## </c> (the chief of staff's night-survey resource): the whole file is the sub-agent's. In a
/// <c>## </c> section (the admiral check-in's "Survey - the surveyor"): that section only. The rest of an
/// officer file is addressed to the officer's own top-level session and stays out.</item>
/// <item>Every markdown file in a <c>skills/productivity/</c> skill whose name crew prose mentions
/// in a code span or skill path. This includes writing-for-agents and its resources. The exact
/// description of a project's first <c>dydo init</c> in skill-mechanics.md is descriptive, so only
/// that quoted mention is excluded; instructions elsewhere in that file are still scanned.</item>
/// </list>
///
/// <para>What counts as telling it to run a command: a code span or fenced-code line in that prose that
/// the guard's own worker-lane scan (<see cref="GuardCommand.FindWorkerBlockedDydoCommand"/>) would
/// refuse. The lint asks the guard, so no command name is listed here; a dydo word in plain prose, a
/// <c>dydo/</c> path and <c>dydo.json</c> are not commands.</para>
///
/// <para>Every worker-allowed dydo word that prose names is also run by the verify-dydo worker-lane journey:
/// the journey's hand-written <c>SKILL_COMMANDS</c> must cover each one, read from the same prose by the same
/// rule (<see cref="GuardCommand.DydoWords"/>), so a skill that starts naming a command fails here until the
/// journey runs it.</para>
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

    /// <summary>
    /// The parts of an officer file a sub-agent reads: the whole file when the text before its first
    /// <c>## </c> tells it to assume no officer role, else each section that does.
    /// </summary>
    internal static IEnumerable<string> SubAgentSections(string officerMarkdown)
    {
        var firstSection = SectionStartRegex().Match(officerMarkdown);
        var preamble = firstSection.Success ? officerMarkdown[..firstSection.Index] : officerMarkdown;
        if (IsMarked(preamble))
            return [officerMarkdown];

        return SectionStartRegex().Split(officerMarkdown).Where(IsMarked);
    }

    private static bool IsMarked(string markdown) =>
        markdown.Contains(SubAgentSectionMarker, StringComparison.OrdinalIgnoreCase);

    private static IEnumerable<(string Source, string Prose)> SubAgentProse(string skills)
    {
        var featureMaps = Path.Combine(skills, "engineering", "verify-dydo", "resources", "features") + Path.DirectorySeparatorChar;
        foreach (var directory in new[] { Path.Combine(skills, "roles", "crew"), Path.Combine(skills, "engineering") })
            foreach (var file in Markdown(directory).Where(file => !file.StartsWith(featureMaps, StringComparison.Ordinal)))
                yield return (Path.GetRelativePath(skills, file), File.ReadAllText(file));

        foreach (var file in Markdown(Path.Combine(skills, "roles", "officers")))
            foreach (var section in SubAgentSections(File.ReadAllText(file)))
                yield return (Path.GetRelativePath(skills, file) + " (sub-agent section)", section);

        var crew = string.Join('\n', Markdown(Path.Combine(skills, "roles", "crew")).Select(File.ReadAllText));
        foreach (var directory in Directory.EnumerateDirectories(Path.Combine(skills, "productivity")).Order(StringComparer.Ordinal))
        {
            var name = Path.GetFileName(directory);
            if (!crew.Contains($"`{name}`", StringComparison.Ordinal)
                && !crew.Contains($"productivity/{name}/", StringComparison.Ordinal)) continue;
            foreach (var file in Markdown(directory))
            {
                var source = Path.GetRelativePath(skills, file);
                yield return (source, ProductivityInstructions(source, File.ReadAllText(file)));
            }
        }
    }

    private const string InitializationDescription =
        "project's copy freezes at its first `dydo init` and an adopter reconciles it by hand";

    private static string ProductivityInstructions(string source, string prose) =>
        source.Replace('\\', '/') == "productivity/writing-for-agents/resources/skill-mechanics.md"
            ? prose.Replace(InitializationDescription, InitializationDescription.Replace("`", ""), StringComparison.Ordinal)
            : prose;

    /// <summary>
    /// Each inline code span (whitespace collapsed) and each fenced-code line of <paramref name="markdown"/>,
    /// the lines of one fenced block joined across continuations by the guard's own rule.
    /// </summary>
    internal static IEnumerable<string> CodeText(string markdown)
    {
        var inFence = false;
        var block = new List<string>();
        var outside = new List<string>();
        foreach (var line in markdown.Split('\n'))
        {
            var trimmed = line.Trim();
            var isFence = trimmed.StartsWith("```", StringComparison.Ordinal) || trimmed.StartsWith("~~~", StringComparison.Ordinal);
            if (isFence)
            {
                foreach (var code in FencedLines(block)) yield return code;
                block.Clear();
                inFence = !inFence;
            }
            else
                (inFence ? block : outside).Add(line);
        }

        foreach (var code in FencedLines(block)) yield return code;

        foreach (Match span in InlineCodeRegex().Matches(string.Join('\n', outside)))
            yield return WhitespaceRegex().Replace(span.Groups[1].Value, " ").Trim();
    }

    private static IEnumerable<string> FencedLines(List<string> block) =>
        GuardCommand.JoinLineContinuations(string.Join('\n', block))
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => line.Length > 0);

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
        Assert.Contains(sources, source => source.Source.Replace('\\', '/') == "productivity/writing-for-agents/SKILL.md");
        Assert.Contains(sources, source => source.Source.Replace('\\', '/') == "productivity/writing-for-agents/resources/skill-mechanics.md");
    }

    [Theory]
    [InlineData("writing-for-agents", "SKILL.md", "dydo index", false)]
    [InlineData("writing-for-agents", "resources/skill-mechanics.md", "dydo init claude", false)]
    [InlineData("another-method", "resources/steps.md", "dydo index", false)]
    [InlineData("another-method", "SKILL.md", "dydo index", true)]
    public void SubAgentProse_FindsRefusedCommandsInCrewLoadedProductivity(
        string skill, string resource, string command, bool pathReference)
    {
        var skills = Path.Combine(Path.GetTempPath(), "dydo-skill-lint-" + Guid.NewGuid().ToString("N"));
        try
        {
            foreach (var directory in new[] { "roles/crew", "roles/officers", "engineering", $"productivity/{skill}/resources",
                         "productivity/top-level-only" })
                Directory.CreateDirectory(Path.Combine(skills, directory));
            var load = pathReference ? $"Load [the method](skills/productivity/{skill}/SKILL.md)." : $"Load `{skill}` when editing prose.";
            File.WriteAllText(Path.Combine(skills, "roles/crew/SKILL.md"), load);
            File.WriteAllText(Path.Combine(skills, $"productivity/{skill}/SKILL.md"), "Read resources as needed.");
            var description = resource == "resources/skill-mechanics.md"
                ? "A project's copy freezes at its first `dydo init` and an adopter reconciles it by hand. "
                : "";
            File.WriteAllText(Path.Combine(skills, $"productivity/{skill}/{resource}"), description + $"Run `{command}`.");
            File.WriteAllText(Path.Combine(skills, "productivity/top-level-only/SKILL.md"), "Run `dydo adopt 1`.");

            var sources = SubAgentProse(skills).ToList();
            var violations = sources.SelectMany(source => Violations(source.Prose)
                .Select(violation => (source.Source, Violation: violation))).ToList();

            var found = Assert.Single(violations);
            Assert.Equal($"productivity/{skill}/{resource}", found.Source.Replace('\\', '/'));
            Assert.Equal($"`{command}` (dydo {command.Split(' ')[1]})", found.Violation);
            Assert.DoesNotContain(sources, source => source.Source.Replace('\\', '/').StartsWith("productivity/top-level-only/"));
        }
        finally
        {
            TestDirectory.Delete(skills);
        }
    }

    [Theory]
    [InlineData("project's copy freezes at its first `dydo init` and an adopter reconciles it by hand", 0)]
    [InlineData("project's copy freezes at its first `dydo init` and an adopter reconciles it by hand. Run `dydo init`.", 1)]
    [InlineData("project's copy freezes at its first `dydo index` and an adopter reconciles it by hand", 1)]
    [InlineData("Run `dydo init`.", 1)]
    public void ProductivityInstructions_ExcludesOnlyTheKnownDescription(string prose, int expectedViolations)
    {
        var instructions = ProductivityInstructions("productivity/writing-for-agents/resources/skill-mechanics.md", prose);

        Assert.Equal(expectedViolations, Violations(instructions).Count);
        Assert.NotEmpty(Violations(ProductivityInstructions("productivity/another-method/SKILL.md", prose)));
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
    [InlineData("```sh\ndydo \\\n  init claude\n```", "dydo init")]
    [InlineData("```sh\r\ndydo \\\r\n  init claude\r\n```\r\n", "dydo init")]
    [InlineData("```sh\ndydo check && \\\n  dydo \\\n  init claude\n```", "dydo init")]
    [InlineData("```sh\ndydo `\n  init claude\n```", "dydo init")]
    public void Violations_NamesATopLevelOnlyCommandInCode(string prose, string expected)
    {
        var violation = Assert.Single(Violations(prose));

        Assert.Contains(expected, violation);
    }

    [Theory]
    [InlineData("```sh\ndydo survey \\\n  P-DYD-46\n```")]
    [InlineData("```sh\r\ndydo survey \\\r\n  P-DYD-46\r\n```\r\n")]
    // A continuation ends with its block: it does not reach the first line of the next one.
    [InlineData("```sh\ndydo \\\n```\n\n```sh\ninit claude\n```")]
    public void Violations_JoinsContinuedLinesOfOneFencedBlockOnly(string prose)
    {
        Assert.Empty(Violations(prose));
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

    private static string JourneyDriver(string skills) =>
        File.ReadAllText(Path.Combine(skills, "engineering", "verify-dydo", "scripts", "control_dydo.py"));

    /// <summary>
    /// The commands in the driver's <c>SKILL_COMMANDS = (...)</c> tuple, read as Python reads it: a comment
    /// (<c>#</c> to end of line) is skipped, a string literal is an entry, and the tuple ends at the first
    /// <c>)</c> outside both. Anything else (a name, <c>+</c>, an f- or r-string, two literals with no comma
    /// between them, a nested bracket, an escape other than <c>\\ \" \'</c>, a one-string "tuple") is
    /// refused with a <see cref="FormatException"/>: the lint reads the tuple or fails, it never guesses.
    /// </summary>
    internal static List<string> JourneyCommands(string driverSource)
    {
        var commands = new List<string>();
        var sawComma = false;
        var afterLiteral = false;
        foreach (var (isComma, text, position) in TupleTokens(driverSource))
        {
            if (isComma && !afterLiteral) throw Unsupported(driverSource, position, "a comma with no entry before it");
            if (!isComma && afterLiteral) throw Unsupported(driverSource, position, "two literals with no comma between them");
            sawComma |= isComma;
            afterLiteral = !isComma;
            if (!isComma) commands.Add(Unescape(text, driverSource, position));
        }

        if (commands.Count == 0) throw Unsupported(driverSource, driverSource.Length, "an empty tuple");
        if (commands.Count == 1 && !sawComma) throw Unsupported(driverSource, driverSource.Length, "a parenthesised string, not a tuple");
        return commands;
    }

    /// <summary>
    /// The commands the journey runs: <see cref="JourneyCommands"/> once the driver's <c>worker_lane</c> is shown to
    /// iterate <c>SKILL_COMMANDS</c> itself. Crediting a tuple the journey never loops over would let an honest edit
    /// (a second tuple, a slice) stop exercising commands while the lint stays green.
    /// </summary>
    internal static List<string> ConsumedJourneyCommands(string driverSource)
    {
        var commands = JourneyCommands(driverSource);
        if (!WorkerLaneLoopsOverTheTuple(driverSource))
            throw new FormatException("The worker_lane journey in control_dydo.py must loop over SKILL_COMMANDS itself, as `for command in SKILL_COMMANDS:`, "
                + "or the lint credits commands the journey never runs (a second tuple, a slice, a call, a loop in another function). "
                + "Restore that loop, or teach this reader the new syntax.");
        return commands;
    }

    /// <summary>
    /// Whether the body of <c>def worker_lane(</c> (to the next column-0 <c>def</c> or <c>class</c>, comments dropped)
    /// holds a loop header whose iterable is exactly <c>SKILL_COMMANDS</c>.
    /// </summary>
    private static bool WorkerLaneLoopsOverTheTuple(string driverSource)
    {
        var body = new List<string>();
        var inside = false;
        foreach (var line in driverSource.Split('\n'))
        {
            if (WorkerLaneDefRegex().IsMatch(line)) inside = true;
            else if (inside && TopLevelDefinitionRegex().IsMatch(line)) break;
            if (inside) body.Add(WithoutComment(line));
        }
        return JourneyLoopRegex().IsMatch(string.Join('\n', body));
    }

    /// <summary>The literals and commas of the tuple, in order, up to its real closing paren; comments and blanks are skipped.</summary>
    private static IEnumerable<(bool IsComma, string Text, int Position)> TupleTokens(string source)
    {
        var start = SoleTupleBinding(source);

        var position = start.Index + start.Length;
        while (position < source.Length)
        {
            var token = TupleTokenRegex().Match(source, position);
            if (!token.Success)
            {
                if (source[position] != ')') throw Unsupported(source, position, "syntax the lint does not read");
                if (!TupleTailRegex().IsMatch(source, position + 1)) throw Unsupported(source, position + 1, "code after the closing paren");
                yield break;
            }

            if (token.Groups["comma"].Success || token.Groups["string"].Success)
                yield return (token.Groups["comma"].Success, token.Value, position);
            position += token.Length;
        }

        throw Unsupported(source, source.Length, "a tuple that never closes");
    }

    /// <summary>
    /// The driver's one binding of <c>SKILL_COMMANDS</c>, as the <c>SKILL_COMMANDS = (</c> the reader parses. Python
    /// runs the last binding, so a second one (a reassignment, a rebinding in a function, an assignment-shaped line
    /// in a docstring) would leave the lint crediting a tuple that never runs: none or several fail closed, naming
    /// their lines. Reads of the name are not bindings; nor are comments.
    /// </summary>
    private static Match SoleTupleBinding(string source)
    {
        var bindingLines = source.Split('\n')
            .Select((line, index) => (Line: line, Number: index + 1))
            .Where(entry => BindingRegex().IsMatch(WithoutComment(entry.Line)))
            .Select(entry => entry.Number).ToList();
        if (bindingLines.Count != 1)
            throw new FormatException($"SKILL_COMMANDS in control_dydo.py must be bound exactly once, as the tuple this lint reads, or the lint may credit "
                + $"commands Python never runs; found {bindingLines.Count} bindings (lines {string.Join(", ", bindingLines)}). "
                + "Remove the others, or teach this reader the new syntax.");

        var start = JourneyCommandsStartRegex().Match(source);
        if (!start.Success || source.Take(start.Index).Count(c => c == '\n') + 1 != bindingLines[0])
            throw new FormatException($"The one binding of SKILL_COMMANDS in control_dydo.py is not `SKILL_COMMANDS = (` at the start of a line "
                + $"(line {bindingLines[0]} of the driver). Rewrite it in that shape, or teach this reader the new syntax.");
        return start;
    }

    private static string WithoutComment(string line)
    {
        var comment = line.IndexOf('#');
        return comment < 0 ? line : line[..comment];
    }

    private static FormatException Unsupported(string source, int position, string what)
    {
        var line = source.Take(position).Count(c => c == '\n') + 1;
        return new FormatException($"SKILL_COMMANDS in control_dydo.py must be a plain tuple of double- or single-quoted string literals "
            + $"separated by commas; found {what} (line {line} of the driver). Rewrite it in that shape, or teach this reader the new syntax.");
    }

    /// <summary>The value of a quoted literal; only the escapes <c>\\</c>, <c>\"</c> and <c>\'</c> are read.</summary>
    private static string Unescape(string literal, string source, int position)
    {
        var value = new System.Text.StringBuilder();
        for (var i = 1; i < literal.Length - 1; i++)
        {
            if (literal[i] != '\\') { value.Append(literal[i]); continue; }
            var escaped = literal[++i];
            if (escaped is not ('\\' or '"' or '\'')) throw Unsupported(source, position, $"the escape \\{escaped}");
            value.Append(escaped);
        }
        return value.ToString();
    }

    /// <summary>
    /// The worker-allowed dydo words <paramref name="prose"/> names in code that no journey command runs.
    /// A word the worker lane refuses is <see cref="Violations"/>' to report, not this check's.
    /// </summary>
    internal static List<string> UnexercisedWords(IEnumerable<string> prose, IEnumerable<string> journeyCommands)
    {
        var exercised = journeyCommands.SelectMany(GuardCommand.DydoWords).ToHashSet(StringComparer.Ordinal);
        var allowed = GuardCommand.WorkerAllowedDydoSubcommands.Concat(GuardCommand.WorkerAllowedDydoOptions).ToHashSet(StringComparer.Ordinal);
        return prose.SelectMany(CodeText).SelectMany(GuardCommand.DydoWords)
            .Where(word => allowed.Contains(word) && !exercised.Contains(word))
            .Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToList();
    }

    [Fact]
    public void JourneyCommands_RunEveryWorkerAllowedCommandSubAgentProseNames()
    {
        var skills = SkillsRoot();
        var prose = SubAgentProse(skills).Select(source => source.Prose);

        var unexercised = UnexercisedWords(prose, ConsumedJourneyCommands(JourneyDriver(skills)));

        Assert.True(unexercised.Count == 0,
            "Sub-agent prose names worker-allowed dydo commands that SKILL_COMMANDS in "
            + "skills/engineering/verify-dydo/scripts/control_dydo.py does not run: "
            + string.Join(", ", unexercised.Select(word => $"dydo {word}")) + ". Add one invocation each, as the prose writes it.");
    }

    [Fact]
    public void JourneyCommands_ReadsTheDriversTupleNotAnEmptyOne()
    {
        var commands = ConsumedJourneyCommands(JourneyDriver(SkillsRoot()));

        Assert.Contains(commands, command => command.StartsWith("dydo survey ", StringComparison.Ordinal));
        Assert.Contains(commands, command => command.StartsWith("dydo statechart ", StringComparison.Ordinal));
        Assert.Contains(commands, command => command.StartsWith("dydo merge ", StringComparison.Ordinal));
        Assert.Contains("dydo --help", commands);
    }

    // The red case for the coverage rule: prose that names `dydo merge` against a journey that never runs it.
    [Theory]
    [InlineData("Run `dydo check`, then `dydo merge 12 --match-head-commit abc`.", "merge")]
    [InlineData("```sh\ndydo survey P-1\n  dydo graph\n```", "graph")]
    [InlineData("Run `dydo --version`.", "--version")]
    public void UnexercisedWords_NamesAWorkerAllowedWordTheJourneyDoesNotRun(string prose, string word)
    {
        var journey = new[] { "dydo check", "dydo survey P-DYD-46", "dydo --help" };

        Assert.Equal([word], UnexercisedWords([prose], journey));
    }

    [Theory]
    [InlineData("Run `dydo check`, then `dydo merge 12 --match-head-commit abc`.")]
    [InlineData("Plain prose names dydo merge outside code; `dydo/guides` and `dydo.json` are paths.")]
    [InlineData("Run `dydo init claude`: a refused word is the other check's, not this one's.")]
    public void UnexercisedWords_IgnoresWhatTheJourneyRunsProseAndRefusedWords(string prose)
    {
        var journey = new[] { "dydo check", "dydo merge 7 --match-head-commit def --match-base-commit ghi" };

        Assert.Empty(UnexercisedWords([prose], journey));
    }

    [Fact]
    public void JourneyCommands_ReadsEachLiteralOfTheTuple()
    {
        const string driver = """
            # a comment ("dydo ignored")
            SKILL_COMMANDS = (
                "dydo check", "dydo fix dydo/guides",
                "dydo merge 7 --match-head-commit abc")


            def refusal_lists(reason):
                return "dydo not-in-the-tuple"
            """;

        Assert.Equal(["dydo check", "dydo fix dydo/guides", "dydo merge 7 --match-head-commit abc"], JourneyCommands(driver));
    }

    // The red case for a commented-out entry: Python no longer runs it, so neither may the lint count it.
    [Fact]
    public void JourneyCommands_DoesNotCountACommentedOutEntry()
    {
        const string driver = """
            SKILL_COMMANDS = (
                "dydo check",
                # "dydo merge 7 --match-head-commit abc",
                "dydo --help")  # "dydo survey P-1"
            """;

        var commands = JourneyCommands(driver);

        Assert.Equal(["dydo check", "dydo --help"], commands);
        Assert.Equal(["merge"], UnexercisedWords(["Run `dydo merge 12 --match-head-commit abc`."], commands));
    }

    [Fact]
    public void JourneyCommands_EndsAtTheRealClosingParenNotOneInsideAString()
    {
        const string driver = """
            SKILL_COMMANDS = (
                "dydo check ) --not-the-end", 'dydo mode show)',
                "dydo merge 7 --match-head-commit abc")
            """;

        Assert.Equal(["dydo check ) --not-the-end", "dydo mode show)", "dydo merge 7 --match-head-commit abc"],
            JourneyCommands(driver));
    }

    [Fact]
    public void JourneyCommands_ReadsSingleQuotesEscapesAndATrailingComma()
    {
        const string driver = "SKILL_COMMANDS = (\r\n    'dydo check',\r\n    \"dydo say \\\"hi\\\" \\\\ it\\'s\",\r\n)  # done\r\n";

        Assert.Equal(["dydo check", "dydo say \"hi\" \\ it's"], JourneyCommands(driver));
    }

    // The red cases for unsupported syntax: read what Python would run or fail, never guess.
    [Theory]
    [InlineData("SKILL_COMMANDS = (\n    f\"dydo check\",\n    \"dydo --help\")\n")]
    [InlineData("SKILL_COMMANDS = (\n    r\"dydo check\",\n    \"dydo --help\")\n")]
    [InlineData("SKILL_COMMANDS = (\n    \"dydo check\" + \" --help\",\n    \"dydo mode show\")\n")]
    [InlineData("SKILL_COMMANDS = (\n    \"dydo check\"\n    \"dydo --help\")\n")]
    [InlineData("SKILL_COMMANDS = (\n    CHECK, \"dydo --help\")\n")]
    [InlineData("SKILL_COMMANDS = (\n    \"dydo check\", (\"dydo --help\",))\n")]
    [InlineData("SKILL_COMMANDS = (\n    \"dydo check\", \"dydo --help\") + EXTRA\n")]
    [InlineData("SKILL_COMMANDS = (\n    \"dydo check\",, \"dydo --help\")\n")]
    [InlineData("SKILL_COMMANDS = (\n    \"dydo a\\nb\", \"dydo --help\")\n")]
    [InlineData("SKILL_COMMANDS = (\n    \"\"\"dydo check\"\"\", \"dydo --help\")\n")]
    [InlineData("SKILL_COMMANDS = (\"dydo check\")\n")]
    [InlineData("SKILL_COMMANDS = ()\n")]
    [InlineData("SKILL_COMMANDS = (\n    \"dydo check\",\n    \"dydo --help\"\n")]
    public void JourneyCommands_RejectsSyntaxItDoesNotRead(string driver)
    {
        var failure = Assert.Throws<FormatException>(() => JourneyCommands(driver));

        Assert.Contains("SKILL_COMMANDS", failure.Message);
    }

    private const string RealTuple = "SKILL_COMMANDS = (\n    \"dydo check\", \"dydo merge 1\")\n";

    // The red cases for a second binding: Python runs the last one (or the one a function rebinds), so the lint
    // that credits the first tuple it sees would credit commands that never run. Any second binding fails, naming its line.
    [Theory]
    [InlineData(RealTuple + "SKILL_COMMANDS = (\"dydo check\", \"dydo --help\")\n", "1, 3")]
    [InlineData(RealTuple + "SKILL_COMMANDS = SKILL_COMMANDS[:1]\n", "1, 3")]
    [InlineData("\"\"\"\nSKILL_COMMANDS = (\"dydo check\", \"dydo merge 1\")\n\"\"\"\nSKILL_COMMANDS = (\n    \"dydo check\", \"dydo --help\")\n", "2, 4")]
    [InlineData(RealTuple + "\ndef reset():\n    global SKILL_COMMANDS\n    SKILL_COMMANDS = ()\n", "1, 5, 6")]
    [InlineData(RealTuple + "for SKILL_COMMANDS in range(3):\n    pass\n", "1, 3")]
    [InlineData(RealTuple + "for index, SKILL_COMMANDS in enumerate(rows):\n    pass\n", "1, 3")]
    [InlineData(RealTuple + "del SKILL_COMMANDS\n", "1, 3")]
    [InlineData(RealTuple + "from elsewhere import SKILL_COMMANDS\n", "1, 3")]
    [InlineData(RealTuple + "import elsewhere as SKILL_COMMANDS\n", "1, 3")]
    [InlineData(RealTuple + "SKILL_COMMANDS += (\"dydo --help\",)\n", "1, 3")]
    [InlineData(RealTuple + "SKILL_COMMANDS |= other\n", "1, 3")]
    [InlineData(RealTuple + "SKILL_COMMANDS: tuple = ()\n", "1, 3")]
    [InlineData(RealTuple + "if (SKILL_COMMANDS := ()):\n    pass\n", "1, 3")]
    [InlineData(RealTuple + "control.SKILL_COMMANDS = ()\n", "1, 3")]
    [InlineData(RealTuple + "def SKILL_COMMANDS():\n    pass\n", "1, 3")]
    public void JourneyCommands_RejectsAnyBindingBesidesTheTuple(string driver, string lines)
    {
        var failure = Assert.Throws<FormatException>(() => JourneyCommands(driver));

        Assert.Contains("SKILL_COMMANDS", failure.Message);
        Assert.Contains("control_dydo.py", failure.Message);
        Assert.Contains($"(lines {lines})", failure.Message);
    }

    // The red cases for a binding the reader would not parse: it is the only one, and still not the tuple it reads.
    [Theory]
    [InlineData("x = 1\n", "found 0 bindings")]
    [InlineData("    SKILL_COMMANDS = (\"dydo check\", \"dydo --help\")\n", "is not `SKILL_COMMANDS = (` at the start of a line (line 1")]
    [InlineData("SKILL_COMMANDS: tuple = (\"dydo check\", \"dydo --help\")\n", "is not `SKILL_COMMANDS = (` at the start of a line (line 1")]
    [InlineData("SKILL_COMMANDS = list((\"dydo check\", \"dydo --help\"))\n", "is not `SKILL_COMMANDS = (` at the start of a line (line 1")]
    public void JourneyCommands_RejectsADriverWhoseOnlyBindingIsNotThePlainTuple(string driver, string found)
    {
        var failure = Assert.Throws<FormatException>(() => JourneyCommands(driver));

        Assert.Contains("SKILL_COMMANDS", failure.Message);
        Assert.Contains(found, failure.Message);
    }

    // Reads, comments and look-alike names are not bindings: the driver's own loop reads the tuple.
    [Fact]
    public void JourneyCommands_DoesNotCountReadsCommentsOrLookAlikeNamesAsBindings()
    {
        const string driver = """
            # SKILL_COMMANDS = ("dydo --help",)
            SKILL_COMMANDS = (
                "dydo check", "dydo merge 1")
            SKILL_COMMANDS_EXTRA = ("dydo --help",)
            MY_SKILL_COMMANDS = ("dydo --help",)

            def run(control, other):
                #   SKILL_COMMANDS = ("dydo --help",)
                for command in SKILL_COMMANDS:
                    print(command)
                total = len(control.SKILL_COMMANDS) + len(SKILL_COMMANDS)
                first = SKILL_COMMANDS[:1]
                same = SKILL_COMMANDS == other
                return [c for c in SKILL_COMMANDS if c in other] or (other in SKILL_COMMANDS)
            """;

        Assert.Equal(["dydo check", "dydo merge 1"], JourneyCommands(driver));
    }

    // The default-argument form is a read of the tuple, not a binding; a parameter named like it is one.
    [Fact]
    public void JourneyCommands_CountsADefaultArgumentAsAReadAndAParameterAsABinding()
    {
        Assert.Equal(["dydo check", "dydo merge 1"],
            JourneyCommands(RealTuple + "\ndef worker_lane(run, commands=SKILL_COMMANDS):\n    pass\n"));
        Assert.Equal(["dydo check", "dydo merge 1"],
            JourneyCommands(RealTuple + "\ndef worker_lane(run, commands: tuple = SKILL_COMMANDS):\n    pass\n"));

        foreach (var parameter in new[] { "SKILL_COMMANDS", "run, SKILL_COMMANDS=()", "run, *SKILL_COMMANDS" })
        {
            var failure = Assert.Throws<FormatException>(() => JourneyCommands(RealTuple + $"\ndef worker_lane({parameter}):\n    pass\n"));
            Assert.Contains("(lines 1, 4)", failure.Message);
        }
    }

    private const string TupleAndLoop = RealTuple + "\n\ndef worker_lane(run):\n    \"\"\"Doc.\"\"\"\n    for command in SKILL_COMMANDS:\n        guard(run, command)\n";

    // The red cases for a tuple the journey does not loop over: bound once and read nowhere that runs the commands.
    [Theory]
    [InlineData(RealTuple + "SMOKE_COMMANDS = (\"dydo check\", \"dydo --help\")\n\n\ndef worker_lane(run):\n    for command in SMOKE_COMMANDS:\n        guard(run, command)\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    for command in SKILL_COMMANDS[:3]:\n        guard(run, command)\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    for command in SKILL_COMMANDS + EXTRA:\n        guard(run, command)\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    for command in sorted(SKILL_COMMANDS):\n        guard(run, command)\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    for command in SKILL_COMMANDS_EXTRA:\n        guard(run, command)\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    for command, word in SKILL_COMMANDS:\n        guard(run, command)\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    return [guard(run, c) for c in SKILL_COMMANDS]\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    # for command in SKILL_COMMANDS:\n    pass\n")]
    [InlineData(RealTuple + "\n\ndef other(run):\n    pass\n")]
    [InlineData(RealTuple + "\n\ndef other(run):\n    for command in SKILL_COMMANDS:\n        guard(run, command)\n\n\ndef worker_lane(run):\n    pass\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    pass\n\n\ndef other(run):\n    for command in SKILL_COMMANDS:\n        guard(run, command)\n")]
    [InlineData(RealTuple + "\n\nclass Lane:\n    for command in SKILL_COMMANDS:\n        pass\n\n\ndef worker_lane(run):\n    pass\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    pass\n\n\nclass Lane:\n    for command in SKILL_COMMANDS:\n        pass\n")]
    [InlineData(RealTuple)]
    public void ConsumedJourneyCommands_RejectsADriverWhoseWorkerLaneDoesNotLoopOverTheTuple(string driver)
    {
        var failure = Assert.Throws<FormatException>(() => ConsumedJourneyCommands(driver));

        Assert.Contains("SKILL_COMMANDS", failure.Message);
        Assert.Contains("worker_lane", failure.Message);
        Assert.Contains("control_dydo.py", failure.Message);
    }

    [Theory]
    [InlineData(TupleAndLoop)]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    for c in SKILL_COMMANDS :  # each one\n        guard(run, c)\n")]
    [InlineData(RealTuple + "\n\ndef worker_lane(run):\n    for _command2 in SKILL_COMMANDS: guard(run, _command2)\n")]
    [InlineData(RealTuple + "\n\ndef setup():\n    pass\n\n\ndef worker_lane(run):\n    hook = installed_hook(run)\n    if hook:\n        for command in SKILL_COMMANDS:\n            guard(run, command)\n\n\ndef activation(run):\n    pass\n")]
    [InlineData("def worker_lane(run, commands):\n    for command in SKILL_COMMANDS:\n        pass\n\n\n" + RealTuple)]
    public void ConsumedJourneyCommands_AcceptsAWorkerLaneThatLoopsOverTheTuple(string driver)
    {
        Assert.Equal(["dydo check", "dydo merge 1"], ConsumedJourneyCommands(driver));
    }

    // The binding check still comes first: the loop proves nothing about which tuple a second binding leaves.
    [Fact]
    public void ConsumedJourneyCommands_StillRejectsASecondBinding()
    {
        var failure = Assert.Throws<FormatException>(() => ConsumedJourneyCommands(TupleAndLoop + "SKILL_COMMANDS = ()\n"));

        Assert.Contains("found 2 bindings", failure.Message);
    }

    // The accept case for a read written as a default argument: the tuple is bound once and the journey loops over it.
    [Fact]
    public void ConsumedJourneyCommands_AcceptsTheTupleReadAsADefaultArgument()
    {
        const string driver = RealTuple + "\n\ndef worker_lane(run, commands=SKILL_COMMANDS):\n    for command in SKILL_COMMANDS:\n        pass\n";

        Assert.Equal(["dydo check", "dydo merge 1"], ConsumedJourneyCommands(driver));
    }

    // The red case for a marker in the intro: the whole file is the sub-agent's, later steps included.
    [Fact]
    public void SubAgentSections_TakesTheWholeFileWhenTheIntroCarriesTheMarker()
    {
        const string officerFile = """
            # Night survey

            You are the surveyor. Read this resource alone; assume no officer role.

            ## Survey
            Run `dydo map`.

            ## Fixed dispatch
            Run `dydo adopt 1`.
            """;

        var sections = SubAgentSections(officerFile).ToList();

        var whole = Assert.Single(sections);
        Assert.Equal(officerFile, whole);
        Assert.Equal(["`dydo map` (dydo map)", "`dydo adopt 1` (dydo adopt)"], Violations(whole));
    }

    [Fact]
    public void SubAgentSections_AllowsAWholeFileWithOnlyAllowedCommandsAfterTheMarker()
    {
        const string officerFile = """
            # Night survey

            Read this resource alone; assume no officer role.

            ## Survey
            Run `dydo survey P-1`.
            """;

        Assert.Empty(SubAgentSections(officerFile).SelectMany(Violations));
    }

    [Fact]
    public void SubAgentSections_ReadsTheNightSurveyFromItsIntroThroughItsSteps()
    {
        var nightSurvey = File.ReadAllText(Path.Combine(SkillsRoot(), "roles", "officers", "chief-of-staff", "resources", "night-survey.md"));

        var whole = Assert.Single(SubAgentSections(nightSurvey));
        Assert.Contains("## Survey", whole);
        Assert.Contains("## Return", whole);
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

    // A line that binds the name: an assignment or augmented assignment (also `obj.SKILL_COMMANDS =`), a walrus, a
    // statement-start annotation, `global`/`nonlocal`/`del`/`import`/`def`/`class` naming it (but not after an `=`:
    // `def f(run, commands=SKILL_COMMANDS)` reads it), `as NAME`, and a `for` target (the tempered part stops at
    // `in`, so `for c in SKILL_COMMANDS` is a read).
    [GeneratedRegex(@"(?<!\w)SKILL_COMMANDS[ \t]*(?::=|(?:[-+*/%&|^@]|//|\*\*|>>|<<)?=(?!=))"
        + @"|^[ \t]*SKILL_COMMANDS[ \t]*:"
        + @"|\b(?:global|nonlocal|del|import|def|class)\b[^\n]*?(?<!\w|=[ \t]*)SKILL_COMMANDS\b"
        + @"|\bas[ \t]+SKILL_COMMANDS\b"
        + @"|\bfor[ \t]+(?:(?!\bin\b)[^\n])*?(?<!\w)SKILL_COMMANDS\b")]
    private static partial Regex BindingRegex();

    [GeneratedRegex(@"^def worker_lane\(")]
    private static partial Regex WorkerLaneDefRegex();

    [GeneratedRegex(@"^(?:def|class) ")]
    private static partial Regex TopLevelDefinitionRegex();

    // A loop header over the tuple itself: a plain loop name, the iterable exactly SKILL_COMMANDS, then the colon.
    [GeneratedRegex(@"(?m)^[ \t]*for[ \t]+[A-Za-z_]\w*[ \t]+in[ \t]+SKILL_COMMANDS[ \t]*:")]
    private static partial Regex JourneyLoopRegex();

    [GeneratedRegex(@"(?m)^SKILL_COMMANDS = \(")]
    private static partial Regex JourneyCommandsStartRegex();

    // One token at the scan position, tried in this order: a comment, a quoted literal, a comma, whitespace.
    [GeneratedRegex(@"\G(?:(?<comment>#[^\r\n]*)|(?<string>""(?:[^""\\\r\n]|\\.)*""|'(?:[^'\\\r\n]|\\.)*')|(?<comma>,)|(?<space>\s+))")]
    private static partial Regex TupleTokenRegex();

    // After the closing paren only blanks and a comment may finish the line.
    [GeneratedRegex(@"\G[ \t]*(?:#[^\r\n]*)?(?:\r?\n|\z)")]
    private static partial Regex TupleTailRegex();

    [GeneratedRegex(@"(?m)^(?=## )")]
    private static partial Regex SectionStartRegex();

    [GeneratedRegex(@"(?<!`)`([^`]+)`(?!`)")]
    private static partial Regex InlineCodeRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
