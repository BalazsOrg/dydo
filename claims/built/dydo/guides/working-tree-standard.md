<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Working-Tree Standard

## Contents

- [capacity](#capacity)
- [contract](#contract)
- [dispatch](#dispatch)
- [escalation](#escalation)
- [evidence](#evidence)
- [identity](#identity)
- [inquisition](#inquisition)
- [linear](#linear)
- [merge](#merge)
- [project](#project)
- [release](#release)
- [return](#return)
- [review](#review)
- [worktree](#worktree)

## capacity

- The Admiral budgets open native capacity before commissioning or resuming a captain.
- The captain alone commissions exact-scope delivery crew, budgets that capacity and preserves the fresh reviewer obligation; `research` still sends its own scouts.
- A capacity 429 means retry after a few minutes, never a blocker.
- Every brief that sends out Codex agents names their concurrency cap: start around 8 at once, rise toward 16 only while runs stay clean, and back off on `429 turn_rate_limit_exceeded` ("Codex is at capacity").
- A brief asks for a capped count, never for "maximise parallelism".

## contract

- Fix is natively blocked by reproduction; shared paths transfer only after the reproduction closes with its evidence recorded.
- This exception does not permit overlapping parallel lanes.
- Each stage has a contract, branch and worktree; every actual integration has a Merge Sub-ticket.
- A Bug may retain its Type template's ordered reproduce-or-identify and fix Sub-tickets.
- A simple Bug collapses the placeholders into parent hops and closes the unused records `Canceled` with the reason.
- | Resolve the work | `ticket-captain` | The captain's compact acceptance contract names the lanes, or none; when the work splits beyond it, the writer's one pre-code comment names them. Parallel crew receive disjoint paths, the ticket's feature files among them, and exact gates; independently trackable parallel lanes become direct Sub-tickets. |
- Crew inherit the relevant ticket or Sub-ticket contract, owned paths and gates, and commit their own hop.
- Lanes have one level; the Bug stages below, Merge, an Inquisition's group Sub-tickets and map-holder-held Sub-tickets are the other permitted children.
- If a lane needs splitting, replace it with sibling lanes under the parent ticket.
- Keep one compact acceptance contract and point to evidence rather than copying it.
- Parallel crew write only the disjoint paths their Ticket Captain grants; the Bug Type-map exception below transfers paths serially.
- Fan-out is safe only across disjoint paths.
- Independently trackable parallel lanes may become direct Sub-tickets, each with an isolated branch and worktree.
- | Open a parallel lane | `ticket-captain` | The Sub-ticket carries the parent's Type and Mode, its own chain, status and evidence, a disjoint owned-path subset, exact gates, child-key branch, parent-branch base SHA and isolated worktree. |
- Each independently trackable parallel lane is a direct Sub-ticket of the captain's ticket.
- Reconcile existing active gate contracts explicitly before changing obligations.

## dispatch

- Under T3, the channel follows where the child's result is awaited.
- A native captain therefore cannot send its own Codex reviewer; a `delegate_task` captain can.
- A native child lives only inside its dispatcher's turn and cannot itself call `delegate_task`; T3 refuses with `parent_not_active` ("Delegated tasks require an active run owned by this MCP provider session").
- A native foreground subagent only when the dispatcher waits for it within its own turn.
- No native background subagents.
- A new top-level T3 thread only for HITL work the user takes over, or when the user asks for one.
- The default crew is one author - `code-writer`, or `docs-writer` for a documentation change - and fresh independent whole-change review, whose first round on a candidate seats two reviewers in parallel, one Claude and one Codex, their findings merged into one block that FAILs only on a blocker.
- Work that cannot finish inside the turn goes through `delegate_task` instead.
- A peer of another vendor is reached by T3 messaging.
- A proof-only brief names the commissioning captain, the pinned source SHA and the test path.
- A return comes back once, on the channel it was dispatched on.
- A saved brief is not Admiral-to-crew authority.
- A dispatcher waiting on a native child, or a child awaiting a result it owns (a reviewer, a suite), stays in its turn until the result arrives.
- `delegate_task` for every other child that returns to its dispatcher: captains, crew, reviewers, and any agent of another vendor.
- Without T3, use the host's native subagents and messaging, or none.

## escalation

- Crew hand their questions up to their captain and never ask the user directly.
- On `parent_not_active`, hand the dispatch up to your officer.
- On a bounded refusal, preserve the record, candidate, hop SHA and exact brief; do not broaden it or retry blindly.
- An officer that needs the user while its work runs asks through the host's question tool; T3 then lifts the thread into the user's queue.
- The Linear Question ticket stays the record.

## evidence

- | Build and prove | crew | Changes stay inside owned paths; exact gates pass; each hop ends on a commit `<KEY> <hop>: <what>`, the hop being `implement`, `fix` after a FAIL, `merge`, or `proof`, which commits as `<KEY> proof: <hypothesis>`; review evidence stays on the work item reviewed; every return comes back to the Ticket Captain. |
- This scale rule does not weaken required G/M, integration or release gates, and skipped native proof is never runtime proof.
- Required combined-merge and release gates stay at their boundaries.
- The captain pins those project-owned commands in Exact gates; the writer proves that hop's actual reach and the reviewer judges its record.
- Record only observed native evidence.
- The [testing strategy](./testing-strategy.md#feature-proof-and-gate-placement) owns selection, stage placement, runner prerequisites, shared-host constraints and exact-candidate evidence reuse.
- Validate uncertain native interface shapes early.

## identity

- One Ticket Captain owns the parent ticket and its integrated outcome end to end.
- The Ticket Captain owns the whole diff, including crew edits.
- The Ticket Captain consumes every crew return and remains accountable for the ticket, evidence and complete diff.
- Ordinary crew do not create its branch, open its PR, merge it or review their own work.

## inquisition

- | Inquisition audit | the audit SHA, the integrated feature commit | `inquisition/<slug>`; a group Sub-ticket's `inquisition/<slug>-<group>` | none; never merges | - |
- Audit, group and proof branches never merge.
- | Inquisition audit and proof branches | `ticket-captain` | Delete the audit branch at Done after the record and retention checks above, and the group audit branches at the parent's Done, and name any proof branch no open Bug needs; never merge them or delete the independently retained Bug refs. |
- Only then does it delete the audit branch and, as parent, every group's audit branch, name their remotes and any proof branch no open Bug needs on the record for the user, close `Done` and return `done <key>: merged`.
- Each group Sub-ticket owns its own Bugs' retention below and closes `Done` with its findings register final and pushed; the parent rechecks retention over the merged register before its record.
- After the record merges, it verifies on the feature the record at its path, every Bug filed and wired, and every retained ref live; a record only on an unmerged branch cannot satisfy this check.
- A prose Bug carries its quoted passages, an unused-member Bug its definition and reference search, and a drive Bug its rerunnable drive steps; none has a ref to retain.
- | Inquisition record | the feature tip; `main` for an audit of main | the Inquisition's `DYD-123-<slug>` | its feature branch; `main` for an audit of main | its captain-directed Merge Sub-ticket; for an audit of main, as an atomic ticket |
- An Inquisition runs its sweep, delivers its own record and closes once.
- The record is the Inquisition's last step, on its own ticket branch from the Branches table.
- A docs-writer writes it in `dydo/project/inquisitions/` from the pushed register, a fresh docs reviewer checks it, and it merges into the feature through the Inquisition's Merge Sub-ticket as any Project ticket's work does; an audit of main merges its record as an atomic ticket does.
- The audit branch carries the findings register, `inquisition-register.md`, pushed after every return.
- Before the sweep closes, every open Bug with a reproduction commit keeps its SHA reachable from a pushed named ref independent of the audit branch, linked on that Bug: its proof branch, or its patch branch when a fix was prepared.
- The Inquisition captain owns that retention until each Bug captain records adopting the reproduction and the cleanup responsibility.

## linear

- Linear assignment is the **claim**.
- Branch, base SHA and worktree path are posted on the relevant ticket or Sub-ticket before the first edit.
- | Claim the ticket | `ticket-captain` | Ticket is assigned; its branch and isolated worktree exist; branch, base SHA and worktree path are on the ticket. The captain sets `Implementing` when it spawns the author, a code-writer or a docs-writer. |
- An agent invocation is recorded as comments and evidence on the relevant ticket or Sub-ticket, never as another child.
- Successive agents may work through the same record.
- | Integrate a Project ticket | `ticket-captain` | Its final Merge Sub-ticket runs a code-writer - conflicts and combined gates mapped, the merge performed (a conflict-free merge of the reviewed PR into its feature branch is `dydo merge`, whose merge SHA is the candidate for the combined gates and merge review; a conflicted merge stays the local merge), a resolution that refactored leaving its code no worse than either side - then a fresh merge reviewer, preserving the merge commit and hop SHAs; the map holder wires the order and may advance an independent ready PR. Parent stays `Ready to Merge` until merge PASS, then both close `Done`. |
- A Merge Sub-ticket never enters `Ready to Merge`.

## merge

- | Atomic ticket | `main` | `DYD-123-<slug>` | `main` | user, clicking its PR once it carries a reviewer PASS and CI is green; no Merge Sub-ticket |
- | Integrate an atomic ticket | `ticket-captain` | No Merge Sub-ticket: the PR into main carries its reviewer PASS, the captain confirms CI green and sets `Ready to Merge`, and the user clicks the merge. The captain then closes the ticket `Done` and cleans up. |
- An atomic ticket has no Merge Sub-ticket: its captain sets `Ready to Merge` once CI is green, and after the user's click closes it `Done`, cleans up and returns `done <key>: merged`.
- A captain-directed Merge code-writer performs the specified merge and conflict resolutions; a fresh reviewer judges that integrated candidate.
- The Ticket Captain directs collision resolution and integration of review-passed branches through their Merge code-writers, then verifies the integrated result.
- A conflicted merge's push to its feature branch passes the guard, while a push to the default branch is blocked: the default branch is the user's.
- Project tickets integrate serially through their feature branch; the user alone lands the feature on `main`/`master` (the repository's default branch, whatever it is named).
- | Land the Project | user | The landing Merge Ticket prepares main into feature and obtains acceptance PASS; the user clicks feature into main as a merge commit, never squash. |
- A delegated captain whose PR still awaits checks returns `PR open <key>, checks pending: <url>, head <SHA>`; its officer, the top-level thread that owns the PR in T3, watches it and wakes the captain, which rechecks the gate before `Ready to Merge`.
- | Review and offer | `ticket-captain` | Passed lane branches are integrated into the parent ticket branch; combined gates pass; a fresh parent ticket-review PASS block is on the ticket and in the PR; the branch is pushed and the PR targets the branch in the table above. |
- It resumes when its Merge Sub-ticket's native blocker clears, or a fresh captain takes the record, and returns `done <key>: merged` after merge PASS and cleanup.
- Merge work proves instead the pins of the code-writer's `merge` resource: source and target at their SHAs, the source's PASS, the governing contract and the combined gates.

## project

- An invoked admiral wakes on a captain's return, the user's word or its scheduled check-in, and rereads the board, including released and blocker-cleared tickets.
- | Open the Project | the map holder, commissioning the first Ticket Captain | The first Captain opens the feature branch from the approved main SHA, commits the Project's working spec on it when the map holder hands one over, and reports both; the Project map is in Linear; every ticket carries outcome, owned paths, blockers, exact gates and base branch. Only then is a ticket pickable. |
- | Merged feature branch | the map holder, commissioning the landing Captain | The landing Captain deletes the branch after the user lands it and reports completion. |

## release

- For an uncleared blocker or user takeover: push, post the resume SHA, clean the worktrees the captain or its crew made (lanes, merge checkouts, temp checkouts) with `dydo clean --dry-run <worktree>`, then `dydo clean <worktree>`, run from outside that tree, set the parent `Todo`, unassign and wire any blocker; return `released <key>: <reason>`.
- A dead session is treated as release from its last recorded hop without a final push.
- Fresh commission from the record works on both hosts.
- It releases in two cases only, by [Return and release](#return-and-release): `released <key>: verification skill first` when a project with a runnable surface has no verification skill, and `released <key>: groups <keys>` when its area map is cut into group Sub-tickets, resuming as parent for the cross-group triage once every group closes.
- Use established lifecycle handling, then return or release the concrete limitation when captain-owned work cannot run.

## return

- The captain offers a PR with its PASS block, sets `Ready to Merge`, and returns `done <key>: PR ready`.
- The captain returns `done <key>: PR ready` when it offers the record's PR.
- If a later merge depends on it, use a following fix ticket.

## review

- Each corrected candidate gets fresh merge review.
- Merge FAIL, a blocker, stays owned: fix integration defects inside Merge; revert a source defect there and close Merge `Canceled`, returning the source to `Implementing`.
- Add a spec review of the contract before any code only when one short, concrete risk reason, recorded in the contract, calls for it; persistence, migrations, permissions and uncertain native interfaces are examples of such a risk.
- After integration it verifies the crew's combined proof and obtains the final parent review.

## worktree

- Audit cleanup never deletes those refs; they remain input to each Bug's normal chain.
- The posted base SHA is an ancestor of `HEAD`.
- Every crew **hop** ends on a commit: the return names its SHA, the Ticket Captain posts it on the record, and the ticket branch keeps its hops unsquashed and unrewritten.
- The record holds the detail; the captain posts each crew hop's SHA.
- The worktree is clean.
- Proof work, a proof-only brief or an Inquisition's Break phase, proves three things: `HEAD` is on the proof branch its brief names, cut from that SHA (the audit SHA for Break); the worktree is clean; and every path it touches is a test path.
- | Parent ticket and lane worktrees | `ticket-captain` | Every worktree it or its crew made (lanes, merge checkouts, temp checkouts) is cleaned with `dydo clean --dry-run <worktree>`, then `dydo clean <worktree>`, run from outside that tree. The worktree its thread runs in belongs to T3: leave it. Any worktree under `~/.t3/worktrees` was made by T3, a crew member's `delegate_task` checkout included, and `dydo clean` skips it. A spawned Ticket Captain first pushes the parent branch and opens its PR so the work survives its return. |
- Commits touch owned paths only.
- | Integrated lane Sub-ticket branch | `ticket-captain` | The branch is deleted after it passes review and is integrated into the parent ticket branch. |
- | Merged Project ticket branch | `ticket-captain` | The branch is deleted after the merge. |
- | Merged atomic ticket branch | `ticket-captain` | The branch is deleted after the user clicks its PR into main. |
- Deleting a branch here means the local branch.
- `dydo clean` deletes a local branch only once origin's default branch holds it.
- Every parent ticket has an isolated integration worktree.
- `HEAD` is on the relevant ticket or Sub-ticket branch.
- | Inquisition patch | the audit SHA | its Bug's own `<BUG-KEY>-<slug>` | as its Bug | as its Bug, through the Bug's own chain |
- | Inquisition proof | the audit SHA | `proof/<KEY>-<area>-<n>`, one per Break dispatch, `<n>` counting that area's dispatches, a follow-up or hand-back included | none; never merges | - |
- `DYD-123` is an example: use the ticket's key so Linear attaches the branch and PR.
- | Lane Sub-ticket | its parent ticket branch | `DYD-124-<slug>` | its parent ticket branch | its captain-directed Merge Sub-ticket |
- The worktree its thread runs in belongs to T3: leave it.
- GitHub deletes a merged PR's remote branch on merge, and the guard blocks agents from deleting remote branches, so the accountable officer names any other remote branch due for deletion on its record for the user.
- Every path the work item will touch is in the ticket's owned paths.
- The host may provide the ticket worktree; otherwise place it beside the repository at `../<repo>.worktrees/DYD-123-<slug>`.
- | Project | `main` | `feature/<project-slug>` | `main` | user |
- | Project ticket | its feature branch | `DYD-123-<slug>` | its feature branch | its captain-directed Merge Sub-ticket |
- | Proof branch | the pinned source SHA its brief names | a child branch named in the proof-only brief | none; never merges | - |
- | Prototype Ticket | its feature branch, else `main` | `prototype/<name>` | none; never merges | - |
- | Prototype branch | `ticket-captain`, tracked by the map holder | Keep the winning code linked as delivery-spec input; delete when that delivery ticket is Done or with feature cleanup. |
- Before the first edit of an `implement` or `fix` hop in a parent ticket or lane, its assigned writer proves all five checks and returns a failure to its Ticket Captain instead of working around it: `HEAD` is on the relevant ticket or Sub-ticket branch.
- The repository root is the isolated worktree, not the main checkout.
- Each writer stages owned paths by name; a whole-tree add can capture another writer's work.
- The `chief-of-staff` audits failures of this standard, not routine cleanup.
- The `chief-of-staff` compares `git worktree list` and merged branches with Linear during board hygiene.
- Anything it finds is a standard failure to clear or route with its owner named, not normal cleanup delegated to staff.
