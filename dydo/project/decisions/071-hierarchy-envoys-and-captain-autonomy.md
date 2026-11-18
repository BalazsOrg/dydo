---
area: project
type: decision
status: proposed
date: 2026-11-18
applies: [dydo-scarce-attention, dydo-attention-human-decisions, dydo-aligned-intent, dydo-verify-own-work, dydo-verification-independent-evidence, dydo-effort-right-agent, dydo-effort-right-sized-bureaucracy, dydo-everything-evolves, dydo-design-reuse-before-build, dydo-efficiency]
weighed:
  - "dydo-attention-human-decisions over dydo-efficiency, because \"bugging me with something which can be answered reliably in another way is a sin. My time is the scarcest resource of all. I can't scale like agents.\" (D-D9)"
  - "dydo-scarce-attention over dydo-efficiency, because one active envoy per admiral is \"better flow than me drowning in envoys\" (D-D8)"
  - "dydo-effort-right-agent over dydo-efficiency, because admirals \"may have not been the best equipped to do this, because they had a too wide of a job description\" (D-D15)"
  - "dydo-scarce-attention over dydo-aligned-intent, because \"me clicking merge is a bottleneck\" and the click on a two-way PR \"is only good for knowing what's been happening, but I have other ways to do that as well\" (D-D14, D-D16)"
  - "dydo-everything-evolves over dydo-effort-right-sized-bureaucracy, because \"if crazy nesting behavior does emerge - and it's bad - we'll deal with it\" (DYD-1114)"
---

# 071 - Hierarchy, Envoys and Captain Autonomy

