---
area: reference
type: reference
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Claims: Implementer

## Weight

`claim:Implementer.Weight`: Most hops implement a contract or fix blockers; a merge conflict is the smallest part (DYD-1082 ae8e285a).

Spec: K31

**implementer** (Job):

> Most hops implement a ticket's contract or fix a review's blockers; resolving a merge conflict is the occasional, smallest part.

## Verify

`claim:Implementer.Verify`: The implementer verifies its own work while it builds, with every tool, driving included (DYD-1082 a1b21b59 Q10).

Spec: K31, R5

**implementer** (Job):

> Verify your own work while you build it, with every tool the project has, driving the app included, so you hand off work you are confident in; how you work is yours to decide.

## ByUse

`claim:Implementer.ByUse`: Each file is checked the way it is used, such as an image rendered and looked at (DYD-1082 a1b21b59 Q10).

Spec: K31

**implementer** (Job):

> Check each file the way it is used: run the code, render an image and look at it, hold a doc against its source.

## Ground

`claim:Implementer.Ground`: Before the first edit the writer checks its branch, worktree, base and a clean tree, and returns a mismatch.

Spec: N12 DR 045 §8, K29

**implementer** (Briefed, nominal):

> Before the first edit, check that HEAD is on the brief's branch in its own worktree, the base SHA is an ancestor of HEAD and the tree is clean; return a mismatch rather than work around it.

## Recipe

`claim:Implementer.Recipe`: The implementer works from its code, docs, Bug or merge recipe (DYD-1082 1bc04dce change 4).

Spec: K31

**implementer** (Build):

> Open the recipe below for your work before you start it: code, docs or both, a Bug, or a merge conflict.

## NoWorse

`claim:Implementer.NoWorse`: The writer leaves what it touched no worse than it found it.

Spec: T13, K22

**implementer** (Build):

> Leave what you touched no worse than you found it.

## Glossary

`claim:Implementer.Glossary`: A glossary term or a Decision Record is touched with domain-modeling, as the pre-candidate code-writer did.

Spec: K14, K22

**implementer** (Build):

> Load /domain-modeling when you touch a glossary term or a Decision Record.

## OpenDesign

`claim:Implementer.OpenDesign`: An open design question stops the build and goes back to the captain, as the pre-candidate code-writer's prototype trigger did.

Spec: K22, K31

**implementer** (Build):

> When a design question is open, stop and return it to your captain; /prototype is the skill that answers one.

## Gherkin

`claim:Implementer.Gherkin`: The implementer writes and wires each acceptance line the contract marks for Gherkin as a feature-file scenario, never weakening it.

Spec: K22, K31

**implementer** (Code):

> Write each acceptance line the contract marks for Gherkin as a feature-file scenario at the product's boundary, in glossary words, with an example table only where values vary and every column changes an outcome, wired through step definitions; add or remove a scenario only when asked, and never weaken one.

## Platforms

`claim:Implementer.Platforms`: Platform-specific code is proved on CI's platform too, as the pre-candidate code-writer did.

Spec: K22, R5

**implementer** (Code):

> Prove platform-specific code, such as processes, paths and signals, on CI's platform too, not only on yours.

## Design

`claim:Implementer.Design`: A seam or interface is designed with codebase-design.

Spec: K14, K22

**implementer** (Code):

> Load /codebase-design when you design a seam or an interface.

## Mocks

`claim:Implementer.Mocks`: A mock sits only at the system's edge; the project's own modules stay real.

Spec: T4, K22

**implementer** (Code):

> Mock only at the system's edge, such as external APIs, time and randomness; your own modules stay real.

## Witness

`claim:Implementer.Witness`: Every sentence of a doc needs a witness in the repository; a missing one is returned as a gap.

Spec: D2, K22

**implementer** (Docs):

> Every sentence you write is a claim that needs a witness in the repository: code, configuration or governing intent (the spec, a DR, a ticket); where none exists, return the gap instead of the sentence.

## OneHome

