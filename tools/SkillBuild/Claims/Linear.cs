namespace DynaDocs.SkillBuild;

/// <summary>The Linear workspace (K12): its statuses, labels, the ticket, the cut rule, the continuous pair and Priority.</summary>
public static class Linear
{
    public static readonly Claim Purpose = new("The Linear workspace standard is built from the same claims as the role skills.", [Actor.SkillBuild],
        [new(Skill.LinearWorkspaceStandard, "The Linear vocabulary for Projects and tickets; every rule here is a claim, built from the same model as the role skills.", new(Section.LinearIntro, 1))])
    { Spec = ["K12"] };

    public static readonly Claim Statuses = new("Seven statuses; the old three are deleted at ship and Sub-issue auto-close stays off.", [Actor.Linear],
        [new(Skill.LinearWorkspaceStandard, "Seven statuses only: FutureFeature, Backlog, Todo, In Progress, Done, Canceled, Duplicate. The Enablement at ship deletes `Implementing`, `In Review` and `Ready to Merge`, and Sub-issue auto-close stays off.", new(Section.LinearSetup, 1))])
    { Spec = ["S5", "N9", "S18"] };

    public static readonly Claim ContinuousPair = new("Every project owns a continuous pair, CI findings and Agent friction, that nobody oversees.", [Actor.Linear],
        [new(Skill.LinearWorkspaceStandard, "Each project (team) gets its continuous pair, `CI findings` and `Agent friction`: a Linear grouping of tickets, not a Project with an outcome; nobody oversees it or sends envoys for it.", new(Section.LinearSetup, 2))])
    { Spec = ["H8"] };

    public static readonly Claim Mode = new("The Mode label says who the work waits on.", [Actor.Linear],
        [new(Skill.LinearWorkspaceStandard, "The Mode label, AFK or HITL, says who the work waits on.", new(Section.LinearLabels, 2))])
    { Spec = ["S6"] };

    public static readonly Claim CutRule = new("The cut rule: a ticket only when someone other than the asker uses what the work leaves.", [Actor.Admiral, Actor.TicketCaptain],
    [
        new(Skill.Admiral, "Cut a ticket only when someone other than its asker uses what the work leaves; a native helper, a scout, a lookup, one seat among several, a command or a file stays off the record.", new(ProjectState.Watching, 2006, MoveKind.Nominal)),
        new(Skill.TicketCaptain, "Cut a ticket only when someone other than its asker uses what the work leaves; a native helper, a scout, a lookup, one seat among several, a command or a file stays off the record.", new(HopsState.Cut, 2006, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "A ticket exists only when someone other than its asker uses what the work leaves; a native helper, a scout, a lookup, one seat among several, a command or a file stays off the record.", new(Section.LinearTickets, 2))
    ])
    { Spec = ["S1"] };

    public static readonly Claim Priority = new("Priority marks which of his tickets he takes next.", [Actor.Admiral],
    [
        new(Skill.Admiral, "Set Priority on every ticket waiting on him, a Question or a HITL ticket, and re-set it as blockers change: High when clearing it lets a top-level ticket or the Project's next step run AFK, Medium when it frees a lane, Low when nothing runs AFK after it; Urgent only in an emergency; AFK tickets carry none.", new(ProjectState.Watching, 2007, MoveKind.Nominal)),
        new(Skill.LinearWorkspaceStandard, "Priority marks which of his tickets he takes next, so the one unlocking the most AFK work goes first: High when clearing it lets a top-level ticket or the Project's next step run AFK, Medium when it frees a lane, Low when nothing runs AFK after it; Urgent is an emergency, never a default; AFK tickets carry none.", new(Section.LinearPriority, 1))
    ])
    { Spec = ["N12 DR 047 §11"] };
}
