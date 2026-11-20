namespace DynaDocs.SkillBuild;

/// <summary>
/// How each scenario resource closes (K9): "save the work, resume, then end resolved (silently, continue) or
/// escalated (up the chain: the officer, then the user only when his attention is needed)". Each line names the
/// scenario's own record and resume point, and its escalation where a rule routes one; otherwise K9's officer.
/// </summary>
public static class Recovery
{
    private const string UpToOfficer = "or escalated to your officer with what you can't resolve.";
    private const string UpAsQuestion = "or escalated as a `research` Question when its captain passes a need up.";

    public static readonly Claim ReCut = new("A re-cut saves the returned work, resumes in the children, and ends staffed or with the ticket sent up as too big.", [Actor.TicketCaptain, Actor.Coordinator],
    [
        new(Skill.TicketCaptain, "Save the returned hop's SHA on it and resume each child from there; end resolved once the children are staffed, or escalated by sending a ticket too big for you to your officer as `too big, re-cut`.", new(ScenarioClose.ReCut, 4001)),
        new(Skill.Coordinator, "Save the `too big, re-cut` on the captain's ticket and resume from the children its Wayfinding captain files; end resolved once they are staffed, " + UpAsQuestion, new(ScenarioClose.ReCut, 4001))
    ])
    { Spec = ["K9", "H2", "H16"] };

