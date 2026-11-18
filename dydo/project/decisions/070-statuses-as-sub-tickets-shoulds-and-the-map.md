---
area: project
type: decision
status: accepted
date: 2026-11-18
applies: [dydo-scarce-attention, dydo-aligned-intent, dydo-finite-context, dydo-efficiency, dydo-effort-right-sized-bureaucracy, dydo-design-reuse-before-build, dydo-verification-independent-evidence]
weighed:
  - "dydo-scarce-attention over dydo-efficiency, because hop sub-tickets add records but a planned step shows before it starts: \"So it's legible and quite simple.\" (C1)"
  - "dydo-aligned-intent over dydo-verify-own-work, because a git-tracked spec means \"different checkouts might have different versions of this folder and these files\", so the spec moves to Linear and loses `dydo check`'s link validation (C8)"
---

# 070 - Statuses as Sub-tickets, Shoulds and the Map

A ticket is a step on the map. Every step that runs its own review loop shows as hop sub-tickets,
so the status set shrinks to seven. Blockers and shoulds become tickets where they were found. The
spec is a Linear document and the contract lives on its ticket. `dydo map` is a roadmap at every
depth, the board is a view of `dydo survey`, and quiet work gets a liveness check. Decided by Balazs
in [grilling C](https://linear.app/bodnar-balazs/document/370-grilling-c-statuses-as-sub-tickets-dydo-map-board-query-99269898a59d)
on 2026-11-11 and 12 (C1 to C14, and the cut-rule side session of 2026-11-12), as superseded in
part by [grilling D](https://linear.app/bodnar-balazs/document/370-grilling-d-hierarchy-t3-inbox-captain-autonomy-sub-captains-a5cb8f0b5a86)
(D11, D16; 2026-11-13 to 15), D11 in turn replaced by
[DYD-1114 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1114#comment-88c79701-572c-4cba-b8e9-800088264a07)
(2026-11-17), with his L1 sitting of 2026-11-17
([DYD-1078 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1078#comment-9ac278c0-00f3-4688-9971-2693bfc14e38)).
Recorded under [DYD-1086](https://linear.app/bodnar-balazs/issue/DYD-1086).
Accepted by Balazs on 2026-11-18 (F10), for all three DRs at
`1ee7066e12a91fbfb3865e863d4ad628ccb733c0`: "1. a (I thought they were already accepted)"
([DYD-1195 Q1 DECIDED, envoy 2](https://linear.app/bodnar-balazs/issue/DYD-1195#comment-23474e54-10ed-49d7-8692-8651b054ced4)).
His 2026-11-18 rulings settle decisions 14 and 15 below. The
grillings say 3.7.0; that release is 4.0.0. Every rule is written once, through claims
([DR 064](./064-claims-build-the-skills.md), A-D13).

---

## Context

Linear's Dydo team had ten statuses, three of them hop statuses (`Implementing`, `In Review`,
`Ready to Merge`) under DR 047 §3. The statechart walk found their absolute claims broken: "In
Review always means a reviewer is running" failed three ways, and "the captain alone sets status"
four. Hop sub-tickets alone would not
help: on a 3.6.0-sized Project they take 277 records to about 865. The bookkeeping came from about
88 Merge records and 85 top-level should tickets. With [DR 065](./065-review-placement-and-the-merge-review-cut.md)'s
Merge hop and the shoulds off the root, the packet estimated about 390 records and 82 top-level,
the hops all inside plates.

`dydo map` opened everything expanded and rect-packed unconnected groups, with no time order.
Linear never auto-archives the issues of an unfinished Project and has no
manual archive. Its `delegate` field needs an installed Linear agent app; in 3.6.0, 190 of 277
tickets were assigned to Balazs and none had a delegate. 3.6.0 landed, yet its git-tracked spec
`dydo/reference/working-specs/dydo-3-6-0/safe-feature-merging.md` is still in the repository.
Grilling B left open where the spec lives and what "deleted" means for it.

## Decision

### Steps and their records

1. **A ticket is a step on the map (the cut rule; C2).** "A ticket is a step on the map that
   guides us to the destination of where we want to be." Tickets exist "to enable agent coordination
   on a huge scale and agent-sized tasks and make sure that things are observable so other agents can
   learn information quickly without having to read all the threads." "Okay, I think we have the
   simple version."
   - Who uses what the work leaves behind? **Only the agent who asked for it**: part of that
     agent's step, no ticket, off the record. **Anyone else**: a step, with a ticket. Off the record
     always: a native helper, a scout, a lookup, one review seat among several, a command, a file.
   - A step leaves a change, a judgement or an answer, kept as a check: "we may even keep it". A
     Project, a ticket, a sub-ticket and a hop are the same thing at different sizes. Depth has no
     rule of its own.
   - An agent reads its role off its step; who cuts, and the "too big, re-cut" return, are
     [DR 065](./065-review-placement-and-the-merge-review-cut.md) decision 15 (B8a; B3 and B3a, its
     decisions 12 and 13, stand). No agent judges its own step. An officer may take one of
     its own smaller steps ("I'd say yes"), as the captain takes its Merge hop. Who gets the Fix
     after a blocker is the officer's call ("I accept C"). The glossary's definition is the L1
     sitting's: "A ticket is a step on the Project map toward the destination, sized to what one
     agent can handle within its context."
   - **D11 retired** (DYD-1114 DECIDED); depth is [DR 071](./071-hierarchy-envoys-and-captain-autonomy.md)'s.
2. **Each step of a review loop is a hop sub-ticket (C1, C2a).** "before the implementation
   finishes the review ticket can already be there (in a todo or a not started way). So it's legible
   and quite simple."
   - Implement, Review (one per round), Fix and Merge each go Todo → In Progress → Done once, with
     blocking relations in step order. A hop may be opened ahead in Todo, blocked by the step before.
   - Hops appear only under a step that runs its own review loop ("yes", C2a). A leaf lane or a
     batched should is one ticket, taken in one go. A quick evidence check on a leaf is a Review hop
     beside it, under the same parent and blocked by it.
3. **Seven statuses (C1, C5).** FutureFeature, Backlog, Todo, In Progress, Done, Canceled,
   Duplicate. `Implementing`, `In Review` and `Ready to Merge` are deleted. A parent's In Progress
   means its officer holds it; a hop's means its one step runs. Whoever starts a step sets its hop
   In Progress and closes it when the step ends, replacing "the captain alone sets status".
   Statuses say where the work is; the Mode label says who it waits on. DR 047 §3's objections are
   met: the open hop's kind names the role, and a kind is set once and never flips.
4. **One agent, one ticket (C3).** "my view is that one agent is responsible for one ticket and
   potentially tickets within that ticket. And having two tickets side by side which are not wrapped
   by something could break this pattern." Several steps given to one agent are wrapped in one
   ticket it holds. Sub-tickets may nest ("go for it").
5. **A round with several seats is held by its officer (C2b).** "yes". A lone reviewer holds its
   Review hop. With several seats, the officer that opened the round holds it and posts one verdict
   naming each seat (model and provider) and the blockers each found.

### The review loop on the board

The loop itself is DR 065 decision 7 (B6). C3 says how each outcome shows.

6. **Blockers are tickets inside a Fix ticket per round (C3).** "in my mind that blocker is a ticket
   (so when created it is registered). And then the actual fix happens on that blocker ticket. So
   when the blocker is marked done, it's fixed." "Just put those tickets within that fix ticket.
   problem solved."
   - No new blocker: Review Done, then the Merge hop or up a level.
   - Blockers: the round's holder opens **Fix rN** in Todo and registers one Blocker ticket per
     blocker inside it at verdict time. A Blocker goes Todo → In Progress → Done. Fix rN is Done when
     its blockers are Done and proof ran on the head; the next Review waits on every Fix of the round.
     For parallel writers the captain splits Fix rN into Fix rNb before dispatch.
   - A re-plan: open hops that no longer apply go Canceled with a one-line reason; the re-cut is new
     sub-tickets. Needs Balazs: a Question ticket blocking only what waits on it.
   - A should: decision 11. A nit, unchanged: "We should keep it that way." One line on its record;
     an inquisition round's nits go to that round's nit ticket.
   - Round 3 with a new blocker: no ticket, one line on round 3's Review: "checkpoint: one more round,
     because …" or "escalated: …".
7. **The verdict lives on the Review sub-ticket (C4).** "yes", after a plain restatement. The round's
   holder writes it once there: PASS or FAIL, lenses, seats, shoulds. The PR body carries only the
   one-line PASS block, naming its Review ticket. The parent gets no copy; the map and the board show
   the latest verdict, computed. `dydo merge` checks that the named Review is Done with a PASS on the
   head, or on an ancestor under DR 065 decision 17.
8. **Merges on the board (C5 as narrowed by D16; C6).** On Ready to Merge: "What do we gain from
   having ready to merge as a status?"
   - A merge into the feature branch is an **AFK Merge** sub-ticket the captain runs. A merge into
     main is an AFK Merge when two-way and a **HITL Merge** only when one-way (D16). Open and
     unblocked, a HITL Merge means his click is due; the captain closes it after verifying the merge
     on main. Which merges need him is [DR 071](./071-hierarchy-envoys-and-captain-autonomy.md)
     rules 13 to 19; the landing is DR 065 decision 19.
   - A real red gate after a two-way merge ("lgtm", C6): the rerun, the revert, the fix-scoped round
     and the flake are DR 065 decision 14. C6 adds the board states: the reverted Merge goes
     **Canceled** with "reverted: <gate> red on <merge SHA>", because Done would be false; the red
     gate is a Blocker inside a Fix (C3); a new Merge sub-ticket follows the fix-scoped Review.
     Revert applies to two-way changes only; a one-way PR carries a recovery plan instead (D16,
     DR 071 rule 15).

### Identity and the spec

9. **Agent identity is the T3 thread link (C7).** "Accepted." Whoever sets a ticket In Progress
   attaches the holder's thread link in the same step, titled "T3 · <role> · <model>": the link
   `delegate_task` returns, or an agent's own. Not the assignee, not Linear's `delegate`. Model and
   provider stay in the comment signature. To verify at build: Linear accepts a `t3-thread://` URL;
   if not, a one-line comment carries it. C7's bullet "The admiral's lane ledger keeps its lane →
   captain link" goes with the ledger (decision 15).
10. **The spec lives in Linear; the contract stays on its ticket (C8).** "What I think of as a spec
    is the shared understanding between me and the agents … formed through a grilling session, or
    multiple ones, and refined along the way." "Move it. And I also like the idea of calling the spec
    a spec and the contract a contract because they are not the same thing."
    - **Spec:** one Linear document on the Project, "Spec: <Project>", with today's headings; a big
      feature may have its own. The map holder writes and refines it; Linear keeps its history.
    - **Contract:** the expected outcome of one ticket, in its description. It quotes and links the
      spec sections it derives from, and the frozen quote replaces pinning a spec commit. A refined
      spec: the map holder reconciles the open contracts that quoted the changed sections.
    - At acceptance, the landing walkthrough, lasting knowledge moves to DRs, the glossary, docs and
      the verification map; then the spec document is deleted. This answers B4a's "deleted"
      (DR 065 decision 4). "A scaffolded Project always gets a spec" (DR 053's amendment) stands.

### Shoulds, the map and the board

11. **Shoulds live where they were found and are collected in batches (C9).** "The shoulds should be
    tickets under the ticket where they appeared… I 100% do not want them to clog up the root level
    because that would diminish my ability to use it as a roadmap… it's not a blocker because it is
    not blocking now, it's blocking later." "I like this design."
    - The round's holder registers a should as a child of the ticket it concerns, Severity should,
      after checking that ticket and its ancestors for a duplicate. Its parent may close Done while
      it stays open.
    - The admiral cuts a "Shoulds batch N" root ticket for one captain: fixed in place before the
      parent merges if cheap, when a lane is free, before an inquisition round, or at the landing.
      The batch captain re-parents them, keeps "related: found in DYD-xxx rN", and marks duplicates.
      No standing collection ticket.
    - At the landing every should is Done, Canceled with a reason ("accepted as is"), or moved to
      another Project by his call. **R4** ([note](https://linear.app/bodnar-balazs/issue/DYD-1086#comment-29607e5e-2e01-4534-9532-92ef2b8a8cd9)):
      this rule is not part of the Linear workspace standard; it lives in the admiral's landing step.
      The L1 proposal had placed it in the standard, citing C9. C9 itself puts it in the landing (its
      Moves, step 4), and its one line for the standard is Linear setup: Sub-issue auto-close stays
      off. The standard carries the landing check today (`linear-workspace-standard.md:113-114`).
12. **The map is a roadmap at every level (C10, C11, C12).** "the map should serve as a roadmap and
    within individual tickets when I expand them they should be their own little roadmap and that's
    recursive" … "there should be the opportunity to dig as deep as the work needs it to be."
    - It opens on the top-level tickets: open ones plus those closed in the last 3 days, faded, the
      view held in the URL. Expanding a card lays out its children the same way, at any depth. A card
      shows its current hop ("now: Review r2") and chips: open shoulds, a blocker in another group,
      liveness. The visual design stays open.
    - Order, computed and never stored ("I like this."): within a connected group a blocker sits
      left of what it blocks; groups are ordered by their first ticket's createdAt, oldest left; the
      longest chain is the top row.
    - Continuous Projects are never rotated: "old tickets should be hidden from the view." Reads
      filter server-side on state and completedAt, never fetch-all-then-filter.
13. **The board is `dydo survey board <project>` (C13).** "we already have a command called … dydo
    survey and that kind of already handles the smart querying of tickets. So I feel like this board
    should be part of that, so like dydo survey board." A second view over the survey's one read:
    open tickets by group with current hop and holder, open shoulds with their age, open nits, HITL
    Merges waiting on his click.
    New survey findings: a thread link on two non-nested open tickets, a landed Project that still
    has a spec document, open shoulds under Done parents past a threshold, and `liveness`.
14. **Quiet work gets a liveness check, not a stale verdict (C14).** "we shouldn't declare it stale
    just because it hasn't been moved for two hours." A hop In Progress with no change for 2 hours,
    or a ticket In Progress with no open hop for 2 hours, raises `liveness`. The officer above
    checks the thread link and the PR, then comments `liveness <UTC time>: <outcome>`: alive (snoozed
    one window), record behind (the record is brought up to date), or stopped (the hop returns to
    Todo and is resumed or re-dispatched with a new link). Quiet time runs from the later of the last
    change and the latest `liveness` comment, so dydo keeps no state. Balazs, 2026-11-18:
    "2 hours for both."
    ([DYD-1195 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1195#comment-77285ec6-fb47-4717-9088-9a6ac1049067);
    [envoy 1 on PR 358](https://github.com/BalazsOrg/dydo/pull/358#issuecomment-6982693932)).
    The 12-hour window goes. The chain above is sub-captain → captain → admiral → chief of staff
    (D7); the chief of staff's quiet-Project check is an overnight-mode double check only
    ([DR 071](./071-hierarchy-envoys-and-captain-autonomy.md) rule 4).
15. **The lane ledger retires (C13's flag, settled 2026-11-18).** Balazs:
    "I don't even know what the lane ledger is, sure. Retire it."
    ([DYD-1195 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1195#comment-53a8d802-6590-4a65-9318-8ce9617830c4);
    [envoy 1 on PR 358](https://github.com/BalazsOrg/dydo/pull/358#issuecomment-6982020734)). C13 flagged
    that with the top-level tickets as the roadmap, the ledger and its findings may be redundant.
    Every ledger column has a home on the ticket: the lane is the top-level ticket (decision 12);
    Owner is its thread link (decision 9); State and Next action are its status and current hop
    (decisions 2, 3, 12); Order is the computed layout (decision 12); the Filler row's shoulds are
    C9's batches, cut when a lane is free (decision 11). The board prints them all (decision 13).
    One agent holds one open top-level ticket at a time (decision 4), so a lane of several tickets
    is a chain of blocking relations. Keeping the ledger
    would give each claim two homes, and `no-ledger`, `unlaned` and `stale-row` would only keep the
    copy in sync. They retire with it, and so does `started-unassigned` (C7). This ruling drops
    C7's accepted bullet "The admiral's lane ledger keeps its lane → captain link" (decision 9).

## Consequences

- **Enablement:** delete `Implementing`, `In Review` and `Ready to Merge` in the Dydo team; keep
  Sub-issue auto-close off; re-home in-flight tickets by hand.
- **Linear workspace standard,** rebuilt from the model (DR 064 decision 11): the status table and
  type paths; "the captain alone sets"; "A review blocker stays a FAIL and never becomes a ticket";
  one-level-deep sub-tickets; and, per R4, the landing check
  (`dydo/reference/linear-workspace-standard.md:32`, `:41-49`, `:112-114`, `:144-146`, `:155`).
- **Code:** `LinearReader` reads timestamps and attachments with server-side filters; the map
  renderer opens collapsed, applies one layout at every depth and keeps the window in the URL;
  `dydo merge` reads the named Review; the survey gains `board` and its new findings.
- **The ledger's consumers,** follow-up work, untouched here:
  `skills/roles/officers/admiral/resources/check-in.md` (its Ledger section, Tick and Survey lines,
  the `fix ledger` verb, the fixed dispatch) and the admiral's Reconcile step
  (`skills/roles/officers/admiral/SKILL.md:74-78`), with their claims; `Services/Survey/Ledger.cs`,
  `LedgerRow.cs`, the ledger branch of `SurveyRules.cs` (the only reader of the Project body,
  `:24`), `Commands/SurveyCommand.cs`'s description and their tests and `survey.feature`;
  `dydo/reference/dydo-commands.md` §dydo survey; the glossary's Lane entry; and the `Scaffold/`
  copies.
- **The spec move,** follow-up: `feature-spec-convention.md`, `_working-specs.md`,
  `admiral/resources/scaffold-project.md`, `working-tree-standard.md:49`, `about.md:27`, the
  glossary's Feature spec entry (Spec and Contract, through domain-modeling); retire the 3.6.0 spec.
  Lost: `dydo check` validates no links inside the spec.
- **Open:** "Contract" also names the working-tree and architecture contracts; whether DYTEST changes
  the CI findings Project is the admiral's to route.

## Supersedes and amends

- Supersedes [DR 047](./047-supersymmetry-hop-statuses-merge-issues-and-the-release-protocol.md) §3,
  the hop statuses (C1, C5).
- Supersedes [DR 053](./053-temporary-feature-specs.md)'s location and its 2026-11-05 amendment's
  location (C8); "a scaffolded Project always gets a spec" stands.
- Amends [DR 063](./063-dydo-survey-read-only-lane-digest.md) §3-4: the ledger and its findings
  retire, `started-unassigned` goes, and the survey gains the board and new findings (C7, C13, C14,
  decision 15).
- Amends [DR 052](./052-dydo-map-read-only-linear-view.md) §5: the map's default view and order
  (C10 to C12).
- Amends [DR 058](./058-review-severities-blocker-should-nit.md) §1, where a blocker returns the
  ticket to `Implementing` and a should is filed in the Project's `Backlog` (C1, C3, C9); and
  [DR 065](./065-review-placement-and-the-merge-review-cut.md) decision 7's "should collection" (C9).

## Related

- [DR 064 - Claims Build the Skills](./064-claims-build-the-skills.md) - Every rule here is written once, through claims
- [DR 065 - Review Placement and the Merge-Review Cut](./065-review-placement-and-the-merge-review-cut.md) - The Merge hop, the review loop and the landing
- [DR 071 - Hierarchy, Envoys and Captain Autonomy](./071-hierarchy-envoys-and-captain-autonomy.md) - Which merges need him, the recovery plan, depth
- [dydo Glossary](../../reference/dydo-glossary.md) - Ticket
