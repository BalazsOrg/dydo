namespace DynaDocs.Tests.SkillBuild.Fixture;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>inquisition</c> claims, sub-topics record to work.</summary>
public static class InquisitionRecordToWork
{
    public static readonly Claim RecordContent = new(
        "The inquisition record is written at dydo/project/inquisitions/<YYYY-MM-DD>-<slug>.md from the pushed register, naming the audit SHA, the commission, the area map with each phase run or its reason, every finding with severity, sightings and disposition, the Bugs and patches filed, the nit ticket and the Questions, an empty section recorded as such.",
        [A.TicketCaptain, A.DocsWriter],
        [
            new(S.DocsWriter, "Work on the Inquisition's own ticket branch: a document at `dydo/project/inquisitions/<YYYY-MM-DD>-<slug>.md` naming the audit SHA, the commission, the area map with each phase run or its reason, every finding with its severity, sightings and disposition, the Bugs and patches filed with their proof, the nit ticket and the Questions.", new(F.DocsWriterMethod, 9)),
            new(S.TicketCaptain, "Brief a `docs-writer` with the pushed register: it writes `dydo/project/inquisitions/<YYYY-MM-DD>-<slug>.md` naming the audit SHA, the commission, the area map with each phase run or its reason, every finding with its severity, sightings and disposition, every unreachable journey with its disposition, the Bugs and patches filed, the nit ticket and the Questions, an empty section recorded as such.", new(F.TicketCaptainInquisitionN11DeliverTheRecord, 1))
        ]);

    public static readonly Claim RecordDeliveredThroughDocsChain = new(
        "The Inquisition delivers its own record as its last step through a docs-writer, a fresh docs review and its Merge Sub-ticket on its own ticket branch <KEY>-<slug> cut from the feature tip, and closes once.",
        [A.TicketCaptain, A.DocsWriter],
        [
            new(S.TicketCaptain, "An Inquisition's phases, proofs and patches stay `In Progress`; its own record then runs the normal docs delivery chain, as below.", new(F.TicketCaptainMethod, 21)),
            new(S.WorkingTreeStandard, "| Inquisition record | the feature tip; `main` for an audit of main | the Inquisition's `DYD-123-<slug>` | its feature branch; `main` for an audit of main | its captain-directed Merge Sub-ticket; for an audit of main, as an atomic ticket |", new(F.WorkingTreeStandardBranchesAndTargets, 8))
        ]);

