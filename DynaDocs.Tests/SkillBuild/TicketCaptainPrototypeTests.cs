namespace DynaDocs.Tests.SkillBuild;

using System.Text.RegularExpressions;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 C and G: the built ticket-captain prototype against the user's rulings on DYD-1295 and
/// DYD-1372, each an acceptance line of the contract (W1 to W31, X2 to X17). Through
/// <see cref="SkillBuilder.Build"/> on the production model, read as the built text a captain loads.
/// Only his own two sentences and his roster token are pinned word for word; every other check is read
/// from the model, by claim id, so an edit of the prose leaves it standing (DR 067).
/// </summary>
public partial class TicketCaptainPrototypeTests
{
    private const string Folder = "skills/roles/officers/ticket-captain/";
    private const string Review = "dydo/reference/claim-review/";
    private const string OpenMark = "(open: his call)";

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static string Spine => Built.Value.Unwritten[Folder + "SKILL.md"];

    private static KeyValuePair<string, string>[] Files =>
        [.. Built.Value.Unwritten.Where(file => file.Key.StartsWith(Folder, StringComparison.Ordinal))];

    // The built lines of the folder that state a claim, each with its file and line number.
    private static MapLine[] Stated => [.. Built.Value.Map.Where(line => line.Path.StartsWith(Folder, StringComparison.Ordinal))];

    [GeneratedRegex(@"^(\d+)\. ")]
    private static partial Regex Numbered();

    [GeneratedRegex(@"static Line \w+\([^)]*\)\s*=>[^;]*;")]
    private static partial Regex LineHelper();

    [Fact]
    [Trait("Job", "contract: DYD-1372 W1 W3 the spine is its job paragraph, five numbered phases in their real order, then When things go wrong")]
    public void Spine_HoldsItsPhasesInOrder() =>
        Assert.Equal(["## 1. Take the ticket", "## 2. Do it or split it", "## 3. Dispatch", "## 4. Get it reviewed", "## 5. Finish", "## When things go wrong"],
            Spine.Split('\n').Where(line => line.StartsWith("## ", StringComparison.Ordinal)));

