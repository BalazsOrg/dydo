---
area: guides
type: guide
---
<!-- built from the model; edit there: tools/SkillBuild -->

# Working-Tree Standard

- How a Project's work moves through branches, worktrees, hops and merges; every rule here is a claim, built from the same model as the role skills.

## Invariants

- No self-review. An agent never reviews its own work or rules on findings about it: its own context biases it, and a fresh context catches what it can't. A leaf's evidence check is a fresh seat the officer sends. Each level checks for itself, never taking a lower level's verdict as its proof. A captain authors only a change too small to need its own review; the review above covers it.
- One writer per worktree, and commits touch owned paths only.
- Every writer hop ends on a commit `<KEY> <hop>: <what>`, where `<hop>` is `implement`, `fix` or `merge`, kept unsquashed. Merges are merge commits only, never squash or rebase.
- A ticket branch is `<KEY>-<slug>` off its feature branch, or off main for an atomic ticket, so Linear attaches the branch and PR; its branch, base SHA and worktree path are on the ticket before the first edit.

## Branches

- A Project's feature branch, `feature/<project-slug>`, is cut from main before its first ticket branch.
- The feature branch is the coordinator's: it merges into it as it wishes.
- The landing candidate is the feature branch with master merged in: the exact tree master gets.

## Delivery scale and evidence

- Each review round's seats scale with the breadth and depth of the work, with no constant pair; a small leaf may need no review.
- The author proves with receipts and a fresh reviewer judges through lenses; nothing is re-run the same way, and a DYTEST result for the same tree counts as proof unless later changes put it in doubt.

## Delegation

- A Linear unit goes out by `delegate_task`. A native sub-agent never gets one: it shares its holder's checkout, and the holder commits.
- Each parallel lane, and each sub-ticket with children, works in its own worktree; serial hops reuse their ticket's.
- Under T3, a native foreground sub-agent only when its dispatcher waits for it within its own turn, and no native background sub-agents; a dispatcher waiting on a native child, or a child awaiting a result it owns, stays in its turn until the result arrives.
- A native child lives only inside its dispatcher's turn and cannot call `delegate_task`: on `parent_not_active`, a captain asks its officer, the chief of staff asks the user, and the coordinator dispatches a captain's crew for it.
- A brief that sends out Codex agents names a cap: start around 8, go toward 16 only while runs stay clean, and back off on `429 turn_rate_limit_exceeded`; a capacity 429 means retry after a few minutes, never a blocker.
- Crew take their questions to their captain, never to the user.
- The cutter, the officer above, decides whether a unit gets children; no agent promotes itself, and crew return `too big, re-cut`.
- Crew work inside their ticket's contract, owned paths and gates, and commit their own hop.
- Ordinary crew do not create its branch, open its PR or merge it.
- Sub-captains are allowed with no hard cap, cut only for coherent units too big for one agent or able to run in parallel; the practical ceiling is coordinator → captain → sub-captain → second-order sub-captain → crew, and bad nesting, or a deeper sub-captain that fails, is dealt with as a finding.

## Merging

- A conflict-free merge into the feature branch is proved by the combined gates on the merge SHA plus hosted CI on the `feature/**` push; there is no merge review.
- `dydo merge` merges only when the Review is Done with a PASS on the head, or on an ancestor whose later commits are all clean syncs with their second parent already in the target, the merge with the tip is clean, and DYTEST holds a green receipt keyed by the combined tree; it refuses a hand-shaped merge, and there is no `--verify`.
- A conflict is resolved only on the ticket branch, under a fresh review of its hunks.
- A Merge hop is Done when DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge SHA.
- A gate red after a merge reruns once; green is a flake and the hop is Done; red again is real: the captain reverts at once and files a CI-findings ticket, a Fix follows on the ticket branch, then a fix-scoped round, then a re-land where `dydo merge` reverts the revert.
- A one-way PR, one whose revert doesn't undo its effect, needs his click and carries a recovery plan. One-way kinds: data migrations; publishing a package or a public API; anything sent outside the repo; secrets; history rewrites; changes to what guards main (CI, branch protection, release workflows, the guard).
- A two-way PR into master merges AFK, by its captain, once it has a PASS on the head or an ancestor, gates green on the combined tree with current master, CI green, and a body declaring it two-way.
- Main-bound merges land one at a time, paced by the coordinator; each re-checks against current master just before landing, and a PR forced to re-sync a third time is held while the coordinator sequences the colliding work.

## Cleanup

- After a merge, its captain safe-deletes the branches and worktrees it made; the archive keeps them 30 days, then purges.