    public static readonly Claim RecordDeliveryDoneOnMergePass = new(
        "Inquisition record delivery is done when the record's merge review passes.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Done when the record's merge review passes.", new(F.TicketCaptainInquisitionN11DeliverTheRecord, 2))
        ]);

    public static readonly Claim RecordDoneCriteria = new(
        "The inquisition record is done when every claim traces to the register, an empty section explicitly recorded as such.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "Done when every claim traces to the register, with an empty section explicitly recorded as such; return the exact path and blob or content digest for the docs review and the captain's check after the merge.", new(F.DocsWriterMethod, 10), () => [ReturnAdmiralToNo.DocsInquisitionRecord])
        ]);

    public static readonly Claim RecordWrittenOnHandover = new(
        "When the Inquisition captain hands over its pushed findings register, the docs-writer writes the inquisition record.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "**Write the inquisition record** when an Inquisition's captain hands you its pushed findings register.", new(F.DocsWriterMethod, 8))
        ]);

    public static readonly Claim RefutedClosesInconclusiveStays = new(
        "A refuted finding closes refuted; an inconclusive one stays in the record with its deciding observation.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Unproven:** stays open in the register until its routed phase returns; a refuted one closes `refuted`, an inconclusive one stays in the record with its deciding observation.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 12), () => [InquisitionRecordToWork.UnprovenStaysOpen!])
        ]);

    public static readonly Claim RegisterFileAtRoot = new(
        "The captain creates inquisition-register.md at the root of the audit branch, one row per finding in the register table shape.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Create the findings register (step 5) on it and push.", new(F.TicketCaptainInquisitionN2OpenTheAuditBranch, 1))
        ]);

    public static readonly Claim RegisterPushAfterReturn = new(
        "The register is committed and pushed after every return, before the next dispatch; the register step is done when every return is in it and pushed.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Commit and push the register after every return, before the next dispatch.", new(F.TicketCaptainInquisitionN5KeepTheFindingsRegister, 4)),
            new(S.WorkingTreeStandard, "The audit branch carries the findings register, `inquisition-register.md`, pushed after every return.", new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, 3))
        ]);

    public static readonly Claim ReleaseBeforeGenericCommissioning = new(
        "The admiral processes an Inquisition release through step 7 before generic pickable commissioning.",
        [A.Admiral],
        [
            new(S.Admiral, "Process an Inquisition's release through step 7 before generic pickable commissioning.", new(F.AdmiralMethod, 5))
        ]);

    public static readonly Claim RetainReproductionRef = new(
        "Before the sweep closes, every open Bug with a reproduction commit keeps its SHA reachable from a pushed named ref independent of the audit branch (its proof or patch branch), linked on the Bug; retention is done when all phases returned, every row has a final disposition and register and proof branches are pushed.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Retain each open Bug's reproduction commit on a pushed independent ref, by **Retaining an Inquisition's record and proofs** in the working-tree standard.", new(F.TicketCaptainKindsAndFailurePaths, 12)),
            new(S.WorkingTreeStandard, "Before the sweep closes, every open Bug with a reproduction commit keeps its SHA reachable from a pushed named ref independent of the audit branch, linked on that Bug: its proof branch, or its patch branch when a fix was prepared.", new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, 4))
        ]);

    public static readonly Claim RetentionOwnership = new(
        "The Inquisition captain owns reproduction retention until each Bug captain records adopting it.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "You own that retention until each Bug's captain records adopting it; audit cleanup never deletes those refs.", new(F.TicketCaptainInquisitionN10CloseTheSweep, 1), () => [WorktreeAuditToReviewer.AuditCleanupNeverDeletesRetainedRefs]),
            new(S.WorkingTreeStandard, "The Inquisition captain owns that retention until each Bug captain records adopting the reproduction and the cleanup responsibility.", new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, 6))
        ]);

    public static readonly Claim RoundsStartFromZero = new(
        "Each Inquisition round starts from zero, independent of the previous round's map and findings.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.TicketCaptain, "Each round is its own Inquisition, started from zero: you never read the previous round's map or findings.", new(F.TicketCaptainInquisitionLead, 3))
        ]);

    public static readonly Claim RouteTaggedFinding = new(
        "The captain routes a tagged finding to the phase that can prove it (testable to Break, drivable to Drive), by a short follow-up agent with only the routed entries when the phase already ran; triage intake is done when every tag has a phase scheduled.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Tagged finding:** route it to the phase that can prove it, `testable` to Break and `drivable` to Drive.", new(F.TicketCaptainInquisitionN7RouteEachReturn, 2))
        ]);

    public static readonly Claim RunByResourcePipeline = new(
        "The captain runs an Inquisition by resources/inquisition.md: map the landed work into small areas, run Read, Break and Drive on each through inquisitor crew, triage every return, then deliver the record.",
        [A.TicketCaptain], []);

    public static readonly Claim SeamIsOwnArea = new(
        "Where two areas meet through a call, a shared type, a file format or a protocol, the seam is its own area.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Seams:** where two areas meet through a call, a shared type, a file format or a protocol, the seam is its own area.", new(F.TicketCaptainInquisitionN3DrawTheAreaMap, 5))
        ]);

    public static readonly Claim SeverityCheckedAgainstSource = new(
        "Each finding severity is checked against the source, not the crew label, and any change is recorded with its reason in the row.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Check each severity against the source, not the crew's label, and record any change with its reason in the row.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 1))
        ]);

    public static readonly Claim ShouldDeferredByUserOnly = new(
        "Only the user defers an inquisition should, at the end of the Project.",
        [A.User],
        [
            new(S.Admiral, "Only the user defers an inquisition's should, at the end of the Project.", new(F.AdmiralMethod, 65))
        ]);

    public static readonly Claim SightingAddsReproduction = new(
        "A later sighting of a Bug, even a fixed one, adds a reproduction to it.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A later sighting of a Bug, even a fixed one, adds a reproduction to it.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 7))
        ]);

    public static readonly Claim SightingNotNewRow = new(
        "A finding matching an existing register row is a sighting on that row, never a new row: one bug, one finding, the sighting added with where seen and proof under the register ID.",
        [A.TicketCaptain, A.Inquisitor],
        [
            new(S.Inquisitor, "**One bug, one finding.** A bug already in your area's register entries is a sighting: add where you saw it and your proof, under its register ID.", new(F.InquisitorCalibration, 3)),
            new(S.TicketCaptain, "**Known bug:** a finding matching a register row is a sighting on that row, never a new row.", new(F.TicketCaptainInquisitionN7RouteEachReturn, 1))
        ]);

    public static readonly Claim StartOnUserConfirmation = new(
        "An Inquisition starts only after the user confirms it; the confirmation moves the ticket on and the captain sets it In Progress.",
        [A.TicketCaptain, A.Admiral, A.User],
        [
            new(S.Admiral, "**Done:** the user moves it to `Todo` and tells you, or cancels it; commission the confirmed ticket with the answers on it.", new(F.AdmiralMethod, 58))
        ]);

    public static readonly Claim StatusPath = new(
        "An Inquisition ticket moves Backlog, Todo (the user confirmation), In Progress (commission inputs, area map, phases, triage, patch lane), Implementing, In Review, Ready to Merge, Done, the last stages being its record docs chain.",
        [A.TicketCaptain],
        [
            new(S.LinearWorkspaceStandard, "| `Inquisition` | `Backlog` → `Todo`, the user's confirmation → `In Progress`, the commission inputs, area map, phases, triage and patch lane → `Implementing` → `In Review` → `Ready to Merge` → `Done`, its record's docs chain. It releases to `Todo` with `released <key>: verification skill first`, resuming once the skill exists, or with `released <key>: groups <keys>`, resuming `In Progress` once its group Sub-tickets, each `Todo` → `In Progress` → `Done`, close; follow the [working-tree standard](../guides/working-tree-standard.md#retaining-an-inquisitions-record-and-proofs) |", new(F.LinearWorkspaceStandardWhoHoldsWhat, 6), () => [Release.InquisitionTwoReleaseCases, WorktreeAuditToReviewer.InquisitionFollowsRetentionRules])
        ]);

    public static readonly Claim TagUnprovenFindings = new(
        "A finding seen but not provable in the phase is tagged testable (for Break), drivable (for Drive) or both, and stays tagged until a phase proves or refutes it.",
        [A.TicketCaptain, A.Inquisitor],
        [
            new(S.Inquisitor, "A finding you see but cannot prove in your phase gets a tag: `testable` for Break, `drivable` for Drive. Tag it and move on.", new(F.InquisitorProof, 7)),
            new(S.TicketCaptain, "Tags are `testable` and `drivable` until a phase proves or refutes the finding.", new(F.TicketCaptainInquisitionN5KeepTheFindingsRegister, 2))
        ]);

    public static readonly Claim TriageDone = new(
        "Triage is done when every register row has a disposition or a routed phase in flight.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Done when every register row has a disposition or a routed phase in flight.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 14))
        ]);

    public static readonly Claim UnprovenStaysOpen = new(
        "An unproven finding stays open in the register until its routed phase returns.",
        [A.TicketCaptain], []);

    public static readonly Claim UnreachableJourney = new(
        "An unreachable journey is a register row with its prerequisite and the route tried, disposed of before that area Drive counts as returned: a follow-up Drive once the prerequisite is met, a verification-gap should, or a Question.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Unreachable journey:** a register row with its prerequisite and the route tried.", new(F.TicketCaptainInquisitionN7RouteEachReturn, 3))
        ]);

    public static readonly Claim VerificationGapIsShould = new(
        "A verification gap (a user can do something the verification skill cannot) is a should whose Bug names maintain-verification-skill as owner and method.",
        [A.TicketCaptain, A.Inquisitor],
        [
            new(S.Inquisitor, "A verification gap is a should.", new(F.InquisitorSeverity, 2)),
            new(S.TicketCaptain, "A verification gap is a should whose Bug names `maintain-verification-skill` as its method.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 8))
        ]);

    public static readonly Claim VerificationSkillFirst = new(
        "A project with a runnable surface and no verification skill needs the skill first: the user makes it with create-verification-skill and the Inquisition resumes once it exists.",
        [A.TicketCaptain, A.Admiral, A.User],
        [
            new(S.TicketCaptain, "A project with a runnable surface and no verification skill stops here: the skill comes first.", new(F.TicketCaptainInquisitionN1CommissionInputs, 4))
        ]);

    public static readonly Claim WorkInOrder = new(
        "The inquisitor works the area items in order and runs the phase checklist in order: discovery, then work, then document.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Work the area's items in order.**", new(F.InquisitorMethod, 1))
        ]);

    public static readonly Claim WorkItemDone = new(
        "A work item is done when its entries for the return are written.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "Done when each item's entries for your return are written.", new(F.InquisitorMethod, 3))
        ]);
}
