namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>linear</c> claims, sub-topics ticket to writer.</summary>
public static class LinearTicketToWriter
{
    public static readonly Claim TicketBodyOneOutcomeOneSession = new(
        "A ticket's body carries one outcome sized to one agent session.",
        [A.AnyAgent],
        [
            new(S.Wayfinder, "Its body carries one outcome sized to one agent session.", new(F.WayfinderTickets, 2))
        ]);

    public static readonly Claim TicketTemplateFromBody = new(
        "Create a Linear ticket template from the body given where the connected tools or UI permit.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Create it from the body below where the connected tools or UI permit.", new(F.LinearWorkspaceStandardTicketTemplates, 1))
        ]);

    public static readonly Claim TypeBug = new(
        "Type Bug is held by a captain at any level: restore intended behaviour; it closes on the behaviour restored.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Bug` | captain | any | Restore intended behaviour. The record holds the defect and its fix. | the behaviour restored | `#EB5757` |", new(F.LinearWorkspaceStandardType, 2))
        ]);

    public static readonly Claim TypeEnablement = new(
        "Type Enablement is held by a captain at any level: access, environment, credentials or material other work needs; it closes on the condition true, with evidence.",
        [A.TicketCaptain], []);

    public static readonly Claim TypeFeature = new(
        "Type Feature is held by a captain at any level: add, improve, refactor or document an outcome to build; it closes on the outcome merged.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Feature` | captain | any | Add, improve, refactor or document: an outcome to build. | the outcome merged | `#BB87FC` |", new(F.LinearWorkspaceStandardType, 1))
        ]);

    public static readonly Claim TypeGrilling = new(
        "Type Grilling is held by the map holder at any level: a tree of intent or specification choices resolved with the user; it closes on shared understanding recorded, with its Decision Records linked.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Grilling` | map holder | any | A tree of intent or specification choices resolved with the user. | shared understanding recorded, with its Decision Records linked | `#D4A017` |", new(F.LinearWorkspaceStandardType, 9))
        ]);

    public static readonly Claim TypeInquisition = new(
        "Type Inquisition is held by a captain on a primary ticket, with group Sub-tickets above about 10 areas: one round of audit of the integrated feature, each round starting from zero.",
        [A.TicketCaptain], []);

    public static readonly Claim TypeMerge = new(
        "Type Merge is held by a captain at any level (the landing is the only primary one): one merge operation; it closes on the merge review PASS.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Merge` | captain | any; the landing is the only primary one | One merge operation: lanes into a parent, a primary into the feature, the feature into main. | the merge review PASS | `#4EA7FC` |", new(F.LinearWorkspaceStandardType, 3))
        ]);

    public static readonly Claim TypePrototype = new(
        "Type Prototype is held by a captain at any level: a design question raised to fidelity the user can react to; it closes on the user's verdict on the ticket.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Prototype` | captain | any | A design question raised to fidelity the user can react to; fast sketches, the user is the review. | the user's verdict on the ticket | `#F2994A` |", new(F.LinearWorkspaceStandardType, 6))
        ]);

    public static readonly Claim TypeQuestion = new(
        "Type Question is held by the map holder at any level: one prepared, discrete question whose answer blocks named work; it closes on the user's answer on the ticket.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Question` | map holder | any | One prepared, discrete question whose answer blocks named work. | the user's answer on the ticket | `#F2C94C` |", new(F.LinearWorkspaceStandardType, 7))
        ]);

    public static readonly Claim TypeResearch = new(
        "Type Research is held by the map holder at any level: a factual answer whose investigation needs its own owner, status or evidence; it closes on cited findings on the ticket.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Research` | map holder | any | A factual answer whose investigation needs its own owner, status or evidence. | cited findings on the ticket | `#95A2B3` |", new(F.LinearWorkspaceStandardType, 8))
        ]);

    public static readonly Claim TypeWalkthrough = new(
        "Type Walkthrough is held by the map holder on a primary ticket only: the user walks the whole product as it stands; it closes on the user's verdict quoted or skip recorded, findings filed as tickets.",
        [A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "| `Walkthrough` | map holder | primary only | The user walks the whole product as it stands: the feature branch at a Milestone, or main at the landing. The emphasis is on what changed since the last walkthrough and what needs the most careful look. The tour also covers where to look, how to try it, and what reviewers left open. There's one per Milestone, prepared even if he skips it, and the landing's comes last. | his verdict quoted on the ticket, or his skip recorded; findings filed as tickets | `#C69C6D` |", new(F.LinearWorkspaceStandardType, 10), () => [Walkthrough.BriefFourParts, Walkthrough.OneTicketPerMilestone])
        ]);

    public static readonly Claim UseAvailableCapabilities = new(
        "Agents read the standard and the ticket contract, then use the capabilities actually available; template listing or retrieval is not a required connector capability.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Agents read this standard and the ticket contract, then use the capabilities actually available; template listing or retrieval is not a required connector capability.", new(F.LinearWorkspaceStandardTicketTemplates, 2))
        ]);

    public static readonly Claim UserGateApprovalNotHitl = new(
        "The user's approval or inspection at a universal gate does not make a ticket HITL.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "The user's approval or inspection at a universal gate does not make a ticket HITL.", new(F.LinearWorkspaceStandardMode, 3))
        ]);

    public static readonly Claim WaiterPrioritySetByStandard = new(
        "Priority is set on every ticket that waits on the user (a Question or a HITL ticket) by the standard's guide and re-set as blockers change.",
        [A.TicketCaptain, A.Admiral, A.MapHolder],
        [
            new(S.LinearWorkspaceStandard, "The map holder sets it on every ticket that waits on the user, a `Question` or a HITL ticket, and re-sets it as blockers change.", new(F.LinearWorkspaceStandardPriority, 1)),
            new(S.TicketCaptain, "Set priority by the standard: the user's next pick is the answer that frees the most AFK work.", new(F.TicketCaptainRelease, 4))
        ]);

    public static readonly Claim WithdrawOnlyObsoleteQuestions = new(
        "Withdraw only Questions made obsolete by discovered facts or completed work; keep tribunal-decided Questions awaiting ratification.",
        [A.Admiral],
        [
            new(S.OvernightMode, "Withdraw only Questions made obsolete by discovered facts or completed work; keep tribunal-decided Questions awaiting ratification.", new(F.OvernightModeStandingOrders, 12))
        ]);

    public static readonly Claim WriteForReaderWhoWasntThere = new(
        "Write every Question and DECIDED for a reader who was not there.",
        [A.Admiral],
        [
            new(S.OvernightMode, "**Write for a reader who wasn't there**, every Question and `DECIDED`.", new(F.OvernightModeStandingOrders, 11))
        ]);

    public static readonly Claim WriterPrecodeCommentOnlyWhenSplit = new(
        "Before code, a code-writer posts one comment only when the work splits into disjoint lanes or the contract is inexact, then stops.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "Before code, post only when the work splits into disjoint lanes or the contract is inexact: one comment naming them, then stop.", new(F.CodeWriterReturn, 1))
        ]);
}