    [Fact]
    [Trait("Job", "contract: DYD-1372 W2 the spine is at most half the legacy spine's characters (DYD-1295 519126ab)")]
    public void Spine_IsAtMostHalfTheLegacySpine()
    {
        var legacy = Built.Value.Unbuilt[Folder + "SKILL.md"];

        Assert.True(Spine.Length * 2 <= legacy.Length, $"the spine is {Spine.Length} characters and the legacy spine {legacy.Length}");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 W26 W9 the worktree sentence he accepted stands verbatim in Dispatch, and delegate_task is named there and nowhere else in the spine")]
    public void Dispatch_NamesDelegateTaskAndTheWorktreeSentenceVerbatim()
    {
        var dispatch = Section("## 3. Dispatch");

        Assert.Contains("\nGive a child its own worktree (`isolation: \"worktree\"`) when it writes in parallel with others or will open its own PR; serial work shares yours.\n", dispatch);
        Assert.Contains("`delegate_task`", dispatch);
        Assert.DoesNotContain("delegate_task", Spine.Replace(dispatch, ""));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 W10 W31 the severities are defined in the review phase, in his words (DYD-1295 41a553f1)")]
    public void ReviewPhase_DefinesTheSeveritiesInHisWords() =>
        Assert.Contains("A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.", Section("## 4. Get it reviewed"));

    [Fact]
    [Trait("Job", "trap: DYD-1372 W11 no line of the spine has the captain judge or review: it dispatches a review and never reviews (DYD-1295 2119cd65)")]
    public void Spine_NeverHasTheCaptainJudgeOrReview()
    {
        var acts = Stated.Where(line => line.Path == Folder + "SKILL.md").Select(line => line.Text).ToArray();

        Assert.NotEmpty(acts);
        Assert.DoesNotContain("judge", Spine, StringComparison.OrdinalIgnoreCase);
        Assert.All(acts, act => Assert.DoesNotMatch("^(Review|Judge) ", act));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 W24 the rate-limit line opens When things go wrong, and every line of the section stands above its scenario links")]
    public void WhenThingsGoWrong_OpensWithTheRateLimitLine()
    {
        var lines = Spine.Split('\n');
        var start = Array.IndexOf(lines, "## When things go wrong");
        var stated = Stated.Where(line => line.Path == Folder + "SKILL.md").ToDictionary(line => line.Line);
        var bullets = Enumerable.Range(start + 1, lines.Length - start - 1).Where(index => lines[index].StartsWith("- ", StringComparison.Ordinal)).ToArray();
        var links = bullets.Where(index => !stated.ContainsKey(index + 1)).ToArray();

        Assert.Contains(Id(CaptainFinish.Limits), stated[bullets[0] + 1].Claims);
        Assert.NotEmpty(links);
        Assert.All(links, index => Assert.StartsWith("- [", lines[index]));
        Assert.True(bullets.Where(index => stated.ContainsKey(index + 1)).All(index => index < links[0]), "a line of the section stands below a scenario link");
    }

    [Theory]
    [Trait("Job", "trap: DYD-1372 W4 W8 W11 W22 W23 W25 W30 no file of the folder says what he ruled out")]
    [InlineData("rule on")]
    [InlineData("authors nothing")]
    [InlineData("review the parent")]
    [InlineData("a blocker that is broken")]
    [InlineData("you get reviewed")]
    [InlineData("need for the user, with")]
    [InlineData("only the user can settle")]
    [InlineData("<KEY>-<slug>")]
    [InlineData("Create the ticket's branch")]
    [InlineData("Codex")]
    [InlineData("429")]
    [InlineData("lands only")]
    [InlineData("the landing")]
    [InlineData("Balazs")]
    [InlineData("one sub-ticket per blocker")]
    public void NoFile_SaysWhatHeRuledOut(string phrase) =>
        Assert.All(Files, file => Assert.DoesNotContain(phrase, file.Value, StringComparison.OrdinalIgnoreCase));

    [Fact]
    [Trait("Job", "contract: DYD-1372 W15 W16 option A: implement, review and research are kind resources the first phase links; a scout is a native helper and has no file")]
    public void KindResources_AreTheThreeTheFirstPhaseLinks()
    {
        var take = Section("## 1. Take the ticket");

        Assert.All(new[] { "implement", "review", "research" }, kind => Assert.Contains($"](resources/{kind}.md) |", take));
        Assert.DoesNotContain(Files, file => file.Key.Contains("scout", StringComparison.Ordinal));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 hop 2 no phase lists its resources: each resource is linked from the line that uses it, a scenario from When things go wrong")]
    public void EveryResource_IsLinkedFromALineOfTheSpine()
    {
        Assert.DoesNotContain("Resources:", Spine);
        Assert.All(Files.Where(file => file.Key.Contains("/resources/", StringComparison.Ordinal)),
            file => Assert.Contains($"](resources/{Path.GetFileName(file.Key)})", Spine));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 E X10 X12 X13 X17 a line that depends on an open point ends with the mark, and one line is left that does: the PR its captain attaches by hand")]
    public void LinesOnAnOpenPoint_EndWithTheMark()
    {
        var marked = Stated.Where(line => line.Text.Contains(OpenMark, StringComparison.Ordinal)).ToArray();

        Assert.All(marked, line => Assert.EndsWith(OpenMark, line.Text));
        Assert.Equal(["CaptainReview.Attach"], marked.Select(line => line.Claims[0]));
        Assert.Equal(marked.Length, Files.Sum(file => file.Value.Split(OpenMark).Length - 1));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A3c X11 no open claim's line stands in a file of the skill but the one placeholder's, and each open claim is listed for review")]
    public void OpenClaims_AreWithheldAndListed()
    {
        // The ticket captain's own: a resource truth-checked ahead of another skill lists its open claims itself (DYD-1402).
        var open = Claims.Where(claim => claim.Key.Truth?.Evidence.Kind == EvidenceKind.Open && claim.Key.Lines.Any(line => Equals(line.Skill, CaptainSkill.TicketCaptain))).ToArray();
        var list = Built.Value.Files[Review + "ticket-captain-open.md"].Split('\n').Where(line => line.StartsWith("## ", StringComparison.Ordinal));

        Assert.NotEmpty(open);
        Assert.Equal(open.Select(claim => "## " + claim.Value).Order(StringComparer.Ordinal), list.Order(StringComparer.Ordinal));
        Assert.All(open.Where(claim => claim.Key.Truth!.Evidence.Token is null).SelectMany(claim => claim.Key.Lines),
            line => Assert.All(Files, file => Assert.DoesNotContain(line.Text, file.Value, StringComparison.Ordinal)));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X3 the roster is his to write: its token stands alone on a line under Seats, once in the folder, and the open list marks it a placeholder (07c86aad)")]
    public void Roster_StandsAsItsPlaceholderUnderSeats()
    {
        var token = CaptainDispatch.Roster.Truth!.Evidence.Token;
        var list = Built.Value.Files[Review + "ticket-captain-open.md"];

        Assert.Equal("{roster_placeholder}", token);
        Assert.Contains($"\n\n{token}\n\n**Sending.**\n", Section("## 3. Dispatch"));
        Assert.Equal(1, Files.Sum(file => file.Value.Split(token).Length - 1));
        Assert.Contains($"## {Id(CaptainDispatch.Roster)}\n", list);
        Assert.Contains("\n\nPlaceholder: ", list);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X7 the dispatch phase repeats at least two anchor words of the frame every agent reads in AGENTS.md (aa5b445e)")]
    public void Dispatch_RepeatsAnchorWordsOfTheFrame()
    {
        var frame = File.ReadAllText(Path.Combine(CommittedTree.Root, "AGENTS.md"));
        var dispatch = Section("## 3. Dispatch");
        string[] anchors = ["one thing", "outcome", "leaf", "contract is met", "only the obvious"];

        Assert.All(anchors, anchor => Assert.Contains(anchor, frame));
        Assert.True(anchors.Count(anchor => dispatch.Contains(anchor, StringComparison.Ordinal)) >= 2, "the dispatch phase repeats fewer than two anchor words of AGENTS.md");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X2 a captain that cannot see the route to the spec is sent to the wayfinder by a line that stands above the cut (777d396e, 07c86aad)")]
    public void Fog_SendsACaptainToTheWayfinderBeforeItCuts()
    {
        var fog = LineOf("CaptainWay.Fog");

        Assert.Equal(Folder + "SKILL.md", fog.Path);
        Assert.Equal(fog.Path, LineOf("CaptainWay.Route").Path);
        Assert.Contains("`/wayfinder`", fog.Text);
        Assert.Contains("](resources/wayfind.md)", fog.Text);
        Assert.True(fog.Line < LineOf(CaptainSplit.Cut).Line, "the spine tells a captain to cut before it tells it to find the way");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X9 the filler resource says what a captain does when no ticket is left, below the rule for what it may pull (777d396e)")]
    public void Filler_SaysWhatACaptainDoesWithNoTicketLeft()
    {
        var empty = LineOf("CaptainDispatch.Empty");

        Assert.Equal(Folder + "resources/filler.md", empty.Path);
        Assert.True(empty.Line > LineOf(CaptainDispatch.Pull).Line, "the empty lane is settled above the pull rule");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X13 a need for the user is one numbered procedure that names the tool a step uses, and carries no open mark")]
    public void NeedForTheUser_IsOneNumberedProcedure()
    {
        var need = Resource("need-for-the-user");
        var steps = need.Split('\n').Select((line, index) => (Line: line, Index: index)).Where(row => Numbered().IsMatch(row.Line)).ToArray();

        Assert.True(steps.Length > 2, "the resource holds no procedure");
        Assert.Equal(steps.Length - 1, steps[^1].Index - steps[0].Index);
        Assert.All(new[] { "`delegate_task`", "`t3_thread_send`" }, tool => Assert.Contains(steps, step => step.Line.Contains(tool, StringComparison.Ordinal)));
        Assert.DoesNotContain(OpenMark, need);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 X17 nothing links a PR to its ticket, so the line after the spine's PR line, the one that has it opened (g6 H2), has its captain attach it, with the tool it names")]
    public void Pr_IsAttachedToItsTicketByTheLineAfterIt()
    {
        var attach = LineOf("CaptainReview.Attach");

        Assert.Contains(" PR ", LineOf(CaptainReview.Whose).Text);
        Assert.Equal(LineOf(CaptainReview.Whose).Line + 1, attach.Line);
        Assert.Contains("`save_issue`", attach.Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A7 the ledger gives every claim line of the legacy spine a fate of its own, and a drop names the truth question it fails")]
    public void Ledger_GivesEveryLegacyLineItsFate()
    {
        var legacy = Built.Value.Unbuilt[Folder + "SKILL.md"].Split('\n');
        var paragraph = legacy.SkipWhile(line => !line.StartsWith("# ", StringComparison.Ordinal)).Skip(2).TakeWhile(line => line.Length > 0).Count();
        var ledger = string.Concat(Enumerable.Range(1, 7).Select(part => Built.Value.Files[$"{Review}ticket-captain-ledger-{part}.md"]));
        var entries = ledger.Split('\n').Where(line => line.StartsWith("`claim:", StringComparison.Ordinal) && line.Contains(", ", StringComparison.Ordinal)).ToArray();
        var drops = entries.Where(entry => entry.Contains(", dropped,", StringComparison.Ordinal)).ToArray();

        Assert.Equal(paragraph + legacy.Count(line => line.StartsWith("1. ", StringComparison.Ordinal)), entries.Length);
        // No entry's own words are a placeholder; the one line that holds the word is the roster's, kept in the claim whose text is not written (fix g5 N3).
        Assert.Equal(["`claim:Roster.Models`, placeholder in `claim:CaptainDispatch.Roster`"], ledger.Split('\n').Where(line => line.Contains("placeholder", StringComparison.Ordinal)));
        Assert.NotEmpty(drops);
        Assert.All(drops, drop => Assert.Matches($@"^`claim:[\w.]+`, dropped, it fails ({string.Join('|', Enum.GetNames<Question>())}): \S", drop));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 W3 a numbered list in the folder counts 1, 2, 3 and holds more than one step: one step alone has no order to number")]
    public void NumberedLists_CountInOrderAndHoldMoreThanOneStep()
    {
        var lists = new List<int>();
        foreach (var line in Files.SelectMany(file => file.Value.Split('\n')))
        {
            var number = Numbered().Match(line) is { Success: true } item ? int.Parse(item.Groups[1].Value) : 0;
            if (number == 1) lists.Add(1);
            else if (number > 1) Assert.Equal(number, lists[lists.Count - 1] += 1);
        }

        Assert.NotEmpty(lists);
        Assert.All(lists, steps => Assert.True(steps > 1, "a numbered list holds one step"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r1b G2 a helper that builds a claim's lines names no actor: each line names its reader where it is written (DYD-1295 317be5f7)")]
    public void LineHelpers_NameNoReader()
    {
        var helpers = Directory.GetFiles(Path.Combine(CommittedTree.Root, "tools/SkillBuild/Claims"), "*.cs")
            .SelectMany(file => LineHelper().Matches(File.ReadAllText(file)).Select(helper => (File: Path.GetFileName(file), Text: helper.Value))).ToArray();

        Assert.NotEmpty(helpers);
        Assert.All(helpers, helper => Assert.False(helper.Text.Contains("Actor.", StringComparison.Ordinal), $"{helper.File}: its line helper names a reader for every line"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r1b P2 H11 a filler ticket is checked before it is set In Progress: the line that sends it to its check stands above the start, and the check's line says what state a failed one goes back in")]
    public void FillerCheck_ComesBeforeTheStart()
    {
        var check = Stated.Single(line => line.Path == Folder + "SKILL.md" && line.Text.Contains($"]({LineOf(Captain.Filler).Path[Folder.Length..]})", StringComparison.Ordinal));

        Assert.True(check.Line < LineOf(Captain.Start).Line, "the spine sets the ticket In Progress before its filler check");
        Assert.Equal(LineOf(Captain.Filler), LineOf(Captain.FillerBack));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r1b P7 the PR that precedes review is proved by the check its line names, which reads no review")]
    public void PrBeforeReview_IsProvedByTheCheckItsLineNames()
    {
        var evidence = CaptainReview.Pr.Truth!.Evidence;

        Assert.Equal(EvidenceKind.Command, evidence.Kind);
        Assert.Contains($"`{evidence.Text}`", LineOf(CaptainReview.Pr).Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r0 9 R34 the level table is the user's, byte for byte the one the legacy reviewer toolset holds")]
    public void LevelTable_IsTheUsersTableAsTheLegacyToolsetHoldsIt()
    {
        var table = Toolset.Matrix.Lines.Single().Text;

        Assert.Equal(table, CaptainReviewing.Matrix.Lines.Single().Text);
        Assert.Contains("\n" + table + "\n", Built.Value.Unwritten[Folder + "resources/review.md"]);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 r0 4 5 W9 a scenario names the tool or command its act uses: t3_thread_send for a Fix, git revert for a red merge")]
    public void Scenarios_NameTheToolTheirActUses()
    {
        Assert.Contains("`t3_thread_send`", Resource("fix-rounds"));
        Assert.Contains("`git revert ", Resource("red-gate"));
    }

    private static readonly Lazy<Dictionary<Claim, string>> Ids = new(() => ModelIndex.ClaimIds(ProductionModel.Model.Kinds));

    private static Dictionary<Claim, string> Claims => Ids.Value;

    private static string Id(Claim claim) => Claims[claim];

    // The one built line of the folder that states a claim.
    private static MapLine LineOf(Claim claim) => LineOf(Id(claim));

    private static MapLine LineOf(string claim) => Stated.Single(line => line.Claims.Contains(claim));

    private static string Resource(string name) => Built.Value.Unwritten[$"{Folder}resources/{name}.md"];

    // A section of the spine: its heading's line through the line before the next "## " heading.
    private static string Section(string heading)
    {
        var start = Spine.IndexOf(heading + "\n", StringComparison.Ordinal);
        Assert.True(start >= 0, $"the spine has no section {heading}");
        var next = Spine.IndexOf("\n## ", start + 1, StringComparison.Ordinal);
        return next < 0 ? Spine[start..] : Spine[start..(next + 1)];
    }
}
