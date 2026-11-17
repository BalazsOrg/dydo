<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Run an Inquisition

## Contents

- [capacity](#capacity)
- [dispatch](#dispatch)
- [escalation](#escalation)
- [identity](#identity)
- [inquisition](#inquisition)
- [linear](#linear)
- [release](#release)
- [worktree](#worktree)

## capacity

- the Codex cap: start about 8 at once, go toward 16 while runs stay clean, back off on a 429;
- a provider and model for each phase, and the swarm width: the most crew agents running at once;
- the crew context limit, 110k tokens unless the commission set another;
- Split the commissioned swarm width and its Codex share between the groups, write each group's share on its Sub-ticket, and keep the totals inside the commission; a finished group's share returns to your officer, who may give it to a running group.
- Keep up to the swarm width running, with Codex inside its cap.

## dispatch

- A return with work items handed back gets a fresh agent for the rest of the area, briefed with those items; a hand-back or follow-up Break agent gets the next `<n>`.
- Brief each agent with its phase, its area's map row and register entries, the audit SHA and landing base, the governing intent, its context limit, and its worktree: one at the audit SHA for Read and Drive; for Break, one on its own proof branch `proof/<KEY>-<area>-<n>` cut from the audit SHA, with the test paths it may touch, `<n>` counting the Break dispatches for that area; for Drive, the verification skill and the API base.
- Dispatch each as its own session through T3 `delegate_task` (DR 057): crew outlive your turn, and the context check needs its own session.
- Dispatch one agent per area per phase, and set no quota of hypotheses or journeys.
- Merge tiny areas into one area here; a dispatch never carries more than one area.
- Wire the parent blocked by every group, open this round's nit ticket (step 8), then release the parent by the Release section with `released <key>: groups <keys>`; your officer commissions one captain per group.

## escalation

- **Judgment call:** a Question for the user, prepared by the standard's Question Tickets rules.

## identity

- You conduct; the crew hunts.

## inquisition

- Done when every changed path sits in exactly one area, every seam is an area, and every cell holds `yes` or a one-line reason.
- Draw it from `git diff --stat <landing base>..<audit SHA>` and the file layout, never from the feature list or an earlier round's map.
- Post it on the ticket as a table: | A-01 | the files or folder | one line | yes | yes, or why not in one line | yes, or why not in one line |
- **Small on purpose:** a big area gets superficial coverage.
- One file can be an area.
- An audit with no Project files a team ticket, base `main`, with no landing relation.
- Done when branch, audit SHA and worktree path are on the ticket and the register's first commit is pushed.
- Cut `inquisition/<slug>` from the audit SHA in your isolated worktree; it never merges.
- The inquisition merges nothing but its own record.
- **Behaviour:** Break and Drive apply wherever the landing changed behaviour; Drive says `nothing to run` on a project without a runnable surface.
- **Proven blocker or should:** file a Bug: Mode `AFK`, Severity `blocker` or `should` matching the finding, in the Project, base the feature branch, `## Reproduction` holding its proof (the red-test SHA and test, the quoted passages, the definition and reference search, or the rerunnable drive steps), linked to this Inquisition.
- Its fix may merge into the feature mid-round: the audit is pinned to its SHA.
- A blocker goes to `Todo`, a should to `Backlog`, natively blocking the Project's landing Merge Ticket when it exists.
- You never hunt yourself: reading code for bugs is the crew's.
- Close the Inquisition `Done` and return `done <key>: merged`.
- Done when the ticket's `## Scope` records: the audit SHA, the integrated feature commit, and the landing base it is diffed against; the governing intent: the Project plan, the landed tickets' contracts, the Decision Records; a provider and model for each phase, and the swarm width: the most crew agents running at once; the Codex cap: start about 8 at once, go toward 16 while runs stay clean, back off on a 429; the crew context limit, 110k tokens unless the commission set another; the project's verification skill, or the one-line reason nothing in the project runs.
- Done when every applicable phase of every area has returned.
- A disposition is a Bug key, `patch <SHA>` on its Bug, the nit ticket, a Question key, or `refuted` with the observation.
- An Inquisition files outcomes; it has no PASS or FAIL.
- IDs run `F-01`, `F-02` and so on; a group qualifies its IDs with its group, as `G2-F-07`.
- **New finding:** add a row with the next ID.
- A group captain runs steps 2 and 5 to 10 on its rows, owns its Bugs' retention, and closes `Done` with its register final and pushed.
- Group captains add their nits to your nit ticket.
- One captain conducts about 10 areas.
- Above that, cut the map into group Sub-tickets of up to about 10 areas each, seams kept with the areas they join.
- Each group Sub-ticket carries the Inquisition Type, the parent's Mode and commission, its map rows and its own `inquisition/<slug>-<group>` branch from the same audit SHA.
- **Nit:** one line on this round's nit ticket, with its `file:line` and the wrong outcome; it needs no proof.
- The map holder lists every open nit ticket at the Project's final walkthrough, where the user decides each nit; it closes `Done` once every nit is fixed or accepted as is.
- Open the nit ticket with the first nit: Type `Feature`, Mode `AFK`, Severity `nit`, `Backlog`, held by the map holder, linked to this Inquisition.
- A Bug whose fix step 9 will prepare is filed assigned to you, the claim that keeps it unpicked, naming its patch branch; any other Bug is filed unassigned and pickable at once.
- When the groups close you resume as parent: merge their registers into yours, join duplicates across groups as sightings, and close a duplicate Bug as `Duplicate` of the one kept.
- Done when every area belongs to one group and each group's register is merged into yours.
- Then run step 10 over the merged register and deliver the one record.
- It cherry-picks the finding's red-test commit first, then commits the fix that turns that test green.
- Brief a `code-writer` in Bug mode on the Bug's own branch, `<BUG-KEY>-<slug>`, cut from the audit SHA, with the Bug's base SHA and owned paths posted on it.
- A blocker or should that Break proved with a red test, whose fix is **well-defined**, gets it prepared now: one obvious fix, inside one area, small, touching no contract, Decision Record or public interface.
- Any doubt means no patch.
- Nits are never patched.
- When the lane fails, the fix not well-defined after all or the code-writer returning blocked, record the reason on the Bug and unassign it without a patch.
- Attach the branch and SHA to the Bug, then unassign it: it is pickable at once.
- The Bug's captain verifies and reviews the patch once, in the Bug's own chain.
- Done when every Bug filed for the lane is unassigned with its patch attached or its reason recorded.
- Run the phases as a pipeline across areas, never in waves: an area's Break starts when its Read returns, its Drive when its Break returns.
- After the record merges, verify on the feature: the record at its path, every Bug filed and wired, every retained ref live.
- A prose, unused-member or drive Bug carries its proof in its body and has no ref to retain.
- **Prose:** a plain skill or doc area gets Read only; its Break and Drive cells say `prose`.
- **Break's commits:** push the proof branch; its red-test SHAs are reproductions.
- Brief a `docs-writer` with the pushed register: it writes `dydo/project/inquisitions/<YYYY-MM-DD>-<slug>.md` naming the audit SHA, the commission, the area map with each phase run or its reason, every finding with its severity, sightings and disposition, every unreachable journey with its disposition, the Bugs and patches filed, the nit ticket and the Questions, an empty section recorded as such.
- The record is this Inquisition's own last step, through the ordinary docs chain on its ticket branch `<KEY>-<slug>`, cut from the feature tip.
- Set `Implementing`, then `In Review` for a fresh docs `reviewer`, then offer the PR and its Merge Sub-ticket into the feature as any Project ticket does; an audit of main merges its record as an atomic ticket does.
- Done when the record's merge review passes.
- **Unproven:** stays open in the register until its routed phase returns; a refuted one closes `refuted`, an inconclusive one stays in the record with its deciding observation.
- Create the findings register (step 5) on it and push.
- `inquisition-register.md` at the root of the audit branch, one row per finding:
- Commit and push the register after every return, before the next dispatch.
- Done when every return is in it and pushed.
- Done when every applicable phase of every area has returned, every register row has a final disposition, the register and every proof branch are pushed, and each open Bug's reproduction SHA is reachable from a pushed named ref independent of the audit branch: its proof branch or its patch branch, linked on the Bug.
- You own that retention until each Bug's captain records adopting it; audit cleanup never deletes those refs.
- Each round is its own Inquisition, started from zero: you never read the previous round's map or findings.
- The map holder offers the next round once every blocker and should Bug is fixed.
- **Tagged finding:** route it to the phase that can prove it, `testable` to Break and `drivable` to Drive.
- When that phase already ran for the area, send a short follow-up agent with only the routed entries as its work items.
- Done when every finding of the return is a row or a sighting and every tag has a phase scheduled.
- Map the landed work into small areas, run Read → Break → Drive on each through a swarm of `inquisitor` crew, triage every return, then deliver the record.
- **Seams:** where two areas meet through a call, a shared type, a file format or a protocol, the seam is its own area.
- Check each severity against the source, not the crew's label, and record any change with its reason in the row.
- A later sighting of a Bug, even a fixed one, adds a reproduction to it.
- **Known bug:** a finding matching a register row is a sighting on that row, never a new row.
- The user has confirmed it.
- Tags are `testable` and `drivable` until a phase proves or refutes the finding.
- Done when every register row has a disposition or a routed phase in flight.
- **Unreachable journey:** a register row with its prerequisite and the route tried.
- Before that area's Drive counts as returned, dispose of it: a follow-up Drive once the prerequisite is met, a verification-gap should, or a Question.
- A verification gap is a should whose Bug names `maintain-verification-skill` as its method.
- A project with a runnable surface and no verification skill stops here: the skill comes first.

## linear

- Otherwise the ticket stays `In Progress` from here through step 10.

## release

- Release with `released <key>: verification skill first`.

## worktree

- Delete the audit branch, and as parent every group's audit branch, locally, and name their remotes, and any proof branch no open Bug needs, on the record for the user.
