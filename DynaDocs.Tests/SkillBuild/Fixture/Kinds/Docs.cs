namespace DynaDocs.Tests.SkillBuild.Fixture.Kinds;

using DynaDocs.SkillBuild;
using A = DynaDocs.Tests.SkillBuild.Fixture.FixtureActor;
using F = DynaDocs.Tests.SkillBuild.Fixture.FixtureSection;
using S = DynaDocs.Tests.SkillBuild.Fixture.FixtureSkill;

/// <summary>Migration fixture: the inventory's <c>docs</c> claims, sub-topics agent to working.</summary>
public static class Docs
{
    public static readonly Claim AgentDocsAnchorAndTagline = new(
        "An anchor missing or doubled, or a tagline that changes nothing, is an agent-docs finding; the no-op test grades against the model default and a failing line is deleted rather than softened.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Anchor missing or doubled, or a tagline that changes nothing.**", new(F.ReviewerDocsAgentFacingDocuments, 3))
        ]);

    public static readonly Claim AgentDocsCrossReferences = new(
        "A cross-reference missing or extra against the exact set its brief binds is an agent-docs finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**A cross-reference missing or extra** against the exact set its brief binds.", new(F.ReviewerDocsAgentFacingDocuments, 7))
        ]);

    public static readonly Claim AgentDocsDescriptionIsTrigger = new(
        "An agent-doc description that is not a trigger is a finding; a crew role spawned by name may state its job in trigger form.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Description that is not a trigger.** Model-invoked: leading word front-loaded, one trigger per branch, no identity the body already carries. Explicit-only: one punchy human-facing line.", new(F.ReviewerDocsAgentFacingDocuments, 2))
        ]);

    public static readonly Claim AgentDocsGlossaryVocabulary = new(
        "Vocabulary off the dydo glossary (a retired word anywhere, or a Linear noun away from a real handoff) is an agent-docs finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Vocabulary off the dydo glossary**: a retired word anywhere, or a Linear noun away from a real handoff.", new(F.ReviewerDocsAgentFacingDocuments, 8))
        ]);

    public static readonly Claim AgentDocsGovernedByWritingForAgents = new(
        "writing-for-agents governs agent-facing writing; the reviewer agent-docs list is that method turned into verdicts.",
        [A.Reviewer, A.DocsWriter],
        [
            new(S.DocsWriter, "A dydo document takes its frontmatter and links from writing-docs.md; an agent-facing document (a skill, its resources, an entry point) is written by `writing-for-agents`.", new(F.DocsWriterMethod, 3), () => [Docs.FrontmatterAndLinksPerWritingDocs!]),
            new(S.Reviewer, "`writing-for-agents` governs this writing; the list below is that method turned into verdicts.", new(F.ReviewerDocsAgentFacingDocuments, 1))
        ]);

    public static readonly Claim AgentDocsOffTheMap = new(
        "An agent doc off the map is a finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Off the map.** Must-Reads that do not name what the sender hands over, a Return that does not name its receiver, or a sentence narrating what a neighbour does.", new(F.ReviewerDocsAgentFacingDocuments, 6))
        ]);

    public static readonly Claim AgentDocsProhibitionSteering = new(
        "Steering by prohibition where the positive target would carry it is an agent-docs finding; a guardrail states both.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Steering by prohibition** where the positive target would carry it; a guardrail states both.", new(F.ReviewerDocsAgentFacingDocuments, 9))
        ]);

    public static readonly Claim AgentDocsReturnShapeParsable = new(
        "A return shape its consumer cannot parse (the review block, or a crew return to the Captain) is an agent-docs finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**A return shape its consumer cannot parse**: the review block, or a crew member's return to the Captain.", new(F.ReviewerDocsAgentFacingDocuments, 11))
        ]);

    public static readonly Claim AgentDocsShape = new(
        "Broken shape is an agent-docs finding; a section order the user set for a role stands in place of Method, while Must-Reads, Boundary, Return and a completion criterion wherever steps exist still bind.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Shape broken.** Officers and crew: H1 → one-line job → Must-Reads → Boundary → Method with a completion criterion on every step → Return or Handoff.", new(F.ReviewerDocsAgentFacingDocuments, 4))
        ]);

    public static readonly Claim AgentDocsSprawl = new(
        "Sprawl, a document longer than its reader path needs, is an agent-docs finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Sprawl**: a document longer than its reader's path needs. Reference only some branches reach goes behind a pointer; a meaning stated twice is stated once.", new(F.ReviewerDocsAgentFacingDocuments, 10))
        ]);

    public static readonly Claim AgentDocsUpstreamUnaltered = new(
        "Methods keep their upstream shape; upstream text altered without a binding reason is an agent-docs finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Methods keep their upstream shape.", new(F.ReviewerDocsAgentFacingDocuments, 5))
        ]);

    public static readonly Claim AuthorNavigationPagesWhenNeeded = new(
        "Navigation pages are authored when the docs need them.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "Author navigation pages when the docs need them; `skills/<category>/<name>/` is the only editable skill source and host discovery paths are setup projections.", new(F.DocsWriterMethod, 6), () => [Docs.SkillsTreeIsTheOnlyEditableSource!])
        ]);

    public static readonly Claim ClaimNeedsWitness = new(
        "Every sentence written is a claim needing a witness in the repository (code, configuration, or governing intent); where the witness is missing the gap is returned instead of the claim written, and the code, configuration and decisions are read until every claim has one.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "Every sentence you write is a claim, and every claim needs a **witness** in the repository: code, configuration, governing intent (spec, Decision Record or ticket).", new(F.DocsWriterBoundary, 1))
        ]);

    public static readonly Claim CutWhatCodeStates = new(
        "Cut whatever the code already states plainly.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "Cut whatever the code already states plainly.", new(F.DocsWriterMethod, 4))
        ]);

    public static readonly Claim DocsWorkDone = new(
        "Docs work is done when each claim has one canonical home, repeated meanings are pointers, and every added claim has its witness.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "Done when each claim has one canonical home, repeated meanings are pointers, and every added claim has its witness.", new(F.DocsWriterMethod, 5))
        ]);

    public static readonly Claim DriftIsFinding = new(
        "Drift is a finding: a claim the tree no longer supports, an environment restated in prose (directory layout, --help, config) where a pointer would not go stale, or agent-facing drift from the canonical source.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Drift** is the failure both kinds share: a claim the tree no longer supports, or an environment restated in prose (directory layout, `--help`, config) where a pointer would not have gone stale.", new(F.ReviewerDocsLead, 2))
        ]);

    public static readonly Claim DydoCheckLeftFailing = new(
        "dydo check or a ticket gate left failing on the touched tree is a human-docs finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "`dydo check` or a ticket gate left failing on the touched tree.", new(F.ReviewerDocsHumanFacingDydoDocuments, 3))
        ]);

    public static readonly Claim EditsInAuthoredSourceDone = new(
        "Docs work is done when every edit is in its authored source and required regeneration is verified in scope or handed to the integration owner with its source paths and command.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "Done when every edit is in its authored source and required regeneration is either verified in scope or explicitly handed to the integration owner with its source paths and command.", new(F.DocsWriterMethod, 7))
        ]);

    public static readonly Claim FrontmatterAndLinksPerWritingDocs = new(
        "A dydo document takes its frontmatter and links from dydo/reference/writing-docs.md; a frontmatter, naming or link broken against it is a finding.",
        [A.DocsWriter, A.Reviewer],
        [
            new(S.Reviewer, "Frontmatter, naming, or a link broken against `dydo/reference/writing-docs.md`, read from the repository root.", new(F.ReviewerDocsHumanFacingDydoDocuments, 1))
        ]);

    public static readonly Claim MaintainFeatureIntentSpec = new(
        "Shared feature intent is maintained following dydo/reference/feature-spec-convention.md.",
        [A.Admiral], []);

    public static readonly Claim OneHomePerClaim = new(
        "Each claim is written in its one home (concepts in understand/, procedures in guides/, exact contracts in reference/, temporary specs under reference/working-specs/, delivery history under project/) and a meaning another document owns is a link.",
        [A.DocsWriter, A.Reviewer],
        [
            new(S.DocsWriter, "Concepts in `understand/`, procedures in `guides/`, exact contracts in `reference/`, temporary specs under `reference/working-specs/`, delivery history under `project/`; a meaning another document owns is a link.", new(F.DocsWriterMethod, 1)),
            new(S.Reviewer, "Written for a reader its folder does not name (`understand/` vs `guides/` vs `reference/`).", new(F.ReviewerDocsHumanFacingDydoDocuments, 2))
        ]);

    public static readonly Claim ReadmeOutsideDydoPointsOnly = new(
        "A README outside dydo/ only points to the dydo doc that owns its folder; skill-owned resources are exempt.",
        [A.DocsWriter],
        [
            new(S.DocsWriter, "A README outside `dydo/` only points to the dydo doc that owns its folder; skill-owned resources are exempt.", new(F.DocsWriterMethod, 2))
        ]);

    public static readonly Claim ReusableKnowledgeAssimilated = new(
        "A reusable decision, invariant, pitfall or explanation left only in execution evidence instead of assimilated into dydo is a human-docs finding.",
        [A.Reviewer],
        [
            new(S.Reviewer, "A reusable decision, invariant, pitfall or explanation left only in execution evidence instead of assimilated into dydo.", new(F.ReviewerDocsHumanFacingDydoDocuments, 4))
        ]);

    public static readonly Claim ReviewFindingsLabelledWithSeverity = new(
        "Every review item below is a finding when it holds, labelled with its severity.",
        [A.Reviewer],
        [
            new(S.Reviewer, "Every item below is a finding when it holds, labelled with its severity.", new(F.ReviewerDocsLead, 3))
        ]);

    public static readonly Claim ReviewResolveContract = new(
        "The reviewer resolves the contract: ticket description as of the brief Linear updatedAt, and the governing plan and feature-spec sections at pinned commits.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Resolve the contract.** The ticket description as of the brief's Linear `updatedAt`, and the governing plan and applicable feature-spec sections at pinned commits. Done when audience, owned paths, acceptance criteria and gates are named; an unmet criterion or an edit outside the owned paths is a finding.", new(F.ReviewerDocsMethod, 1))
        ]);

    public static readonly Claim ReviewSortChangedFiles = new(
        "The reviewer sorts every changed file into one of the two lists (human docs, agent docs), then works that list entire.",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Sort every changed file** into one of the two lists below, then work that list entire.", new(F.ReviewerDocsMethod, 2))
        ]);

    public static readonly Claim ReviewVerifyFromSource = new(
        "The reviewer verifies from source: opens the paths, runs the commands, reads the code and checks the stage exact-candidate gate evidence (dydo check among it).",
        [A.Reviewer],
        [
            new(S.Reviewer, "**Verify from source.** Open the paths, run the commands, read the code; check this stage's exact-candidate gate evidence, `dydo check` among it, and rerun a gate only on a doubt you state. Done when every claim the change makes has met its source.", new(F.ReviewerDocsMethod, 3))
        ]);

    public static readonly Claim SkillsTreeIsTheOnlyEditableSource = new(
        "skills/<category>/<name>/ is the only editable skill source; host discovery paths are setup projections.",
        [A.DocsWriter], []);

    public static readonly Claim WorkingSpecDraft = new(
        "The admiral drafts the working spec at dydo/reference/working-specs/<project-slug>/<project-slug>.md with the convention headings; it holds what the user agreed (problem, behavior, decisions, out of scope, acceptance checks) in the user words where they matter.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] Draft the working spec for `dydo/reference/working-specs/<project-slug>/<project-slug>.md` with the headings in `dydo/reference/feature-spec-convention.md` (from the repository root).", new(F.AdmiralScaffoldProjectLead, 2))
        ]);

    public static readonly Claim WorkingSpecDraftInProjectDescription = new(
        "The spec draft is put in the Project description under ## Working spec draft.",
        [A.Admiral],
        [
            new(S.Admiral, "Put the draft in the Project description under `## Working spec draft`.", new(F.AdmiralScaffoldProjectLead, 3))
        ]);

    public static readonly Claim WorkingSpecLinkAndDeleteDraft = new(
        "Once the spec is on the remote it is linked from the Project description and the draft deleted.",
        [A.Admiral],
        [
            new(S.Admiral, "[ ] The spec is on the remote: link it from the Project description and delete the draft.", new(F.AdmiralScaffoldProjectLead, 8))
        ]);
}
