namespace DynaDocs.SkillBuild;

/// <summary>Descriptive docs (the Docs machine, DR 072): invite-only, admitted by three tests, declared by explains:, kept true, never authorizing.</summary>
public static class Docs
{
    public static readonly Claim New = Move(DocsState.Start, DocsState.Proposed, "a new descriptive doc is proposed", "D4", "D1");
    public static readonly Claim Existing = Move(DocsState.Start, DocsState.Grandfathered, "an existing descriptive doc is grandfathered", "D7");
    public static readonly Claim Uninvited = Move(DocsState.Proposed, DocsState.NotWritten, "no invitation, or an architecture plan: not written", "D4", "D8");
    public static readonly Claim Invite = Move(DocsState.Proposed, DocsState.Invited, "he invites it", "D4");
    public static readonly Claim Admit = Move(DocsState.Invited, DocsState.Admitted, "solid, important and expensive to rebuild from code", "D3");
    public static readonly Claim Refuse = Move(DocsState.Invited, DocsState.NotWritten, "it fails one of the three tests", "D3");
    public static readonly Claim Declare = Move(DocsState.Admitted, DocsState.Declared, "written, declaring the code it explains in explains:", "D5", "D2");
    public static readonly Claim Flag = Move(DocsState.Declared, DocsState.Flagged, "a change to its paths: dydo check and pr-ready flag it", "D5");
    public static readonly Claim Hold = Move(DocsState.Flagged, DocsState.Declared, "the writer updates the doc or states it holds", "D5");
    public static readonly Claim ReviewFinds = Move(DocsState.Declared, DocsState.Finding, "review finds an unlinked why, or a declared path changed without the doc checked", "D6", "D2");
    public static readonly Claim FixFinding = Move(DocsState.Finding, DocsState.Declared, "the finding is fixed", "D6");
    public static readonly Claim GivePaths = Move(DocsState.Grandfathered, DocsState.Declared, "one mechanical ticket gives it its declared paths", "D7");
    public static readonly Claim FixDrifted = Move(DocsState.Grandfathered, DocsState.Declared, "found drifted or with an unlinked reason: fixed", "D7");
    public static readonly Claim CutDrifted = Move(DocsState.Grandfathered, DocsState.CutDoc, "found drifted or with an unlinked reason: cut", "D7");

    public static readonly Claim Explain = new("A descriptive doc explains and never authorizes; a reason counts only when it links a DR or principle.", [Actor.Implementer],
        [new(Skill.Implementer, "A descriptive doc explains and never authorizes: on what the code does, the code wins; a reason counts only when it links a DR or a principle.", new(DispatchState.Working, 2401, MoveKind.Nominal))])
    { Spec = ["D2"] };

    public static readonly Claim InviteOnly = new("Descriptive docs are invite-only and pass the admission test.", [Actor.Implementer],
        [new(Skill.Implementer, "Write a new descriptive doc only when invited, and only if its subject is solid, important and expensive to rebuild from code.", new(DispatchState.Working, 2402, MoveKind.Nominal))])
    { Spec = ["D4", "D3"] };

    public static readonly Claim Explains = new("A change to a doc's declared paths: the writer updates the doc or states it holds.", [Actor.Implementer],
        [new(Skill.Implementer, "When your change touches the paths a doc's `explains:` declares, update the doc or state that it holds.", new(DispatchState.Working, 2403, MoveKind.Nominal))])
    { Spec = ["D5"] };

    public static readonly Claim DrFields = new("Every new DR carries applies: or new-ground, and weighed: per tension it resolved.", [Actor.Implementer],
        [new(Skill.Implementer, "A new DR you write carries `applies:` (or `new-ground`) and one `weighed: <slug> over <slug>, because <reason>` for each tension it resolved.", new(DispatchState.Working, 2404, MoveKind.Nominal))])
    { Spec = ["L8"] };

    public static readonly Claim Rubric = new("Review gains two lines: an unlinked why is a finding; a change to declared paths means checking that doc.", [Actor.Reviewer],
        [new(Skill.Reviewer, "On docs, an unlinked 'why' is a finding; on code, a change to paths a doc's `explains:` declares means checking that doc.", new(ReviewState.Round, 2405, MoveKind.Nominal))])
    { Spec = ["D6"] };

    private static Claim Move(DocsState from, DocsState to, string arrow, params string[] spec) =>
        new($"A descriptive doc: {arrow}.", [Actor.DydoCheck], []) { Spec = spec, Transition = new(from, to, arrow) };
}