    public static readonly Claim FixRounds = new("A fix round saves the verdict's head, resumes the Fix from it, and ends on a clean round or the checkpoint's escalation.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Save the verdict's head SHA on the Review hop and resume Fix rN from it; end resolved when a round finds no new blocker, or escalated only from the checkpoint, written `escalated: …` on the Review.", new(ScenarioClose.FixRounds, 4002))])
    { Spec = ["K9", "R8", "R10"] };

    public static readonly Claim MergeConflict = new("A conflict saves the resolution's commit, resumes the merge from it, and ends merged or escalated.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Save the resolution's commit SHA on the Merge hop and resume the merge from it; end resolved once the hunks pass and the merge runs green, " + UpToOfficer, new(ScenarioClose.MergeConflict, 4003))])
    { Spec = ["K9", "R19"] };

    public static readonly Claim RedGate = new("A red gate saves the merge SHA and the gate, resumes from them, and ends green or escalated.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Save the merge SHA and the red gate on the Merge hop and resume from them; end resolved once the rerun is green or the re-land merges green, " + UpToOfficer, new(ScenarioClose.RedGate, 4004))])
    { Spec = ["K9", "R22"] };

    public static readonly Claim ForcedResync = new("A held PR saves its head and the hold, resumes on its turn, and ends merged or escalated.", [Actor.TicketCaptain, Actor.Coordinator],
    [
        new(Skill.TicketCaptain, "Save the held PR's head SHA on the Merge hop and resume from it on your next turn; end resolved once the PR merges, " + UpToOfficer, new(ScenarioClose.ForcedResync, 4005)),
        new(Skill.Coordinator, "Save the hold on the held PR's ticket and resume pacing once the colliding work is sequenced; end resolved once the PR merges, " + UpAsQuestion, new(ScenarioClose.ForcedResync, 4005))
    ])
    { Spec = ["K9", "R29", "H16"] };

    public static readonly Claim MasterRevert = new("A reverted master merge saves its SHA and the gate, resumes the fix, and ends merged green or escalated.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Save the reverted merge's SHA and its red gate on the Merge hop and resume the fix from them; end resolved once the PR, offered again, merges green, " + UpToOfficer, new(ScenarioClose.MasterRevert, 4006))])
    { Spec = ["K9", "R22"] };

    public static readonly Claim NeedForBalazs = new("A need saves the branch and the need on the ticket, resumes once answered, and ends decided or passed up with the ticket blocked on its Question.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Save your branch's head SHA and the need on your ticket and resume from them once it is answered; end resolved when the call is decided where it arose or answered on its Question, or escalated with the need passed up and your ticket blocked on its Question.", new(ScenarioClose.NeedForBalazs, 4007))])
    { Spec = ["K9", "H16", "H18"] };

    public static readonly Claim Liveness = new("A liveness finding is saved as its comment, the hop resumes from its last posted SHA, and it ends healed or escalated.", [Actor.TicketCaptain, Actor.ChiefOfStaff],
    [
        new(Skill.TicketCaptain, "Save each finding as its `liveness <UTC time>: <outcome>` comment and resume the hop from its last posted SHA; end resolved once the hop is alive, up to date or restarted, or escalated to your officer with a hop you can't restart.", new(ScenarioClose.Liveness, 4008)),
        new(Skill.ChiefOfStaff, "Save each finding as its `liveness <UTC time>: <outcome>` comment and resume the hop from its last posted SHA; end resolved once the hop is alive, up to date or restarted, or escalated, when it needs him, through its coordinator's own envoy, never through you.", new(ScenarioClose.Liveness, 4008))
    ])
    { Spec = ["K9", "S24", "H12"] };

    public static readonly Claim OverLimit = new("A file over the limit saves the work, resumes once its path is settled, and ends split or ignored, or by path (b).", [Actor.TicketCaptain, Actor.Implementer],
    [
        new(Skill.TicketCaptain, "Save the head SHA on the ticket and resume from it once the file's path is settled; end resolved once the file is split or its entry checked, or escalated by path (b), which ends in his blessing.", new(ScenarioClose.OverLimit, 4009)),
        new(Skill.Implementer, "Save your work on the brief's branch and resume from it once the file's path is settled; end resolved once the file is split or its entry is in your PR, or escalated to your captain for path (b).", new(ScenarioClose.OverLimit, 4009))
    ])
    { Spec = ["K9", "T16", "T17", "T18"] };

    public static readonly Claim LandingNotAccepted = new("A landing he does not accept saves the branch's SHA, resumes the re-cut, and ends with another landing or escalated.", [Actor.TicketCaptain],
        [new(Skill.TicketCaptain, "Save the landing branch's SHA on its ticket and resume the re-cut from it; end resolved once another landing follows, " + UpToOfficer, new(ScenarioClose.LandingNotAccepted, 4010))])
    { Spec = ["K9", "N6"] };

    public static readonly Claim Replanning = new("A re-plan saves the blocker on the Wayfinding ticket, resumes watching, and ends sequenced or as a Question.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Save the blocker or requirement on the Wayfinding ticket and resume watching from what it files; end resolved once its re-plan is sequenced, " + UpAsQuestion, new(ScenarioClose.Replanning, 4011))])
    { Spec = ["K9", "R9", "H16"] };

    public static readonly Claim RejectedFiller = new("A filler sent back keeps its reason, resumes once released, and ends released or held by the Wayfinding ticket.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Save the captain's one-line reason on the filler and resume pulling it once its block is released; end resolved when a Wayfinding ticket records the release or he assigns it, or escalated through the Wayfinding ticket that holds the block.", new(ScenarioClose.RejectedFiller, 4012))])
    { Spec = ["K9", "H11"] };

    public static readonly Claim Survivors = new("Survivors are saved on their fix tickets, the landing resumes once they close, and it ends clean or as a Question.", [Actor.Coordinator],
        [new(Skill.Coordinator, "Save each survivor on its fix ticket and resume the landing once every one is closed; end resolved once the mutation check is clean, " + UpAsQuestion, new(ScenarioClose.Survivors, 4013))])
    { Spec = ["K9", "T12", "H16"] };

    public static readonly Claim Survivor = new("A survivor's writer saves the work on the branch, resumes the hop, and ends killed, approved or at a council.", [Actor.Implementer],
        [new(Skill.Implementer, "Save your work on the brief's branch and resume the hop from it; end resolved once the survivor is killed or its claim approved, or escalated to a council when you dispute a rejected claim.", new(ScenarioClose.Survivor, 4014))])
    { Spec = ["K9", "T12"] };

    public static readonly Claim Hunks = new("A hunk review saves its verdict where the captain resumes, and ends on a PASS or escalated to the captain.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Save your verdict on the hop, where your captain resumes the merge from it; end resolved with a PASS naming the new head, or escalated to your captain with what you can't judge.", new(ScenarioClose.Hunks, 4015))])
    { Spec = ["K9", "R19"] };

    public static readonly Claim Equivalent = new("An equivalence ruling is saved on the claim where the mutation work resumes, and ends approved, sent back or at a council.", [Actor.Reviewer],
        [new(Skill.Reviewer, "Save your ruling and its reason on the claim, where the mutation work resumes from it; end resolved with the claim approved or sent back to fix, or escalated to a council when the author disputes it.", new(ScenarioClose.Equivalent, 4016))])
    { Spec = ["K9", "T12"] };

    public static readonly Claim IgnoreEntry = new("An ignore-entry check is saved in the verdict, and ends passed, sent back, or escalated for path (b).", [Actor.Reviewer],
        [new(Skill.Reviewer, "Save your check and its reason in your verdict, where your captain resumes from it; end resolved once the entry passes or goes back to its writer, or escalated to your captain for path (b) when the file is written by hand.", new(ScenarioClose.IgnoreEntry, 4017))])
    { Spec = ["K9", "T17", "T18"] };
}