`claim:Implementer.OneHome`: Each meaning has one home, and everywhere else links to it.

Spec: K28

**implementer** (Docs):

> Write each meaning in one home and link to it from everywhere else.

## BuiltSource

`claim:Implementer.BuiltSource`: A built file changes only in its source, then the build.

Spec: K6

**implementer** (Docs):

> A built file changes only in its source: edit the model or the generator, then rebuild.

## AgentDocs

`claim:Implementer.AgentDocs`: Agent-facing docs are written with writing-for-agents.

Spec: K14, K22

**implementer** (Docs):

> Write a skill, its resources or an entry file with /writing-for-agents.

## DocsProof

`claim:Implementer.DocsProof`: A docs change is proved with dydo check and the ticket's checks; dydo fix follows an added, moved or renamed doc.

Spec: D5, K13

**implementer** (Docs):

> Prove a docs change with `dydo check` and the checks the ticket names; run `dydo fix` after you add, move or rename a document.

## BugSeam

`claim:Implementer.BugSeam`: A Bug names expected and observed behaviour at the highest stable seam, and what must not change.

Spec: T3, T4

**implementer** (Bug):

> Name the expected and the observed behaviour at the highest stable seam, and what the fix must leave unchanged.

## BugRed

`claim:Implementer.BugRed`: A Bug is reproduced red at the boundary where it shows; an existing red test is its reproduction.

Spec: T3, T4

**implementer** (Bug):

> Reproduce it red: a test at the product's boundary when the defect shows there, else at that seam; an existing red test is the reproduction.

## BugDiagnose

`claim:Implementer.BugDiagnose`: A Bug with no red reproduction yet loads diagnosing-bugs, as the pre-candidate code-writer did (DYD-1287 contract); its dispatcher names the skill.

Spec: T3, K14

**implementer** (Bug):

> With no red reproduction yet, load /diagnosing-bugs.

**ticket-captain** (legacy, not built; ImplementerBrief):

> On a Bug, add /diagnosing-bugs to the skills this brief names.

## BugUnused

`claim:Implementer.BugUnused`: An unused member's reproduction is its definition and a reference search; the fix deletes it with its test-only callers.

Spec: T3, K22

**implementer** (Bug):

> An unused member needs no test: its definition at `file:line` and a reference search with no caller outside the tests reproduce it, and the fix deletes it with its test-only callers.

## BugRevert

`claim:Implementer.BugRevert`: A Bug fix is reverted to watch the reproduction fail, then restored; the return carries that red line.

Spec: T3

**implementer** (Bug):

> Fix it, then revert the fix, watch the reproduction fail and restore it; return that red line, with the behaviour you kept still green.

## BugRace

`claim:Implementer.BugRace`: A race is forced with fake timers or interleaving; a test needing a sleep or timeout is wrong.

Spec: T1, K22

**implementer** (Bug):

> Force a race with fake timers or a set interleaving; a test that needs a sleep or a timeout is wrong.

## MergeRead

`claim:Implementer.MergeRead`: A conflict is resolved after reading both sides and what changed since the source's review.

Spec: R19, K22

**implementer** (Resolving, nominal):

> Read both sides, and what changed since the source was reviewed, before you resolve a hunk.

## MergeIntent

`claim:Implementer.MergeIntent`: Each hunk's intended behaviour is stated; a new product choice goes back to the captain.

Spec: R19, K22

**implementer** (Resolving, nominal):

> State each hunk's intended behaviour; one that needs a new product choice goes back to your captain.

## MergeNoWorse

`claim:Implementer.MergeNoWorse`: A resolution that refactors leaves the code no worse than either side.

Spec: R19, K22

**implementer** (Resolving, nominal):

> A resolution that refactors leaves the code no worse than either side.

## MergeReturn

`claim:Implementer.MergeReturn`: A resolved conflict returns both parents and the merge commit's SHA.

Spec: R32

**implementer** (Resolving, nominal):

> Return both parents and the merge commit's SHA.
