namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>inquisition</c> claims, sub-topics group to read.</summary>
public static class InquisitionGroupToRead
{
    public static readonly Claim GroupCaptainRunAndClose = new(
        "A group captain runs the Inquisition steps on its rows, owns its Bugs retention, and closes Done with its register final and pushed.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A group captain runs steps 2 and 5 to 10 on its rows, owns its Bugs' retention, and closes `Done` with its register final and pushed.", new(F.TicketCaptainInquisitionN4SplitAboveAbout10Areas, 6)),
            new(S.WorkingTreeStandard, "Each group Sub-ticket owns its own Bugs' retention below and closes `Done` with its findings register final and pushed; the parent rechecks retention over the merged register before its record.", new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, 2), () => [InquisitionGroupToRead.ParentRechecksRetentionOverMergedRegister!])
        ]);

    public static readonly Claim GroupNitsToParentTicket = new(
        "Group captains add their nits to the parent nit ticket.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Group captains add their nits to your nit ticket.", new(F.TicketCaptainInquisitionN4SplitAboveAbout10Areas, 5))
        ]);

    public static readonly Claim GroupReleaseWiring = new(
        "The parent is wired blocked by every group, this round nit ticket is opened, then the parent is released with released <key>: groups <keys>.",
        [A.TicketCaptain], []);

    public static readonly Claim GroupSplitThreshold = new(
        "Above about 10 areas the captain cuts the map into group Sub-tickets of up to about 10 areas each, seams kept with the areas they join, and releases the parent with them.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.TicketCaptain, "One captain conducts about 10 areas.", new(F.TicketCaptainInquisitionN4SplitAboveAbout10Areas, 1))
        ]);

    public static readonly Claim GroupSubticketContents = new(
        "Each group Sub-ticket carries the Inquisition Type, the parent Mode and commission, its map rows and its own inquisition/<slug>-<group> branch from the same audit SHA.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Each group Sub-ticket carries the Inquisition Type, the parent's Mode and commission, its map rows and its own `inquisition/<slug>-<group>` branch from the same audit SHA.", new(F.TicketCaptainInquisitionN4SplitAboveAbout10Areas, 2))
        ]);

    public static readonly Claim NitIsOneLine = new(
        "A nit is one line on this round nit ticket with its file:line and the wrong outcome; it needs no proof.",
        [A.TicketCaptain, A.Inquisitor],
        [
            new(S.Inquisitor, "| Nit | none; a nit needs no proof | any |", new(F.InquisitorProof, 6)),
            new(S.TicketCaptain, "**Nit:** one line on this round's nit ticket, with its `file:line` and the wrong outcome; it needs no proof.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 9))
        ]);

    public static readonly Claim NitTicketClosesWhenEveryNitDecided = new(
        "A nit ticket closes Done once every nit is fixed or accepted as is.",
        [A.MapHolder],
        [
            new(S.TicketCaptain, "The map holder lists every open nit ticket at the Project's final walkthrough, where the user decides each nit; it closes `Done` once every nit is fixed or accepted as is.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 11), () => [InquisitionGroupToRead.NitTicketFinalWalkthrough!])
        ]);

    public static readonly Claim NitTicketFinalWalkthrough = new(
        "The map holder lists every open nit ticket at the Project final walkthrough, where the user decides each nit.",
        [A.MapHolder], []);

    public static readonly Claim NitTicketOpening = new(
        "The nit ticket is opened with the first nit: Type Feature, Mode AFK, Severity nit, Backlog, held by the map holder, linked to the Inquisition.",
        [A.TicketCaptain, A.MapHolder],
        [
            new(S.TicketCaptain, "Open the nit ticket with the first nit: Type `Feature`, Mode `AFK`, Severity `nit`, `Backlog`, held by the map holder, linked to this Inquisition.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 10))
        ]);

    public static readonly Claim NoHypothesisQuota = new(
        "The captain sets no quota of hypotheses or journeys.",
        [A.TicketCaptain], []);

    public static readonly Claim OfferNextRoundAfterFixes = new(
        "Offer the next Inquisition round once every blocker and should fix of the previous round is complete.",
        [A.MapHolder, A.Admiral],
        [
            new(S.Admiral, "Rounds start from zero: once every blocker and should fix of a round is `Done`, offer the next round as a fresh Inquisition, never a recheck of the old findings.", new(F.AdmiralMethod, 61), () => [InquisitionRecordToWork.RoundsStartFromZero]),
            new(S.TicketCaptain, "The map holder offers the next round once every blocker and should Bug is fixed.", new(F.TicketCaptainInquisitionN12Close, 4))
        ]);

    public static readonly Claim OtherBugFiledUnassignedPickable = new(
        "A Bug whose fix the patch lane will not prepare is filed unassigned and pickable at once.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A Bug whose fix step 9 will prepare is filed assigned to you, the claim that keeps it unpicked, naming its patch branch; any other Bug is filed unassigned and pickable at once.", new(F.TicketCaptainInquisitionN8TriageEachReturn, 5), () => [InquisitionGroupToRead.PatchLaneBugAssignment!])
        ]);

    public static readonly Claim ParentClosesDuplicateBugAsDuplicate = new(
        "The parent closes a duplicate Bug as Duplicate of the one kept.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "When the groups close you resume as parent: merge their registers into yours, join duplicates across groups as sightings, and close a duplicate Bug as `Duplicate` of the one kept.", new(F.TicketCaptainInquisitionN4SplitAboveAbout10Areas, 7), () => [InquisitionGroupToRead.ParentJoinsCrossGroupDuplicatesAsSightings!, InquisitionGroupToRead.ParentMergesGroupRegisters!])
        ]);

    public static readonly Claim ParentJoinsCrossGroupDuplicatesAsSightings = new(
        "The parent joins duplicates across groups as sightings.",
        [A.TicketCaptain], []);

    public static readonly Claim ParentMergesGroupRegisters = new(
        "When the groups close the parent resumes and merges their registers into its own; grouping is done when every area belongs to one group and each group's register is merged into the parent's.",
        [A.TicketCaptain, A.Admiral], []);

    public static readonly Claim ParentRechecksRetentionOverMergedRegister = new(
        "The parent rechecks retention over the merged register before its record.",
        [A.TicketCaptain], []);

    public static readonly Claim ParentRunsRecordStepOverMergedRegister = new(
        "The parent runs the record step over the merged register and delivers the one record.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Then run step 10 over the merged register and deliver the one record.", new(F.TicketCaptainInquisitionN4SplitAboveAbout10Areas, 8))
        ]);

    public static readonly Claim PatchCherryPickRedTest = new(
        "The patch code-writer cherry-picks the finding red-test commit first, then commits the fix that turns it green.",
        [A.CodeWriter],
        [
            new(S.TicketCaptain, "It cherry-picks the finding's red-test commit first, then commits the fix that turns that test green.", new(F.TicketCaptainInquisitionN9RunThePatchLane, 3))
        ]);

    public static readonly Claim PatchCodeWriterBrief = new(
        "The patch is briefed to a code-writer in Bug mode on the Bug own branch <BUG-KEY>-<slug>, cut from the audit SHA, with base SHA and owned paths posted.",
        [A.TicketCaptain, A.CodeWriter],
        [
            new(S.TicketCaptain, "Brief a `code-writer` in Bug mode on the Bug's own branch, `<BUG-KEY>-<slug>`, cut from the audit SHA, with the Bug's base SHA and owned paths posted on it.", new(F.TicketCaptainInquisitionN9RunThePatchLane, 2))
        ]);

    public static readonly Claim PatchEligibility = new(
        "A patch is prepared now only for a blocker or should that Break proved with a red test and whose fix is well-defined (one obvious fix, inside one area, small, touching no contract, Decision Record or public interface); any doubt means no patch and nits are never patched.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A blocker or should that Break proved with a red test, whose fix is **well-defined**, gets it prepared now: one obvious fix, inside one area, small, touching no contract, Decision Record or public interface.", new(F.TicketCaptainInquisitionN9RunThePatchLane, 1))
        ]);

    public static readonly Claim PatchFailureUnassign = new(
        "When the patch lane fails, the fix is not well-defined after all, or the code-writer returns blocked, the reason is recorded on the Bug and it is unassigned without a patch.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "When the lane fails, the fix not well-defined after all or the code-writer returning blocked, record the reason on the Bug and unassign it without a patch.", new(F.TicketCaptainInquisitionN9RunThePatchLane, 5))
        ]);

    public static readonly Claim PatchLaneBugAssignment = new(
        "A Bug whose fix will be prepared is filed assigned to the captain, naming its patch branch, and stays assigned until the patch is attached, then is unassigned and pickable.",
        [A.TicketCaptain, A.Admiral],
        [
            new(S.Admiral, "A patch-lane Bug stays assigned to the inquisition's captain until its patch is attached; it is pickable once unassigned.", new(F.AdmiralMethod, 60))
        ]);

    public static readonly Claim PatchReviewedOnceInBugChain = new(
        "The Bug captain verifies and reviews the patch once, in the Bug own chain.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "The Bug's captain verifies and reviews the patch once, in the Bug's own chain.", new(F.TicketCaptainInquisitionN9RunThePatchLane, 4))
        ]);

    public static readonly Claim PatchingDone = new(
        "Patching is done when every Bug filed for the lane is unassigned with its patch attached or its reason recorded.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Done when every Bug filed for the lane is unassigned with its patch attached or its reason recorded.", new(F.TicketCaptainInquisitionN9RunThePatchLane, 6))
        ]);

    public static readonly Claim PhasesAsPipeline = new(
        "The phases run as a pipeline across areas, never in waves: an area Break starts when its Read returns, its Drive when its Break returns.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "Run the phases as a pipeline across areas, never in waves: an area's Break starts when its Read returns, its Drive when its Break returns.", new(F.TicketCaptainInquisitionN6ScheduleThePipeline, 1))
        ]);

    public static readonly Claim PostMergeVerification = new(
        "After the record merges, the captain verifies on the feature the record at its path, every Bug filed and wired and every retained ref live; a record only on an unmerged branch does not satisfy it.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "After the record merges, verify on the feature: the record at its path, every Bug filed and wired, every retained ref live.", new(F.TicketCaptainInquisitionN12Close, 1)),
            new(S.WorkingTreeStandard, "After the record merges, it verifies on the feature the record at its path, every Bug filed and wired, and every retained ref live; a record only on an unmerged branch cannot satisfy this check.", new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, 8))
        ]);

    public static readonly Claim ProoflessBugHasNoRef = new(
        "A prose Bug carries its quoted passages, an unused-member Bug its definition and reference search, and a drive Bug its rerunnable drive steps in the body, with no ref to retain.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A prose, unused-member or drive Bug carries its proof in its body and has no ref to retain.", new(F.TicketCaptainInquisitionN10CloseTheSweep, 2)),
            new(S.WorkingTreeStandard, "A prose Bug carries its quoted passages, an unused-member Bug its definition and reference search, and a drive Bug its rerunnable drive steps; none has a ref to retain.", new(F.WorkingTreeStandardRetainingAnInquisitionSRecordAndProofs, 5))
        ]);

    public static readonly Claim ProseAreaReadOnly = new(
        "A plain skill or doc area gets Read only; its Break and Drive cells say prose.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Prose:** a plain skill or doc area gets Read only; its Break and Drive cells say `prose`.", new(F.TicketCaptainInquisitionN3DrawTheAreaMap, 6))
        ]);

    public static readonly Claim PushBreakProofBranch = new(
        "Break proof branch is pushed; its red-test SHAs are reproductions.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "**Break's commits:** push the proof branch; its red-test SHAs are reproductions.", new(F.TicketCaptainInquisitionN7RouteEachReturn, 4))
        ]);

    public static readonly Claim ReadFindIntent = new(
        "Read finds the intent: the ticket contracts, plan sections and Decision Records the brief names for the area, and any DR the area files cite.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Find the intent.** Read the ticket contracts, plan sections and Decision Records the brief names for this area, and any Decision Record the area's files cite. Done when you can say in one sentence what the area does and which intent governs it.", new(F.InquisitorReadDiscovery, 2))
        ]);

    public static readonly Claim ReadPinArea = new(
        "Read pins the area: in the brief worktree at the audit SHA open every file in the area map row whole, not only diff hunks.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Pin the area.** In the worktree your brief names, at the audit SHA, open every file in the area's map row, whole, not only the diff hunks; `git diff <landing base>..<audit SHA> -- <paths>` shows what landed. Done when every path in the row is open at the audit SHA.", new(F.InquisitorReadDiscovery, 1))
        ]);

    public static readonly Claim ReadProseAreaQuestions = new(
        "In a prose area the Read questions are intent, decisions and contradiction (two passages claiming different things about one fact).",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "In a prose area the questions are intent, decisions and contradiction: two passages, in the area or across it, that claim different things about one fact.", new(F.InquisitorReadWork, 2))
        ]);

    public static readonly Claim ReadQuestionChecklist = new(
        "Read asks every question of every file, relentlessly: intent, decisions, comments and docs, standards, security, dead code and, in a seam area, what each side assumes of the other.",
        [A.Inquisitor],
        [
            new(S.Inquisitor, "**Ask every question of every file**, relentlessly: Done when each question has been asked of each file.", new(F.InquisitorReadWork, 1))
        ]);
}
