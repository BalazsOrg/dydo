namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 phase 1, gap hunt 3 (X16, fix g3): the gaps a third walk of the skill found, each as the
/// rule that closes it, read from the production model through <see cref="SkillBuilder.Build"/>. A
/// rule is stated by claim id, path and line order, by a link, by a tool's or a command's own name, or
/// by a word that is Linear's, never by a sentence of the skill (DR 067). Claims are named by id, so
/// this file compiles against the model as it was before the fix, where every test here is red.
/// </summary>
public class TicketCaptainGap3Tests
{
    private const string Folder = "skills/roles/officers/ticket-captain/";
    private const string Spine = Folder + "SKILL.md";
    private const string Resources = Folder + "resources/";

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    private static JsonElement Machine => JsonDocument.Parse(Built.Value.Files["claims/statechart/captain/transitions.json"]).RootElement;

    [Fact]
    [Trait("Job", "contract: DYD-1372 g3 N1 R21 S6 a woken captain dispatches only what is still in Todo and never its Merge: the dispatch line excepts the Merge itself, and a line below it makes the ready Merge the captain's own step, by the merging resource")]
    public void WokenCaptain_NeverDispatchesItsMerge()
    {
        var ready = LineOf("CaptainWaking.Ready");
        var merge = LineOf("CaptainWaking.MergeNext");

        Assert.Equal(Resources + "waking.md", merge.Path);
        Assert.True(merge.Line > ready.Line, "the Merge is made the captain's own step above the line that dispatches what is ready");
        Assert.Contains("](merging.md)", merge.Text);
        Assert.All(new[] { "Todo", "Merge" }, word => Assert.Contains(word, ready.Text));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g3 N2 S6 S9 a brief that names a ticket key new to its reader is a dispatch, whichever thread it arrives in: the first line of phase 1 sorts by the key, and a Fix is briefed by the briefs resource under its own key wherever it is sent")]
    public void NewKeyInAWorkedThread_IsADispatch()
    {
        var sort = LineOf("CaptainIntake.NewKey");
        var fix = LineOf("CaptainTrouble.FixBrief");

        Assert.Equal(Spine, sort.Path);
        Assert.Equal(LineOf("CaptainIntake.Woken"), sort);
        Assert.True(sort.Line < LineOf("CaptainIntake.Check").Line, "the intake check stands above the line that says whose intake it is");
        Assert.Equal(LineOf("CaptainTrouble.FixOwner"), fix);
        Assert.Contains("](briefs.md)", fix.Text);
        Assert.All(new[] { sort, fix }, line => Assert.Contains("key", line.Text));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g3 N3 S6 H2 S10 a ticket returned too big, re-cut is closed by the child that started it: its officer rewrites the contract and sends it to the child's thread, cancels only a ticket with nothing whole, and no line has the officer close it Done")]
    public void ReturnedTicket_IsClosedByTheChildThatStartedIt()
    {
        var narrow = LineOf("CaptainTrouble.Narrow");
        var handed = LineOf("CaptainTrouble.Handed");

        Assert.Equal(Resources + "too-big.md", handed.Path);
        Assert.True(narrow.Line < handed.Line && handed.Line < LineOf("CaptainTrouble.ChildReCut").Line, "the contract is sent before it is rewritten, or after what is left is cut");
        Assert.Contains("`t3_thread_send`", handed.Text);
        Assert.All(new[] { handed, LineOf("CaptainTrouble.Left") }, line => Assert.Contains("Done", line.Text));
        Assert.DoesNotContain("Done", narrow.Text);
        Assert.Contains("cancel", LineOf("CaptainTrouble.Settle").Text, StringComparison.OrdinalIgnoreCase);
        // A return ends a holder's run: the child's close is a second run, and the machine keeps Returned final.
        Assert.Equal(["Returned"], Machine.GetProperty("finals").EnumerateArray().Select(final => final.GetString()));
        Assert.DoesNotContain(Machine.GetProperty("transitions").EnumerateArray(), move => move.GetProperty("from").GetString() == "Returned");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g3 N4 H23 H16 the wayfinder skill is loaded only for how to find the way: the line that has it loaded opens the wayfinding resource, above its first step, and the spine's fog line still leads there")]
    public void Wayfinder_IsLoadedOnlyForFindingTheWay()
    {
        var scope = LineOf("CaptainWay.Scope");

        Assert.Equal(Resources + "wayfind.md", scope.Path);
        Assert.Contains("`/wayfinder`", scope.Text);
        Assert.True(scope.Line < LineOf("CaptainWay.Visible").Line, "a part is cut before the wayfinder's reach is said");
        Assert.Contains($"](resources/{Path.GetFileName(scope.Path)})", LineOf("CaptainWay.Fog").Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g3 N5 S3 R2 a Fix or Review opened after the Merge is made to block it before anything newly ready is dispatched: the act stands in the waking resource, below the cancel and above the dispatch, with Linear's own field")]
    public void LaterFixOrReview_BlocksTheMergeBeforeTheDispatch()
    {
        var wired = LineOf("CaptainWaking.Wired");
        var ready = LineOf("CaptainWaking.Ready");

        Assert.Equal(ready.Path, wired.Path);
        Assert.True(LineOf("CaptainWaking.Unneeded").Line < wired.Line && wired.Line < ready.Line, "a step is wired after what it unblocked is dispatched, or before the unneeded one is cancelled");
        Assert.All(new[] { "`save_issue`", "`blocks`", "Merge" }, word => Assert.Contains(word, wired.Text));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g3 N6 H12 a holder finds its officer's thread where it first sends there: the step that sends names the tool that shows it, and the folder names that tool once")]
    public void OfficersThread_IsFoundWhereTheHolderSends()
    {
        var address = LineOf("CaptainNeed.Address");

        Assert.Equal(LineOf("CaptainNeed.Up"), address);
        Assert.All(new[] { "`t3_thread_list`", "`t3_thread_send`" }, tool => Assert.Contains(tool, address.Text));
        Assert.Equal(1, Built.Value.Files.Where(file => file.Key.StartsWith(Folder, StringComparison.Ordinal)).Sum(file => file.Value.Split("t3_thread_list").Length - 1));
        Assert.Contains("](need-for-the-user.md)", LineOf("CaptainIntake.Lacking").Text);
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g3 N7 R2 K14 a part built in its own worktree enters the whole by its officer's merge of the returned SHA, where the return is handled and before anything is dispatched; and the brief of a child that merges names the branch its PR targets")]
    public void ReturnedPart_EntersTheWholeByItsOfficersMerge()
    {
        var part = LineOf("CaptainWaking.Part");
        var target = LineOf("CaptainBriefs.Target");

        Assert.Equal(Resources + "waking.md", part.Path);
        Assert.True(LineOf("CaptainDispatch.Resume").Line < part.Line && part.Line < LineOf("CaptainWaking.Ready").Line, "a part is merged before its SHA is posted, or after the next dispatch");
        Assert.Contains("`git merge <SHA>`", part.Text);
        Assert.Equal(LineOf("CaptainBriefs.Merger"), target);
        Assert.All(new[] { "PR", "`dydo merge`" }, word => Assert.Contains(word, target.Text));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g3 walk R8 S9 a return is taken in before its holder goes on: the returned commit is merged above the line that resumes the phase, which stands above the cancel, the wiring and the dispatch, so the next round judges a head that holds the fix")]
    public void Return_IsTakenInBeforeTheHolderGoesOn()
    {
        var on = LineOf("CaptainWaking.GoOn");
        var before = new[] { "CaptainWaking.Reread", "CaptainDispatch.Resume", "CaptainWaking.Part", "CaptainDispatch.Route" }.Select(LineOf).ToArray();
        var after = new[] { "CaptainWaking.Unneeded", "CaptainWaking.Wired", "CaptainWaking.Ready", "CaptainWaking.MergeNext" }.Select(LineOf).ToArray();

        Assert.All(before.Concat(after), step => Assert.Equal(on.Path, step.Path));
        Assert.True(before.Max(step => step.Line) < on.Line, "the holder goes on before the return is taken in");
        Assert.True(on.Line < after.Min(step => step.Line), "a step is cancelled, wired or dispatched before the phase has said what the return asks for");
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 g3 walk S9 K14 a Fix starts from the head that holds the reviewed candidate: its brief names that SHA beside its findings, and the Fix recipe opens by having its writer bring its own checkout up to it")]
    public void Fix_StartsFromTheShaItsBriefNames()
    {
        var from = LineOf("CaptainImplement.From");
        var named = LineOf("CaptainBriefs.FixBase");

        Assert.Equal(LineOf("CaptainBriefs.Change"), named);
        Assert.Contains("SHA", named.Text);
        Assert.Equal(Resources + "implement.md", from.Path);
        Assert.True(from.Line < LineOf("CaptainImplement.Source").Line, "a finding is checked against the source before the writer stands on the commit it was found at");
        Assert.Contains("`git merge <SHA>`", from.Text);
    }

    // The built line of the folder that states a claim; a table's is its first.
    private static MapLine LineOf(string claim)
    {
        var stated = Built.Value.Map.Where(line => line.Path.StartsWith(Folder, StringComparison.Ordinal) && line.Claims.Contains(claim)).MinBy(line => line.Line);
        Assert.True(stated is not null, $"no line of the skill states claim:{claim}");
        return stated!;
    }
}
