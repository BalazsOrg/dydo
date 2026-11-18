namespace DynaDocs.Tests.SkillBuild;

using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Charts;

/// <summary>
/// The chart files a model's machines build (DYD-1082 contract, "Statechart model"; K1, K3, K7):
/// the lease fixture's module, config and view byte for byte, and a machine with no transitions or
/// variables. <c>transitions.json</c> is <see cref="TransitionsJsonTests"/>'.
/// </summary>
public class StatechartFilesTests
{
    private const string Folder = "claims/statechart/lease/";

    private static readonly BuiltTree Lease = SkillBuilder.Build(ChartModel.With());

    [Fact]
    [Trait("Job", "contract: K1/K3 a machine's module has one action per transition and gap, a Done stutter, and no property")]
    public void LeaseModule_IsTheMachineAsTla() => Assert.Equal(Text("""
        \* built from the model; edit there: tools/SkillBuild
        ---- MODULE Lease ----
        EXTENDS Naturals

        VARIABLES state, fixes

        vars == <<state, fixes>>

        Finals == {"Done"}

        Open == {"GAP 2"}

        Init ==
            /\ state = "Free"
            /\ fixes = 0

        \* claim:LeaseMoves.Fail
        LeaseMoves_Fail ==
            /\ state = "Review"
            /\ fixes < 2
            /\ state' = "Held"
            /\ fixes' = fixes + 1

        \* claim:LeaseMoves.Land
        LeaseMoves_Land ==
            /\ state = "Review"
            /\ state' = "Done"
            /\ UNCHANGED <<fixes>>

        \* claim:LeaseMoves.Return
        LeaseMoves_Return ==
            /\ state = "Held"
            /\ state' = "Review"
            /\ UNCHANGED <<fixes>>

        \* claim:LeaseMoves.Take
        LeaseMoves_Take ==
            /\ state = "Free"
            /\ state' = "Held"
            /\ UNCHANGED <<fixes>>

        \* GAP: the spec names no release of a held lease
        Gap1 ==
            /\ state = "Held"
            /\ state' = "Free"
            /\ UNCHANGED <<fixes>>

        \* GAP: the spec names no move after a second failed fix
        Gap2 ==
            /\ state = "Review"
            /\ state' = "GAP 2"
            /\ UNCHANGED <<fixes>>

        Done == state \in Finals \cup Open /\ UNCHANGED vars

        Next ==
            \/ LeaseMoves_Fail
            \/ LeaseMoves_Land
            \/ LeaseMoves_Return
            \/ LeaseMoves_Take
            \/ Gap1
            \/ Gap2
            \/ Done

        Bounds == fixes <= 2

        Spec == Init /\ [][Next]_vars
        ====
        """), Lease.Files[Folder + "Lease.tla"]);

    [Fact]
    [Trait("Job", "contract: K1 a machine's config names Init and Next, and its bounds as the CONSTRAINT")]
    public void LeaseConfig_BoundsTheVariables() => Assert.Equal(Text("""
        \* built from the model; edit there: tools/SkillBuild
        INIT Init
        NEXT Next
        CONSTRAINT Bounds
        """), Lease.Files[Folder + "Lease.cfg"]);

    [Fact]
    [Trait("Job", "contract: K1 a machine's view labels states and steps with their spec ids, gaps as GAP, and hides Done")]
    public void LeaseView_LabelsWithTheSpec() => Assert.Equal(Text("""
        // built from the model; edit there: tools/SkillBuild
        const states = {
          "Free": "Free · K1",
          "Held": "Held · K9",
          "Review": "In review · R1",
          "Done": "Done · R2, N12 DR 049 §2–4",
          "GAP 2": "GAP: unknown state"
        };
        const steps = {
          "LeaseMoves_Fail": "the review fails · R3",
          "LeaseMoves_Land": "the merge lands · R2, CA19",
          "LeaseMoves_Return": "the holder returns it · R1, K28",
          "LeaseMoves_Take": "the holder takes the lease · K9",
          "Gap1": "GAP: the spec names no release of a held lease",
          "Gap2": "GAP: the spec names no move after a second failed fix"
        };
        export const title = "Lease";
        export const state = (v) => states[v.state];
        export const step = (action) => (action === "Done" ? null : steps[action]);
        """), Lease.Files[Folder + "view.mjs"]);

    [Fact]
    public void MachineWithNoMovesOrVariables_BuildsABareModule()
    {
        var files = SkillBuilder.Build(new Model([], []) { Machines = [ChartModel.Other] }).Files;

        Assert.Equal(Text("""
            \* built from the model; edit there: tools/SkillBuild
            ---- MODULE Other ----

            VARIABLES state

            vars == <<state>>

            Finals == {}

            Init ==
                /\ state = "Idle"

            Done == state \in Finals /\ UNCHANGED vars

            Next ==
                \/ Done

            Spec == Init /\ [][Next]_vars
            ====
            """), files["claims/statechart/other/Other.tla"]);
        Assert.Equal(Text("""
            \* built from the model; edit there: tools/SkillBuild
            INIT Init
            NEXT Next
            """), files["claims/statechart/other/Other.cfg"]);
    }

    [Fact]
    [Trait("Job", "contract: K6 a model with no machines builds no chart file")]
    public void ModelWithNoMachines_BuildsNoChartFile() =>
        Assert.DoesNotContain(SkillBuilder.Build(new Model([], [])).Files.Keys, path => path.StartsWith("claims/", StringComparison.Ordinal));

    [Fact]
    [Trait("Job", "contract: K7 the chart files a root holds read back as the build, so the golden test covers them")]
    public void ChartFiles_WrittenToARoot_ReadBackAsTheBuild()
    {
        using var root = new TempRoot();

        Assert.Equal(0, root.Run(ChartModel.With()).Exit);

        Assert.Equal("", GoldenDiff.Describe(Lease.Bytes(), BuiltHeader.Committed(root.Path)));
        Assert.Contains("claims/statechart/lease/transitions.json", BuiltHeader.Committed(root.Path).Keys);
    }

    [Fact]
    [Trait("Job", "trap: K7 a hand edit of a chart file fails the golden test, naming it")]
    public void ChartFile_WithAHandEdit_FailsTheGoldenTest()
    {
        using var root = new TempRoot();
        root.Run(ChartModel.With());
        File.AppendAllText(root[Folder + "Lease.tla"], "\\* edited\n");

        Assert.Contains($"changed: {Folder}Lease.tla line ", GoldenDiff.Describe(Lease.Bytes(), BuiltHeader.Committed(root.Path)));
    }

    internal static string Text(string raw) => raw.ReplaceLineEndings("\n") + "\n";
}
