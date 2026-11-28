---
name: to-project
description: Turn this co-think into a Linear Project, with its settled intent and links. Load only when the user asks.
---

<!-- Adapted from mattpocock/skills to-spec at 6654f6b60cd9d5be8b54c6fafe44346dabeb3b76 (MIT). -->

# To Project

Synthesize the conversation into one Linear Project. The co-think supplies the intent; this command
records it without another interview.

Use `to-project` when the intent should become a Linear Project the factory runs: intent only, the
coordinator and map holder take it from there, and the map holder writes its spec later. When the
user wants the whole spec written now, for work he will drive more hands-on or that fits under one
ticket tree without a Project, he invokes `to-spec` instead.

1. Read the settled context and any linked Decision Records, glossary entries and source
   FutureFeature. Use the project's words. Done when the problem, solution, decisions taken and out
   of scope can be stated without inventing an answer.
2. Write the Project title, short summary and intent description under those four headings. Link
   the records and source FutureFeature when present. Done when the Project carries the intent and
   its provenance, with unresolved points still identified as unresolved.
3. Create the Project in `Backlog`, with no tickets. The Project's own spec is its `Spec: <Project>` Linear document, which the map holder writes and refines (S14); the rest of this step covers feature specs. When shared behavior or decisions need one
   current anchor, read `dydo/reference/feature-spec-convention.md` from the repository root. Link the
   governing spec or one of its linked feature specs; if neither supplies the anchor, prepare a
   feature spec from settled intent for the map holder's custody. The map holder commits a new spec
   at its convention path and makes that commit reachable to recipients before relying on its Project
   link; otherwise record the anchor as unresolved. An atomic outcome stays in its eventual ticket.
   Done when any shared anchor is reachable and linked, with unresolved intent explicit.
4. Return the Project link and the next step: the user breaks it into tickets with `to-ticket`, or
   starts its coordinator, himself or through the chief of staff. Done when the recorded Project can be read without this conversation.
