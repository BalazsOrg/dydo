namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 C: the built ticket-captain prototype against the user's rulings on DYD-1295, each an
/// acceptance line of the contract (W1 to W31). Through <see cref="SkillBuilder.Build"/> on the
/// production model, read as the built text a captain loads, never as claims.
/// </summary>
public class TicketCaptainPrototypeTests
{
    private const string Folder = "skills/roles/officers/ticket-captain/";
    private const string Review = "dydo/reference/claim-review/";
    private const string OpenMark = "(open: his call)";

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static string Spine => Built.Value.Files[Folder + "SKILL.md"];

    private static KeyValuePair<string, string>[] Files =>
        [.. Built.Value.Files.Where(file => file.Key.StartsWith(Folder, StringComparison.Ordinal))];

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
    [Trait("Job", "contract: DYD-1372 W26 the worktree sentence he accepted stands verbatim in Dispatch, beside delegate_task (W9)")]
    public void Dispatch_NamesDelegateTaskAndTheWorktreeSentenceVerbatim()
    {
        var dispatch = Section("## 3. Dispatch");

        Assert.Contains("\nDispatch each sub-ticket with `delegate_task`.\n" +
                        "Give a child its own worktree (`isolation: \"worktree\"`) when it writes in parallel with others or will open its own PR; serial work shares yours.\n", dispatch);
        Assert.DoesNotContain("delegate_task", Section("## 1. Take the ticket"));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 W10 W31 the severities are defined in the review phase, in his words (DYD-1295 41a553f1)")]
    public void ReviewPhase_DefinesTheSeveritiesInHisWords() =>
        Assert.Contains("A blocker is fixed now; a should needs fixing, but the fix can wait; a nit is everything else.", Section("## 4. Get it reviewed"));

    [Fact]
    [Trait("Job", "contract: DYD-1372 W11 in the review phase every imperative is the captain's own act: it dispatches, records, sends and repeats, and never reviews (DYD-1295 2119cd65)")]
    public void ReviewPhase_HasTheCaptainDispatchNeverReview()
    {
        var steps = Section("## 4. Get it reviewed").Split('\n').Where(line => line.Length > 2 && char.IsDigit(line[0]) && line[1] == '.').ToArray();

        Assert.Collection(steps,
            step => Assert.StartsWith("1. Once the parts are wired, dispatch a Review ticket", step),
            step => Assert.StartsWith("2. A lone reviewer records its own round; a round of several seats is yours to record", step),
            step => Assert.StartsWith("3. Send a blocker", step),
            step => Assert.StartsWith("4. Send a fundamental blocker", step),
            step => Assert.StartsWith("5. Repeat until a round finds no new blocker", step));
        Assert.DoesNotContain("judge", Spine, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 W24 one general rate-limit line opens When things go wrong, above its scenario links")]
    public void WhenThingsGoWrong_OpensWithOneRateLimitLine()
    {
        var lines = Section("## When things go wrong").Split('\n').Where(line => line.StartsWith("- ", StringComparison.Ordinal)).ToArray();

        Assert.Equal("- On a rate-limit or capacity error, wait, then retry; don't hammer it, and never take a limit to the user.", lines[0]);
        Assert.All(lines.Skip(1), line => Assert.StartsWith("- [", line));
        Assert.Equal(1, Files.Sum(file => file.Value.Split("rate-limit").Length - 1));
    }

    [Theory]
    [Trait("Job", "trap: DYD-1372 W4 W8 W11 W22 W23 W25 W30 no file of the folder says what he ruled out")]
    [InlineData("rule on")]
    [InlineData("authors nothing")]
    [InlineData("review the parent")]
    [InlineData("<KEY>-<slug>")]
    [InlineData("Create the ticket's branch")]
    [InlineData("Codex")]
    [InlineData("429")]
    [InlineData("lands only")]
    [InlineData("the landing")]
    [InlineData("Balazs")]
    public void NoFile_SaysWhatHeRuledOut(string phrase) =>
        Assert.All(Files, file => Assert.DoesNotContain(phrase, file.Value, StringComparison.OrdinalIgnoreCase));

    [Fact]
    [Trait("Job", "contract: DYD-1372 W15 W16 option A: implement, review and research are kind resources the first phase links; a scout is research's native helper")]
    public void KindResources_AreTheThreeTheFirstPhaseLinks()
    {
        var take = Section("## 1. Take the ticket");

        Assert.All(new[] { "implement", "review", "research" }, kind => Assert.Contains($"](resources/{kind}.md) |", take));
        Assert.DoesNotContain(Files, file => file.Key.Contains("scout", StringComparison.Ordinal));
        Assert.Contains("Send one scout per family in play, as native helpers in parallel", Resource("research"));
        Assert.Contains("and the verdict is yours.", Resource("review"));
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
    [Trait("Job", "contract: DYD-1372 E each line that depends on an open point ends with the mark, and only those do")]
    public void LinesOnAnOpenPoint_EndWithTheMark()
    {
        var marked = Files.SelectMany(file => file.Value.Split('\n')).Where(line => line.Contains(OpenMark, StringComparison.Ordinal)).ToArray();

        Assert.All(marked, line => Assert.EndsWith(OpenMark, line));
        Assert.Collection(marked,
            line => Assert.StartsWith("Work with its own review rounds becomes Implement, Review (one per round), Fix and Merge sub-tickets", line),
            line => Assert.StartsWith("3. Send a blocker that is broken, against the spec or a security hole to a Fix ticket", line));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A3c the claims nobody could verify are withheld from the skill and listed")]
    public void OpenClaims_AreWithheldAndListed()
    {
        var open = Built.Value.Files[Review + "ticket-captain-open.md"].Split('\n').Where(line => line.StartsWith("## ", StringComparison.Ordinal));

        Assert.Equal(["## Captain.Thread", "## CaptainDispatch.Roster", "## CaptainDispatch.Usage", "## CaptainMerging.Mutation"], open);
        Assert.All(new[] { "thread's link", "roster", "dydo usage", "waits on mutation" },
            withheld => Assert.All(Files, file => Assert.DoesNotContain(withheld, file.Value, StringComparison.Ordinal)));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A7 the ledger gives every legacy line a fate of its own: none is left to the placeholder, and a drop names its question")]
    public void Ledger_GivesEveryLegacyLineItsFate()
    {
        var ledger = string.Concat(Enumerable.Range(1, 7).Select(part => Built.Value.Files[$"{Review}ticket-captain-ledger-{part}.md"]));
        var entries = ledger.Split('\n').Where(line => line.StartsWith("`claim:", StringComparison.Ordinal) && line.Contains(", ", StringComparison.Ordinal)).ToArray();

        Assert.Equal(76, entries.Length);
        Assert.DoesNotContain("placeholder", ledger);
        Assert.Equal(
            [
                "`claim:Role.CaptainAccounts`, dropped, it fails Verify", "`claim:Role.CaptainTools`, dropped, it fails Need", "`claim:Tree.Branch`, dropped, it fails Handled",
                "`claim:Tree.CrewLimits`, dropped, it fails Handled", "`claim:Dispatch.Foreground`, dropped, it fails Handled",
                "`claim:Dispatch.ParentNotActive`, dropped, it fails Need", "`claim:Dispatch.CodexCap`, dropped, it fails Reason"
            ],
            entries.Where(entry => entry.Contains(", dropped,", StringComparison.Ordinal)).Select(entry => entry[..entry.IndexOf(':', entry.IndexOf("fails", StringComparison.Ordinal))]));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A8 W5 the captain's states are one machine whose arrows are the prototype's own claims")]
    public void CaptainMachine_IsBuiltFromThePrototypesClaims()
    {
        var arrows = Built.Value.Files["claims/statechart/captain/transitions.json"];
        var claims = arrows.Split('\n').Where(line => line.Contains("\"claim\": ", StringComparison.Ordinal)).ToArray();

        Assert.NotEmpty(claims);
        Assert.All(claims, claim => Assert.Contains("\"claim:Captain", claim));
        Assert.All(new[] { "Taken", "Leaf", "Split", "Waiting", "InReview", "Fixing", "Question", "Finishing", "Done", "Returned" },
            state => Assert.Contains($"\"{state}\"", arrows));
        Assert.Contains("VARIABLES state, round", Built.Value.Files["claims/statechart/captain/Captain.tla"]);
    }

    private static string Resource(string name) => Built.Value.Files[$"{Folder}resources/{name}.md"];

    // A section of the spine: its heading's line through the line before the next "## " heading.
    private static string Section(string heading)
    {
        var start = Spine.IndexOf(heading + "\n", StringComparison.Ordinal);
        Assert.True(start >= 0, $"the spine has no section {heading}");
        var next = Spine.IndexOf("\n## ", start + 1, StringComparison.Ordinal);
        return next < 0 ? Spine[start..] : Spine[start..(next + 1)];
    }
}
