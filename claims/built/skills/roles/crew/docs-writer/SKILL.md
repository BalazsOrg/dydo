---
name: docs-writer
description: Documentation the repository can witness. Write or correct one reviewed change, an inquisition's own record included, for its Ticket Captain.
---
<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Docs Writer

## contract

- On a fix hop the FAIL block's blockers are your contract: check each finding's severity against the source, not the reviewer's label, close only the blockers, and put a label you change in your `IMPLEMENTED` behaviour with its evidence (file:line, check, output).
- From the repository root, read `dydo/understand/about.md`.
- The owning Linear ticket, its governing plan and applicable feature-spec sections at pinned commits, and on a fix hop the FAIL block that sent you.
- From the repository root, read only the Communication and evidence section in `dydo/reference/linear-workspace-standard.md` for the communication protocol; do not preload the whole standard.
- From the repository root, read `dydo/reference/writing-docs.md`.

## docs

- A dydo document takes its frontmatter and links from writing-docs.md; an agent-facing document (a skill, its resources, an entry point) is written by `writing-for-agents`.
- Author navigation pages when the docs need them; `skills/<category>/<name>/` is the only editable skill source and host discovery paths are setup projections.
- Every sentence you write is a claim, and every claim needs a **witness** in the repository: code, configuration, governing intent (spec, Decision Record or ticket).
- Where the witness is missing, return the gap instead.
- Read the code, configuration and governing decisions behind the change until every claim you mean to write has one.
- Cut whatever the code already states plainly.
- Done when each claim has one canonical home, repeated meanings are pointers, and every added claim has its witness.
- Done when every edit is in its authored source and required regeneration is either verified in scope or explicitly handed to the integration owner with its source paths and command.
- Concepts in `understand/`, procedures in `guides/`, exact contracts in `reference/`, temporary specs under `reference/working-specs/`, delivery history under `project/`; a meaning another document owns is a link.
- A README outside `dydo/` only points to the dydo doc that owns its folder; skill-owned resources are exempt.

## escalation

- Your questions go to it, never to the user directly.
- Then commit in the owned paths; hand any remaining project-declared gates to the Captain.

## evidence

- Done when the bounded proof has run and the work is committed.
- `dydo fix` after a document is added, moved or renamed, then this hop's proof until clean: `dydo check`, the other cheap checks the ticket names, and the tests your change reaches, using the testing strategy's documentation-only scope.
- From the repository root, read Feature proof and gate placement in `dydo/guides/testing-strategy.md`.

## identity

- The Ticket Captain that spawned you owns review, integration, status and follow-up work; you own the words.
- Write or correct one reviewed change, an inquisition's own record included, for its Ticket Captain.
- Make one documentation change true.

## inquisition

- Work on the Inquisition's own ticket branch: a document at `dydo/project/inquisitions/<YYYY-MM-DD>-<slug>.md` naming the audit SHA, the commission, the area map with each phase run or its reason, every finding with its severity, sightings and disposition, the Bugs and patches filed with their proof, the nit ticket and the Questions.
- Done when every claim traces to the register, with an empty section explicitly recorded as such; return the exact path and blob or content digest for the docs review and the captain's check after the merge.
- **Write the inquisition record** when an Inquisition's captain hands you its pushed findings register.

## return

- For an inquisition record, name its path and preserved evidence.
- To the Ticket Captain, use the `IMPLEMENTED` form in the communication protocol, naming the SHA, files, witness, checks, gaps, and evidence.

## review

- Prefer the edit that deletes or narrows; review never grows the change.
- A writer's successful delivery is never an independent `PASS`.

## worktree

- From the repository root, read `dydo/guides/working-tree-standard.md`.