A moving Project has an admiral, which holds its outcome and keeps the work moving; hard calls are
not the admiral's. Balazs meets the fleet through envoys: fresh HITL agents that only the admiral
sends, one at a time, after the homework is done. The chief of staff triages across Projects and
never relays. Sub-captains are allowed now, with no hard cap on depth. A two-way PR into master
merges AFK on complete evidence; only one-way PRs and release tags need his click. Decided by
Balazs in
[grilling D](https://linear.app/bodnar-balazs/document/370-grilling-d-hierarchy-t3-inbox-captain-autonomy-sub-captains-a5cb8f0b5a86)
from 2026-11-13 to 15 (D1 to D16), with his
[DYD-1114 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1114#comment-88c79701-572c-4cba-b8e9-800088264a07)
of 2026-11-17 replacing D11's interim rule on sub-captains, and R5 of his L1 sitting of 2026-11-17
([DYD-1078 DECIDED](https://linear.app/bodnar-balazs/issue/DYD-1078#comment-9ac278c0-00f3-4688-9971-2693bfc14e38))
on which Projects have an admiral. Recorded under
[DYD-1087](https://linear.app/bodnar-balazs/issue/DYD-1087); the same change resolves
[DYD-1177](https://linear.app/bodnar-balazs/issue/DYD-1177). Proposed, awaiting Balazs's acceptance
(F10). The grillings say 3.7.0; that release is 4.0.0. Every rule is written once, through claims
([DR 064](./064-claims-build-the-skills.md), A-D13).

---

## Context

The admiral's thread did two jobs that should be kept apart: it was where Balazs spent his
attention, and where the fleet's bookkeeping landed (PR watches, relayed crew returns, check-in
ticks, empty completions). Grilling D's packet traced gaps that each made him look himself: crew
dispatched for native captains returned to the admiral (T1); with no admiral, nobody resumed a
captain after a Question (T3); nobody woke an atomic ticket's captain after his click (T4); and a
Project without a live admiral did not move (T5). [DR 051](./051-captains-by-default-admiral-on-invocation.md)
§3-4 had made the admiral optional.

Three more questions were open. May a captain do small changes itself
([DYD-910](https://linear.app/bodnar-balazs/issue/DYD-910))? How deep may officers nest, given
that T3 had shown depth 2 below a top-level thread and nothing deeper? And which merges need his
click, when "me clicking merge is a bottleneck" (D14)?

Grilling D's answers are speech-to-text and are quoted as the grilling records them, with only
spelling normalised.

## Decision

Recorded elsewhere, not restated here: a captain implements only what is too small to need a
review of its own (D10) and how a landing is accepted (D16's landing) are in
[DR 065](./065-review-placement-and-the-merge-review-cut.md); what-would-balazs-do as one use of a
general council (D12) and the council that takes hard calls (D15's council side) are in
[DR 066](./066-the-law-councils-and-the-tribunal.md); dispatch templates (D13) and the doctrine
living in built skills (D4's first rule) are in [DR 064](./064-claims-build-the-skills.md).

### Who holds what

1. **His attention lands in HITL threads he works with, not in admiral threads (D1).** "I work
   within T3 code." What needs him reaches him through HITL agents: HITL Merges, Questions,
   escalations, walkthroughs, grillings and giving out work. "HITL agents are where I am usually
   interacting." Visiting each admiral's thread is out, and so is anything outside T3 (Project
   updates, digests). `dydo map` is his strategic lens, not the operational surface: "It's a map
   for me a lens. A strategic thinking tool. Not the tactical and operative part." D1's parts are
   answered by rules 2, 4 and 8 to 10.
2. **The admiral holds the outcome: the Project reaches its destination (D2, L1 R5).** "Yes, B is
   probably what the admiral should be accountable for, to make sure that the project moves towards
   its destination. They are kind of doing what the individual captains are supposed to do with
   their own tickets. but the admiral's ticket is the project so the self-similarity stands but they
   are at the root level of this hierarchy."
   - The admiral holds the Project as its step (C2) and answers for the destination being reached.
     It watches, and keeps re-charting the route as the work evolves: it cuts the next tickets,
     re-plans when a blocker says the approach was wrong (B6), and sequences the work.
   - Its watching is mostly deterministic (survey, board, liveness findings in a loop), so its
     context goes to charting and routing, not bookkeeping.
   - Which Projects have one, in his words at the L1 sitting: "all open projects have an agent
     assigned to them with the role admirals." Closed, not-started and idle Projects have no live
     admiral. The two continuous Projects are the exceptions (rule 5).
   - The alternatives were dropped: a watcher that charts each route with him, and no admiral
     with every step a wayfinding session with him. Either way the route would move only while he
     is present.
3. **The admiral coordinates; hard calls are not its job (D15, D2).** In his words, unprompted:
   "the admiral can just focus on keeping the lanes moving and the project moving towards its
   direction and making sure that everything stays in sync and that's not a small task in itself so
   I think I would probably waive the burden of making hard calls from the admirals, and they should
   defer it to a dedicated council." Only forks it can't decide come to Balazs: direction, spec
   gaps, walkthroughs, things only he can do. A hard call goes to the council at the homework step
   (rule 9), and the admiral routes the verdict; the council, the tribunal and their exits are
   [DR 066](./066-the-law-councils-and-the-tribunal.md)'s. An obvious, reversible call stays
   ordinary judgement for whoever holds the step.
4. **The chief of staff triages across Projects and never relays content (D5).** "I think this is
   good. Also I find myself using them for two scenarios - overnight watch, so there's one
   coordinator, and during the day I almost never use them, maybe for big, cross project thinking
   or if I want to find out something about an unknown thing (like who is eating my ram, or why do I
   hear certain voices being played, etc)."
   - It reads `dydo survey board` across every Project and keeps one short list for him: HITL
     threads waiting on him, HITL Merges waiting on his click, anything waiting too long. Each item
     is a thread link; he goes to the thread itself. It never carries content between him and an
     admiral, and sends no envoys.
   - It is the officer above the admirals for liveness only (C14): it checks an admiral that has
     gone quiet and catches an open HITL ticket with no envoy. Recorded in the grilling as a
     default, not asked: the admiral attaches its T3 thread link to its Project (as C7 does for
     tickets), and a Project with open work and no ticket moving for 12 hours raises the check.
     *Balazs to confirm.*
   - Its uses: the overnight watch (overnight-mode, [DR 064](./064-claims-build-the-skills.md)
     decision 8), big cross-Project
     thinking, and investigating an unknown on his machine. By day it is mostly idle, not a standing
     inbox he must check.
5. **Continuous Projects are queues with no admiral (D6).** "b and c". A continuous Project (Agent
   friction, CI findings) has no destination and no admiral. The chief of staff lists its open
   tickets with their age; Balazs gives a ticket out, or tells the chief of staff to. An admiral
   with a free lane may take a continuous ticket and commission a captain for it, within a standing
   rule Balazs sets once (for example, Bugs up to Severity should, no HITL Mode); outside the rule it
   asks. Each such ticket still gets a captain, and review and merge as usual. The standing rule's
   text is his to set. Recorded in the grilling as a default, not asked: the officer that
   commissioned the captain oversees the ticket (the admiral that pulled it, or the chief of staff
   when he gives it out through it); it is the ticket's officer above, sends its envoys and paces
   its merge. *Balazs to confirm*, including a conflict the grilling leaves open: through the chief
   of staff this default has it send envoys, which rule 4 says it doesn't (D5), and rule 9 gives
   every envoy in a Project to the admiral, which a continuous Project lacks.
6. **The officer-above chain (D7, D9).** "c" (D7). The officer above, per level: sub-captain →
   captain → admiral → chief of staff. The same chain carries liveness (C14), re-plans (B6) and
   escalations. Above the admirals the chief of staff takes liveness only; an escalation that needs
   Balazs goes from the admiral through its own envoy, not through the chief of staff. D7's "an
   officer sends its own envoy for its own step" is withdrawn by D9 (rule 9).
7. **Sub-captains: allowed now, with no hard cap on depth (DYD-1114, replacing D11).** D11 held
   sub-captains back until the T3 nesting experiment reported: "let's do the experiment first and
   we'll be smarter about judging it". The experiment
   ([DYD-1072](https://linear.app/bodnar-balazs/issue/DYD-1072)) reached crew at depth 4. His ruling
   on it, live with the admiral's envoy: "I did want to wait for technical feasibility, but as it
   seems to me, it is technically feasible, so I wouldn't necessarily want to put a hard cap on how
   many levels can we have? In practice, we should probably not go deeper than four or five levels.
   So Admiral, captain, subcaptain, second order subcaptain, and its crew. That should be the
   practical limit. but I'm not sure we should limit it otherwise. … a ticket captain subcaptain
   will almost always be enough and a second order subcaptain may occasionally appear but I'm not
   sure that there is any task where realistically we would have a third order subcaptain." His
   correction to the envoy's paraphrase: "I said if crazy nesting behavior does emerge - and it's
   bad - we'll deal with it. If the third order sub captain thing fails we deal with it as well."
   - No hard cap. A sub-captain is self-similar (D2, B8a): a ticket-captain on a Sub-ticket with
     children.
   - The expected practical ceiling is admiral → captain → sub-captain → second-order sub-captain
     → crew. A third-order sub-captain is not expected in practice.
   - **Cutting stays with the officer above (B8a).** "I like the idea of oversight for cutting
     things so one agent (officer) can't unilaterally decide to cut things. The fear we have is that
     agents will keep passing things down. That's not what we want. What we want is for each more
     complex coherent work unit to get it's own orchestrator (who we call captains). If the work is
     too big for one agent to handle well and/or it can properly be parallelized, "cut" then it gets
     done so." The cutter decides; no agent promotes itself; "too big, re-cut" goes up. Nesting
     follows coherent units that are too big for one agent or can be parallelised, never passing
     work down.
   - **No pilot:** "a" (allowed now; the first real use is the pilot). If deep nesting misbehaves,
     or a deeper sub-captain fails, it is handled then as a finding.
   - The record adds, with no quote of his: no per-level budget split, because the per-brief cap is
     being retired by B7's model table and `dydo usage`
     ([DR 065](./065-review-placement-and-the-merge-review-cut.md) decision 8).
   - Until this ruling, D11 had a ticket too big for one captain cut into sibling tickets under a
     parent the admiral holds. That interim rule goes.

### Envoys

8. **An envoy is a way of dispatching, not a new role (D3).** "I like b." When an officer needs
   Balazs for real work, it opens the ticket that work gets (a Question, Grilling, Walkthrough,
   Wayfinding or Enablement ticket) and dispatches a fresh HITL agent to hold it. The envoy loads the
   skill for the job plus a HITL brief, works it through with him in its own thread, records the
   result on the ticket and returns it to the dispatcher, which keeps working meanwhile. He can
   still talk to the officer directly, or ask it to open such a session. No new role or skill: the
   officer spine gains the "send an envoy" routine. Recorded in the grilling as a default, not
   asked: the host question tool stays for a one-line question when he is already in the officer's
   thread. *Balazs to confirm.*
9. **Only the admiral sends envoys; the officer above does the homework first (D9).** "the captains
   and sub captains shouldn't bother me unless really necessary and I'm not sure about them having
   the ability to just bug me without oversight." "So bugging me with something which can be
   answered reliably in another way is a sin. My time is the scarcest resource of all. I can't scale
   like agents."
   - Every envoy in a Project is the admiral's. Captains and sub-captains pass their need up the
     chain (rule 6), including for things only he can do.
   - Before anything goes to him, the officer above checks the reliable sources: docs, codebase,
     Linear, DRs, the spec. An obvious, reversible call is decided by the officer and recorded; he is
     left out. What needs him: things only he can do (an API token, an authorisation) and deep design
     or product calls. Questions should surface during charting and wayfinding, before
     implementation.
   - If he is needed, the officer above prepares the decision before an envoy goes. This homework
     step is where a call goes to the council or the tribunal
     ([DR 066](./066-the-law-councils-and-the-tribunal.md)).
   - Only-he-can-do-it items are Enablement tickets, blocking only the work that waits on them. When
     the Enablement is a setup procedure he runs (credentials, cutover), the envoy uses the wizard
     skill; otherwise it just asks.
   - How this changes the review loop's escalation route is
     [DR 065](./065-review-placement-and-the-merge-review-cut.md) decision 7's.
10. **One active envoy per admiral, with a queue behind it (D8, D9).** "one admiral should aim to keep
    one active envoy because that's better flow than me drowning in envoys. So when the previous one
    returns to them, and I close their thread, they should only then dispatch the next one whenever
    possible." "There's one more thing why one envoy per admiral is good. It's because they can
    actually triage which one is more important and which one I should handle first."
    - The next envoy goes only after the previous one has reported to the admiral and he has closed
      its thread, whenever possible; others wait as open tickets. "Whenever possible" leaves the
      admiral discretion when he has plainly moved on without closing it.
    - Once several wait, the admiral triages at its discretion; a suggested heuristic, not a rule:
      the costliest wait first.
    - "If there's an enablement and two questions coming from the same place, that should probably
      be bundled up into one envoy for efficiency." The envoy holds one wrapper ticket with the
      bundled tickets inside it; each keeps its own result and blocking relations.
    - "(Although we shouldn't be extremely strict about tickets here)": the queue is a judgement
      aid, not a ceremony.
11. **The envoy's brief has seven slots, and the envoy closes the loop out loud (D8).** "I like these
    seven rules". The "send an envoy" template: (1) the ticket, plus one line on why it needs him;
    (2) what's already settled, linked, not to be reopened; (3) sources to read before the first
    question, which the envoy reads and he doesn't; (4) the stop line: what the envoy may write and
    must not touch; (5) where and how to record: on the ticket, in his words, each decision with its
    rule, roles and move; (6) return: to whom, with the closing summary (decisions, open items, what
    touches other work); (7) the opening rule: when he arrives, open fresh with a two-to-three-line
    picture, then the first question with its recommendation, never assuming he saw anything above.
    The envoy "should tell that you can close this thread when the interaction is complete and
    they've already messaged back. But of course it should be done after a shared understanding."
    Changes during a session reach the envoy as dispatcher updates; it never asks him to repeat them.
    The template is one of D13's dispatch templates
    ([DR 064](./064-claims-build-the-skills.md)).
12. **dydo builds only what the host doesn't give (D4).** "This makes sense." Host features are the
    layer underneath (threads, dispatch, schedules, the inbox, PR watches); today's host is T3. dydo
    builds the tickets as the map, the cut rule, the review ladder and the rules. Where dydo is about
    to build something a host already does well (a PR watch on the thread, a "waiting on you" view),
    it copies the host's shape. Claude Code Projects is learned from, not adopted, and re-checked if
    it gains other vendors, Linear or nesting.

### Which merges need him

13. **The agents prove the code; Balazs sets the course (D16).** "That I'm not the one who has to do
    the proving, but the agents can do it reliably. And I'm the one who sets the course for the big
    picture, does the product decisions and makes sure everything keeps working and in check,
    reviewing the code sometimes, the product etc." Walkthroughs are his product review and his look
    at the code: "the walkthroughs are the place where I touch the codebase, look around, examine the
    evidence, look at the app and so." Not every PR is reviewed by him, nor its content. "Blind"
    means without his click, never unverified: "If you literally thought that I want to merge PRs
    blind then you have gotten something very wrong."
14. **A two-way PR into master merges AFK, and its captain merges it (D16).** His Q18 answer: (a),
    AFK. "over 99% of pull requests are two-way doors". A two-way PR merges when its evidence is complete:
    an independent PASS on the head, or on an ancestor when every later commit is a clean sync (B9,
    C4); gates green on the combined tree with current master (B8b, B10); CI green; the PR body
    declaring two-way. This covers PRs into master: an atomic ticket's PR and a Project's landing PR,
    whose acceptance at his walkthrough comes first
    ([DR 065](./065-review-placement-and-the-merge-review-cut.md) decision 19). A Project's own
    tickets still merge into its feature branch first, which is the admiral's: "they should be free
    to merge to that branch as they wish." There is no objection window (option (b), not chosen).
15. **Only one-way PRs need his click (D16).** "Only irreversible PRs should need my click." One-way
    means reverting the commit does not undo the effect. The listed kinds, which he accepted with
    option (a), always need his click: data migrations; publishing a package or a public API;
    anything that sends or publishes outside the repository; secrets; history rewrites; changes to
    what guards main (CI, branch protection, release workflows, the guard). Paths flag them where
    possible; a PR the paths don't flag is declared by its captain. Revert applies to two-way changes
    only (B8, C6): a one-way PR carries a recovery plan in its body. D proposed that the council judge
    doubtful cases; [DR 066](./066-the-law-councils-and-the-tribunal.md) does not take that use up.
16. **Release tags are his, per Project (D16).** "The big decision whether a tag is pushed so that a
    new official version is released should always rest with me. But main isn't that much more
    special." In many projects his own repository permissions enforce it.
17. **Main-bound merges are paced (D16).** "there shouldn't be too many merges landing on main at once
    so they don't collide." Part of option (a) as he accepted it: main-bound PRs re-check against
    current master just before landing; the admiral lands its Project's main-bound PRs one at a time;
    a PR forced to re-sync a third time is held, and the admiral sequences the colliding work.
    Recorded in the grilling as a default, not asked: across Projects, `dydo merge` re-checking the
    combined tree with master at landing time is the serialiser; if master moved, it re-checks first.
    *Balazs to confirm.*
18. **Every weed becomes a rule (D16).** A mistake found after landing, at a walkthrough or by a
    revert, is fixed with a lint, test or rule too, so it can't land again.
19. **PR watching and the quiet tick (D14).** "I think these are all good ideas". Before his click,
    the captain watches its own CI in-turn and opens the HITL Merge only when green. Nobody watches
    for the click: the survey sees "PR merged, HITL Merge open" and wakes the captain, which closes
    T4. A tick that finds nothing ends in one line and wakes no one. Since rule 14, the captain's
    in-turn CI watch and the survey's merge detection apply to whatever still waits on him.

## Consequences

- His surface is HITL threads and the chief of staff's short list; admiral threads stop being his
  inbox, and the admiral's context goes to charting and routing.
- Grilling C's C5 narrows (D16): an atomic ticket's PR into main is an AFK Merge when two-way, and a
  HITL Merge is for one-way PRs only. B2's landing click goes the same way. DR 065 decision 19 holds
  the landing.
- B3a stands, and the captain also merges two-way PRs into master. Ch4's `dydo merge` work for
  master is reshaped, and how an agent merges into the default branch feeds
  [DYD-944](https://linear.app/bodnar-balazs/issue/DYD-944). Until it delivers that route,
  [DR 061](./061-agents-merge-only-into-feature-branches.md) §2's default-branch refusal and §7's
  guard blocks stand.
- Prose that must change (DYD-1114): "Lanes have one level" in the working-tree contract and
  control flow, and the captain's Crew line.
- The officer spine gains the "send an envoy" routine with its seven-slot template, the homework
  step and the queue behind one active envoy.
- **Follow-ups,** untouched here: the glossary's Map holder ("the user, or an admiral the user
  invoked" contradicts rule 2), and entries for Envoy, Sub-captain and continuous Project; and
  `AGENTS.md`'s "an admiral only when invoked".
- The two continuous Projects need creating in the Dydo team, and the standing rule for admirals
  pulling their tickets is his to set.
- Still open from D: a sub-captain's brief and return line (DYD-1114 settled its depth and budget
  split only); which skill or built resource an envoy loads per job, to be named in the envoy
  template; and, to verify at build, that an officer can see he closed a T3 thread.

## Affects

- Supersedes [DR 051](./051-captains-by-default-admiral-on-invocation.md) §3-4, "the admiral is
  optional" and the human as default map holder: a moving Project has an admiral that holds its
  outcome (D2, L1 R5), hard calls are not the admiral's (D15), and continuous Projects have none
  (D6). What 051 rests on §3-4 falls with them: its Consequences line "A Project moves without an
  admiral", and its amendments to DR 047 §1-§3 and DR 050 decision 1 where they make the human the
  map holder or the admiral invoked-only.
- Amends [DR 065](./065-review-placement-and-the-merge-review-cut.md): its D-D11 notes on decisions
  2 and 15 give way to rule 7; decision 19 keeps only the landing, and which other merges need him
  is rules 13 to 19 here (DYD-1177).
- Amends [DR 061](./061-agents-merge-only-into-feature-branches.md) §1: a merge into the default
  branch is the user's click only for one-way PRs and release tags (rules 14 to 16).
- Supersedes [DR 045](./045-flow-map-hats-review-tiers-and-working-tree-contract.md) §7's gate on
  the feature → main merge: a two-way landing merges AFK after his walkthrough acceptance; a one-way
  landing still needs his click.

## Related

- [DR 064 - Claims Build the Skills](./064-claims-build-the-skills.md) - Dispatch templates, built skills, every rule written once
- [DR 065 - Review Placement and the Merge-Review Cut](./065-review-placement-and-the-merge-review-cut.md) - Captain autonomy and the landing
- [DR 066 - The Law, Councils and the Tribunal](./066-the-law-councils-and-the-tribunal.md) - The council that takes hard calls
- [Attention Principles](../../law/dydo/attention.md) - `dydo-attention-human-decisions`
