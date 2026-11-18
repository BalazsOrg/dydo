namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>return</c> claims, sub-topics ordinary to writer.</summary>
public static class ReturnOrdinaryToWriter
{
    public static readonly Claim OrdinaryUpdatesCompact = new(
        "Ordinary updates expose only counts, exits, failure excerpts and an evidence link, and do not repeat contracts, hashes, full returns, passing-test name dumps, unchanged updates, or token, cost or latency claims.",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "Ordinary updates expose only counts, exits, failure excerpts, and an evidence link.", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 12))
        ]);

    public static readonly Claim PassBlockForm = new(
        "A PASS block reads `PASS - rubric/type: <rubric>; contract <ref>; candidate <SHA>; base <SHA>; reviewer/model: <name>; applicable tests <N/N>, gates <N/N>. Checks: ... Deferred (optional): ... Evidence: <link>.`",
        [A.Reviewer],
        [
            new(S.LinearWorkspaceStandard, "`PASS - rubric/type: <rubric>; contract <ref>; candidate <SHA>; base <SHA>; reviewer/model: <name>; applicable tests <N/N>, gates <N/N>. Checks: <one substantive sentence>. Deferred (optional): 1. Where: <place>; severity: <should|nit>; wrong: <fact>; why: <impact>; required correction: <action>; owner: <role>. Evidence: <link>.`", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 14))
        ]);

    public static readonly Claim PrOpenChecksPending = new(
        "A delegated captain whose PR awaits checks links the PR, returns PR open <key>, checks pending: <url>, head <SHA> and ends its turn.",
        [A.TicketCaptain], []);

    public static readonly Claim ProofOnlyVerdict = new(
        "A proof-only hop returns to the commissioning captain confirmed with its red-test SHA, not reproduced, or inconclusive with the deciding observation.",
        [A.CodeWriter], []);

    public static readonly Claim PrototypeHopReturnsDemo = new(
        "A Prototype hop loads the prototype skill and returns runnable-demo evidence and its commit, not the Prove section.",
        [A.CodeWriter],
        [
            new(S.CodeWriter, "**Prototype**: load `prototype`; return runnable-demo evidence and its commit, not the Prove section.", new(F.CodeWriterBugOrFeature, 7))
        ]);

    public static readonly Claim ResearchReportDestination = new(
        "The research report goes to the named ticket as a named evidence comment in the DECIDED form, or without a ticket to dydo/agents/workspace/research-<slug>.md; either way report the answer, destination and what stayed unsettled.",
        [A.Research],
        [
            new(S.Research, "Post the report to the ticket the invoker named, as a named evidence comment using the `DECIDED` form in the communication protocol.", new(F.ResearchReturn, 1))
        ]);

    public static readonly Claim ResearchReportShape = new(
        "The research report is written once, answer first, with each claim cited (URL, DOI or repo path, commit, exact passage), what stayed unsettled and the families searched including empty ones; it is done when a reader can check any claim and see what was not searched.",
        [A.Research],
        [
            new(S.Research, "The question; the answer; each claim with the citation that carries it (URL, DOI, or repository path, with the commit where it matters, plus the exact passage); what stayed unsettled; and the families searched, including those that came back empty.", new(F.ResearchMethod, 9))
        ]);

    public static readonly Claim ReviewBlockIsTheReturn = new(
        "The review block is the reviewer's return, in the PASS or FAIL form of the communication protocol with reviewer/model as signature, returned to the invoker even when a read-only host prevents posting.",
        [A.Reviewer],
        [
            new(S.Reviewer, "The block is the return, using the `PASS` or `FAIL` form in the communication protocol; its `reviewer/model` field is the reviewer's signature.", new(F.ReviewerReturn, 1))
        ]);

    public static readonly Claim ScoutReturn = new(
        "A scout returns to the researcher the findings, leads that leave its family, and when the family holds nothing, \"nothing here\" with the list searched.",
        [A.Scout],
        [
            new(S.Scout, "To the researcher: the findings; the leads you could not follow because they leave your family; and, when the family holds nothing, \"nothing here\" with the list of what you searched.", new(F.ScoutReturn, 1))
        ]);

    public static readonly Claim SeparateFactsFromCauses = new(
        "Separate facts from uncertain causes.",
        [A.Crew],
        [
            new(S.ChiefOfStaff, "Separate facts from uncertain causes.", new(F.ChiefOfStaffNightSurveyReturn, 4))
        ]);

    public static readonly Claim SpecReviewPassCondition = new(
        "A spec review block is PASS only when a writer can build the spec without being misled.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Return the review block.** PASS only when a writer can build it without being misled. **Done:** every finding states one material correction.", new(F.ReviewerSpecMethod, 7))
        ]);

    public static readonly Claim StateBlockForm = new(
        "A STATE block reads `STATE - <status>; owner: <role>; candidate or PR: <reference>; next or blocker: <fact>; evidence: <link>.`",
        [A.AnyAgent],
        [
            new(S.LinearWorkspaceStandard, "`STATE - <status>; owner: <role>; candidate or PR: <reference>; next or blocker: <fact>; evidence: <link>.`", new(F.LinearWorkspaceStandardCommunicationAndEvidence, 19))
        ]);

    public static readonly Claim SurveyThreeResults = new(
        "The survey crew returns one of NO_ACTION, ATTENTION or INCOMPLETE.",
        [A.Crew],
        [
            new(S.Admiral, "Return one of:", new(F.AdmiralCheckInSurveyTheSurveyor, 10)),
            new(S.ChiefOfStaff, "Return one result below.", new(F.ChiefOfStaffNightSurveySurvey, 6))
        ]);

    public static readonly Claim SurveyorHandleUnreflectedCaptainReturn = new(
        "When a captain's thread shows a finished result the ledger does not reflect, return it as `handle <captain> return — <its result line, UTC time>`.",
        [A.Crew],
        [
            new(S.Admiral, "[ ] A captain's thread shows a finished result the ledger doesn't reflect: return it as `handle <captain> return — <its result line, UTC time>`.", new(F.AdmiralCheckInSurveyTheSurveyor, 6))
        ]);

    public static readonly Claim SurveyorViaTaskResult = new(
        "Surveyor and inspector crew return findings through the task result only, never t3_thread_send.",
        [A.Crew],
        [
            new(S.Admiral, "Return through the task result only.", new(F.AdmiralCheckInSurveyTheSurveyor, 3)),
            new(S.ChiefOfStaff, "Inspect live work and return findings through the task result, never `t3_thread_send`.", new(F.ChiefOfStaffNightSurveyLead, 2))
        ]);

    public static readonly Claim TopLevelCaptainReturnsInOwnSession = new(
        "A top-level captain returns in its own session; the user tells an invoked admiral.",
        [A.TicketCaptain],
        [
            new(S.TicketCaptain, "A top-level captain returns in its own session; the user tells an invoked admiral.", new(F.TicketCaptainReturn, 1))
        ]);

    public static readonly Claim WriterReturnCarriesRedProof = new(
        "A code-writer's return carries its red proof (or for an unused-member Bug its definition and reference search) and the gates run with their exits, in the maintain-verification-skill proof recipe; a Bug fix claims success only with it.",
        [A.CodeWriter],
        [
            new(S.TicketCaptain, "The writer's `IMPLEMENTED` return carries its red proof, or for an unused-member Bug its definition and reference search, in the proof recipe `maintain-verification-skill` defines.", new(F.TicketCaptainMethod, 22))
        ]);
}
