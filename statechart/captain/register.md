# The captain's rule register

This register is the closed list of rules that order a captain's work. The captain model (`CaptainBase.tla`, `Captain.tla`) and its properties (`verification/statechart/captain/`) are judged against it, and against nothing else. Written by hand at DYD-1504.

- **Closed.** After its one completeness review, a reviewer judges the model against these rows, not against an open list. A rule found later is added here first, in its part's section with the next free ID. The row cites where the rule comes from, and the date it was found. Only then do the model and its properties cite it.
- **Its one completeness review** was DYD-1504's review r1 (Codex `gpt-6.2-sol`). The rows it found missing are in, each marked "found at review r1". Rows found later say so the same way ("found at review r2").
- **Every source line is here.** Each rule of the three sources is either a row that orders the work, or a line under "Not an order" with its reason. `check-register.mjs` fails when a spec rule ID or a source key below appears in no row.
- **Precedence.** When sources conflict, his latest recorded words win, then the spec, then the decision records (C1, W11). The core page counts as his words (C0). "Conflicts settled" at the end lists each case.

## Marks and parts

A row that orders the work has one mark:

- **action**: the rule says what step the process takes, who takes it and when. The model has an action for it, and the action's comment names the row.
- **property**: the rule forbids or requires an order across steps (never, only, before, after, at most). The properties module has a property for it, named after the row; it reads the event record.

Every row belongs to one part of the model. Its ID prefix names the part:

| Prefix | Part | Its chart states |
|---|---|---|
| FR | 1, the frame (DYD-1504): the chart's arrows, and what holds across all parts | all |
| IQ | 2, intake and Questions | Dispatched, Taken, Leaf, Asking; a Question's stages and its council |
| SP | 3, the split | Cutting, Waiting |
| RM | 4, review and merge | InReview, Fixing, Merging |

**Built as** names the actions and properties that carry the row. For part 1 they exist now. For parts 2 to 4 the row says `owed`, and names the frame's placeholder action where one stands in. The part that builds the row writes its names here.

## Sources

**The spec.** `Spec: dydo 4.0.0` (Linear document `8bc07731-1edc-40f4-a1f1-41279e70db30`, updatedAt 2026-11-22T02:27:46.083Z). Its rules are cited by their own IDs: K1-K34, R1-R34, S1-S26, H1-H24, L1-L23, T1-T23, D1-D12, Y1-Y9, N1-N15. Sections 1 and 3 to 6 (goals, out of scope, acceptance checks, drift, open questions) hold no rules of their own.

**The core page**, `dydo/core.md` as of `9c67651e`, cited by keyed quote, in page order:

| Key | Line |
|---|---|
| C0 | "Every line here comes from Balazs's recorded words. Every skill file is checked against this page, together with the statechart." "This page serves the dydo 4.0.0 Project (P-DYD-51) and retires when that Project lands" |
| C1 | "The Axioms and Principles are always a good source. When records disagree, this order wins": his latest recorded words, then the spec, then the decision records |
| C2 | "A stale record is a weed. When you find one that a later record contradicts, fix or retire it in the same piece of work." |
| C3 | "A ticket is one thing, done exceptionally. Its holder owns its outcome, and it is done when its contract is met." |
| C4 | "A ticket is the right size when one agent can hold it: about 100k tokens of context. Work that needs the same thing understood belongs to one agent." |
| C5 | "Every agent dispatched onto a ticket is its captain. It either does the work itself, with native helpers, or splits the ticket and only orchestrates." |
| C6 | "A captain that cannot see the route to the spec finds it one manageable part at a time. It does not cut the whole at once." |
| C7 | "An agent decides what its ticket makes its job to decide: a reviewer its verdict, a Wayfinding captain the cut, a Question captain its answer." |
| C8 | "Outside its job, a captain decides only obvious, reversible calls. Any other call becomes a Question ticket for a fresh agent." |
| C9 | "The coordinator decides mechanically only: it sequences, restarts dead lanes, paces merges and opens the tickets the plan names. It makes no judgement calls." |
| C10 | "An agent verifies its own work wherever it can: it runs it, drives it, shows the receipts. It never judges its own work: the verdict is another agent's." |
| C11 | "Balazs decides: anything one-way (a merge that a revert cannot undo), release tags, the model roster, and every call that is his to make. An agent brings him a question with every option, each option's trade-off, and its recommendation." |
| C12 | "A **blocker** is a problem so bad that it needs fixing now." |
| C13 | "A **should** is a problem that needs a fix, but the fix can wait. It may be better fixed with other work, or a later refactor may solve it." |
| C14 | "A **nit** is an observation that gets a ruling. It may need no fix at all." |
| C15 | "A review's verdict is FAIL when it has at least one blocker." |
| C16 | "A verdict is not reviewed again." |
| C17 | "Mechanical: easy to follow, hard to get wrong, and when it does go wrong it heals itself or does little harm." |
| C18 | "Each agent gets only the skills and context its position needs. The dispatching agent names those skills in the brief." |
| C19 | "Review where it has high impact: early enough that wrong work is not built on, late enough that the system can be observed." |
| C20 | "A problem outside your ticket goes to your officer. What matters is that it surfaces." |
| C21 | "The work never stops for usage limits." |
| C22 | "The statechart is the digital twin of the process the role skills describe. It is written by hand. Every skill file is checked against it, and it shows exactly who sees what" |
| C23 | "The chart for a captain and its ticket is in `statechart/captain/`" |

**His recorded words**, on DYD-1440 (2026-11-22) and DYD-1372 (DECIDED, 2026-11-21), by comment and item:

| Key | Where | His words, or what he settled |
|---|---|---|
| W1 | DYD-1440 `7a1eac3b` | "there is also the comprehensive version, which should indeed include all the states and all the logic. So It actually serves as a model, a digital twin, so that we can check if a behavior or the process is faulty by design. And leaving out logic like third review or something else is actually a mistake. because that is part of the process being modelled." The checkpoint after a third FAIL, the extra round after a PASS and the cheap should fixed in place are process logic. |
| W2 | DYD-1440 `e055979e` | "Okay, I like this statechart. This looks good." (`chart.mmd` as committed at `8077c474`) |
| W3 | DYD-1440 `83827d42` | "Unless it's their job to decide a thing. Because then they decide that." |
| W4 | DYD-1440 `9fd14c67` | "I don't know what these stand for. So how do you expect me to rule on things without context?" (fault class DYD-1490: a question to him names things in plain words) |
| W5 | DYD-1440 `d06e13ad` | "go ahead (deleting the generator is definitely the right call)"; "Not now". Fault class DYD-1491: agents bring him calls the truth already settles, or that are another agent's to make. |
| W6 | DYD-1440 `3daad293`, `55927438` | `dydo/core.md` is "Temporary"; its index line: "Drop it". |
| W7 | DYD-1440 `33cb7cce` | The ticket-captain skill "tastes" good; "We probably need to perfect it and change a few things". |
| W8 | DYD-1372 `051616a1` item 1 | A core he approves, then a process for the rest: "if the core is correct then I can have confidence that the rest is correct as well." |
| W9 | DYD-1372 `051616a1` item 2 | "A blocker is a problem that's so bad that it needs fixing now. A should is a problem which needs a fix, but the fix can wait [...] And a nit is an observation which will be ruled on, it may not need a fix at all." |
| W10 | DYD-1372 `051616a1` item 3 | Stale records are weeds and get pulled. |
| W11 | DYD-1372 `051616a1` item 4 | Order of truth: the spec is closest, the grilling transcripts hold his words, "if those conflict the latest ones are correct", then the DRs, the Axioms and Principles; "my word is recorded as well." |
| W12 | DYD-1372 `051616a1` item 5 | "Mechanical" means easy to follow, hard to mess up, self-healing; "ideally each agent gets the kind of skills and context that they need"; the statechart "can be used to check exactly who sees what." |
| W13 | DYD-1372 `051616a1` item 6 | "the recipe should probably be a process". |
| W14 | DYD-1372 `051616a1` item 7 | A fresh coordinator. |
| W15 | DYD-1372 `dc0952ba` item 1 | The built prose is rejected. |
| W16 | DYD-1372 `dc0952ba` item 2, and the HOLD `2cf052ae` | "Things should be simple enough that we don't need the generator". |
| W17 | DYD-1372 `dc0952ba` item 3 | "I like the statechart, that's useful. It shows the whole process together and that's the test to make sure that the pieces do fit together when measured against it." |
| W18 | DYD-1372 `dc0952ba` item 4 | "I want every dead thing removed"; a crew's skill is its handoff plus what it is given. |
| W19 | DYD-1372 `dc0952ba` item 5 | Bake in what is unique, point at a skill when shared. |
| W20 | DYD-1372 `dc0952ba` item 6 | "when dispatching another agent agents should include a bunch of skills in the dispatch". |
| W21 | DYD-1372 `dc0952ba` items 7 to 9 | A recipe is needed; the repo is restructured last; the `implement.md` draft's taste. |
| W22 | DYD-1372 `aa5b445e` items 1, 2 | "Accepted with changes"; "Okay" to the brief's other recommendations. |
| W23 | DYD-1372 `aa5b445e` items 6, 7 | The dispatch part repeats at least two anchor words of `AGENTS.md`; a leaf's emphasis is on its kind part. |
| W24 | DYD-1372 `aa5b445e` items 3, 4 | A review has a captain; a seat from another family "get[s] their own tickets. Yes, they're leafs." "They're not meant do delegate into further sub agents for sure." |
| W25 | DYD-1372 `aa5b445e` item 5 | "So keeping the escape hatch is probably right." |
| W26 | DYD-1372 `07c86aad`, flavours | Each kind resource says which spine phases its holder uses and skips. |
| W27 | DYD-1372 `07c86aad`, "Fits you" | "if the agent.md says that then the two together are okay." |
| W28 | DYD-1372 `07c86aad`, wayfinding | "This is a very important part. Finding the way towards the spec, one manageable part at a time." "these human points in the workflow have to be timed very cleverly." |
| W29 | DYD-1372 `07c86aad`, roster | "I will write it. (not now). Just use a placeholder for now." |
| W30 | DYD-1372 `07c86aad`, review captain | A review "is also possible to be owned by a captain [...] it will fill the two seats prefering a different model family to the one who wrote the code", with parallel review kinds. |
| W31 | DYD-1372 `07c86aad`, open points | More gaps exist; how a need reaches him is open there (settled later by H16). |
| W32 | DYD-1372 `07c86aad`, item 4 | "4. Lgtm": one Fix ticket lists all its blockers, a sub-ticket only for a big one. |
| W33 | DYD-1372 `777d396e` | "What happens when there is no more ticket? How does the captain know to wayfind?" "Reviewing the reviews is probably not a good idea." |

**The grilling transcripts** (Linear documents 3.7.0 grillings A to F and 4.0.0 grilling G). The spec was built from them and cites them rule by rule. They were read again at DYD-1504 for orders the spec does not carry; these are what that found:

| Key | Where | The grilling's record |
|---|---|---|
| G1 | B4a | "Ticket captain (orders the scout pass, answers or escalates its Questions, refines the spec)" |
| G2 | B6 | Whether to widen the seats for a security blocker is the officer's call. |
| G3 | B8a | Read-only helpers use the host's read-only type, or a "don't edit" brief. |
| G4 | C2 | "Nobody judges their own step" is checkable from the record. |
| G5 | C3 | "The next Review waits on every Fix ticket of the round" |
| G6 | C3 | On a re-plan the parent stays In Progress and the re-cut appears as new sub-tickets. |
| G7 | C3 | Nits are read once per Project, at the final inquisition and the landing. |
| G8 | C5 | "captain (closes it after the click, verifying the merge on main)"; the chief of staff flags a HITL Merge waiting too long. |
| G9 | D9 | "during the charting and wayfinding issues should surface before implementation" |
| G10 | D9, D12 | An obvious, reversible call: the officer decides it and records it. |
| G11 | D10 | A captain's own small change is a leaf step with no Implement hop under it. |
| G12 | D16 | "Revert applies to two-way changes only (B8, C6). A one-way PR carries a recovery plan in its body, because reverting the commit does not undo its effect." |
| G13 | D16 | Judging doubtful one-way cases is "proposed for the council". |
| G14 | E1 | The Guard hardening Project runs one lane per family, in series within a family, Urgent first. |
| G15 | E4 | A CI finding about a feature whose ticket is open goes to that ticket's officer, as a lane inside it. |
| G16 | E5 | The push/branch family's captain takes PR 328's branch as its base. |
| G17 | F10 | Weeds are fixed at the lowest level that holds them first; a principle that cites no axiom is rejected. |
| G18 | A, D8 | An edge case is saved, resumed, then resolved silently or bubbled up the chain. |

## Part 1: the frame

FR1 to FR23 are the arrows of the chart he approved (W2). Each arrow is one case; its words are the rule, and the comment above it in `chart.mmd` names what it rests on. Each arrow's condition in `view-map.mjs` says what its words claim of a step, and `check-chart.mjs` tests it on every step under the arrow.

| ID | The rule | Sources | Mark | Built as |
|---|---|---|---|---|
| FR1 | Dispatched to handed back: "it cannot take the ticket, so it goes back untouched with the reason" | W2; S7, H11 | action | `CannotTake`; its arrow condition |
| FR2 | Dispatched to taken: "the ticket is free, so it sets it In Progress" | W2; S6, K30, H23 | action | `Take`; its arrow condition |
| FR3 | Taken to leaf: "the ticket fits one agent, so it does the work itself" | W2; C4, S2 | action | `WorkItself`; its arrow condition |
| FR4 | Taken to cutting: "the ticket needs more than one agent, so it splits it" | W2; C5, H2, R34, L18, H10 | action | `Split`; its arrow condition |
| FR5 | Leaf to cutting: "the work proves too big, so it splits what is left" | W2; K30, H2 | action | `SplitTheRest`; its arrow condition |
| FR6 | Leaf to asking: "a call it may not decide becomes a Question for a fresh agent" | W2; C8, H16, S10, H23, L18 | action | `AskAQuestion`; its arrow condition |
| FR7 | Asking to leaf: "the answer is back, so the work goes on" | W2; H16 | action | `Answered`; its arrow condition |
| FR8 | Leaf to review: "the change is whole and it merges it, so a reviewer is dispatched" | W2; C10, R5, R23, R33 | action | `SendLeafToReview`; its arrow condition |
| FR9 | Leaf to done: "the contract is met and it merges nothing, so it closes the ticket" | W2; C3, R2, R6, R34, R17, H23 | action | `CloseLeaf`; its arrow condition |
| FR10 | Leaf to handed back: "the work cannot be done inside this ticket, so it goes to its officer" | W2; H2, C20 | action | `HandBackTheWork`; its arrow condition |
| FR11 | Cutting to waiting: "each part is dispatched as a sub-ticket" | W2; C18, H3, K14 | action | `Dispatch`; its arrow condition |
| FR12 | Cutting to handed back: "the work cannot be cut inside this ticket, so it goes to its officer" | W2; H2 | action | `HandBackTheCut`; its arrow condition |
| FR13 | Waiting to cutting: "a part is back and the contract is not met yet, so it cuts the next parts" | W2; C6, H10 | action | `CutTheNextParts`; its arrow condition |
| FR14 | Waiting to handed back: "a part shows the contract cannot be met inside this ticket, so it goes to its officer" | W2; H2, R9 | action | `HandBackTheWhole`; its arrow condition |
| FR15 | Waiting to review: "the parts are back and wired and it merges them, so a reviewer is dispatched" | W2; R2 | action | `SendTheWholeToReview`; its arrow condition |
| FR16 | Waiting to done: "the parts are back, the contract is met and it merges nothing, so it closes the ticket" | W2; R6, S8, L18 | action | `CloseTheWhole`; its arrow condition |
| FR17 | Review to fixing: "the verdict is FAIL, so one Fix ticket takes every blocker" | W2; C12, C15, R8, R9, R10, R11, S9, S10 | action | `Fail`; its arrow condition |
| FR18 | Review to merging: "the verdict is PASS" | W2; C15, R8, R11, S17 | action | `Pass`; its arrow condition |
| FR19 | Review to handed back: "a blocker cannot be fixed inside this ticket, so it goes to its officer" | W2; R9, R10, S10, H16 | action | `Replan`; its arrow condition |
| FR20 | Fixing to review: "the Fix is done, so a reviewer looks again" | W2; R8, S9, R19, R22 | action | `ReviewAgain`; its arrow condition |
| FR21 | Fixing to handed back: "the Fix comes back undone, so it goes to its officer" | W2; H2 | action | `HandBackTheFix`; its arrow condition |
| FR22 | Merging to done: "the merge is in and its gates are green, so it closes the ticket" | W2; C3, R20, R22, S6 | action | `GreenOnTheMergeSha`; its arrow condition |
| FR23 | Merging to fixing: "the merge is not clean and green, so a Fix ticket repairs it" | W2; R19, R21, R22 | action | `Repair`; its arrow condition |
| FR24 | A ticket is done when its contract is met. Only its captain closes it, with nothing waiting on an answer. A captain that merges has its merge in, with gates and CI green on the merge SHA. Any other has its result in: a change proved, or the parts back for one that only groups them; a verdict written; an answer's call settled; findings or a cut written. | C3, K34, R6, R20, H16 | property | `FR24_DoneWhenTheContractIsMet` |
| FR25 | After a close or a hand-back nothing more happens: crew does its one job and returns, and what goes up goes to the officer above. | H1, H12, K34 (`AGENTS.md` "Sort yourself") | property | `FR25_EndsAreFinal` |
| FR26 | Each agent decides what its ticket makes its job to decide, and never judges its own work. The verdict on a change is written by the fresh reviewer on the round's Review hop, never by its captain or the Fix's holder. A ticket whose result is a verdict writes its own. A Question's captain settles its own call. | C7, C10, W3, R33, K29, R16 | property | `FR26_EachDecidesItsJob` |
| FR27 | FAIL means at least one blocker, and PASS means none. | C15, R11, W9 | property | `FR27_FailHasABlocker` |
| FR28 | A verdict is not reviewed again: a reviewer is dispatched only onto a change, never onto a verdict, a seat's findings, an answer or a cut. | C16, R34, W33 | property | `FR28_AVerdictIsNotReviewedAgain` |
| FR29 | The event record is faithful: each fact a rule decides on changes only in a step whose events name that change, so a rule read from the record sees every such change. Each event a rule gives to one position is that position's alone. | DYD-1504 contract item 3; W1; found at review r1 | property | `FR29_TheRecordIsFaithful` |

An actor named on an event is the event's only actor: the arrows' conditions and the properties fail a step that gives the same event to a second position as well.

The frame's steps inside a chart state, which the arrows' conditions need, rest on rows of the parts that will replace them: `ProveTheWork` (IQ8), `WriteTheVerdict`, `AnswerTheCall` and `WriteTheResult` (FR24, FR26, IQ14), `MeetACall` (IQ9), `PartBack` (SP3, SP7), `CheckTheContract` (SP7, SP8, SP10), `WireTheParts` (SP16), `TheReviewerWritesTheVerdict` (RM3), `TheFixIsProved` (RM9), `RunTheGates` (RM15) and `Land` (RM20). The frame's facts that those conditions read are the ticket as found on dispatch (free, held or closed), a call met that the captain may not decide, and what the captain found when it checked its contract.

## Part 2: intake and Questions

| ID | The rule | Sources | Mark | Built as |
|---|---|---|---|---|
| IQ1 | One agent holds one ticket, and several steps given to one agent are wrapped in one ticket it holds. A ticket another agent holds, or a closed one, goes back untouched with the reason; nobody sets it In Progress. | S7, S6, K34 | action | owed; frame: `CannotTake` |
| IQ2 | Whoever starts a step sets it In Progress and attaches the holder's thread link, titled `T3 · <role> · <model>`. The ticket and its brief say what its result is (a change, a verdict, findings as one seat of a review, an answer, a cut) and whether this captain merges it; only a change is merged. | S6, S13, K30, K14, S15 | action | owed; frame: `Take` |
| IQ3 | The ticket's result is known before its holder works, and does not change under it. | K30, S6, S7 | property | owed |
| IQ4 | A ticket pulled as filler: the dispatched captain checks that it is small, two-way and needs no design call. A filler that fails goes back unassigned with a one-line reason, blocked by a relation to the open Wayfinding ticket, and it is not pulled again until a Wayfinding ticket records its release, or he assigns it. | H11, H10 | action | owed |
| IQ5 | A filler that is taken is a change: a Bug or a small Feature. | H11 | property | owed |
| IQ6 | A ticket that fits one agent, about 100k tokens of context with the same thing understood, is a leaf; a bigger one splits. Work can be too small as well as too big. | C4, S2, K34, W27 | action | owed; frame: `WorkItself`, `Split` |
| IQ7 | The escape hatch: a leaf whose work proves too big splits what is left, keeping coherence. A holder divides its own unit itself. | K30, H2, H14, W25 | action | owed; frame: `SplitTheRest` |
| IQ8 | The author proves its change with receipts before it closes or goes to review; an agent verifies its own work wherever it can. | C10, R5, S9 | action | owed; frame: `ProveTheWork` |
| IQ9 | A call outside its job that is not obvious and reversible: the captain passes the need up to its officer. The officer that receives it opens a Question, a sub-ticket under its own ticket (or a ticket under the coordinator), held by a fresh captain, and the work that waits on it is blocked by it. A need only he can meet (a token, an authorisation) is an Enablement ticket, waited on the same way; its captain loads `wizard` for a setup procedure he runs, and otherwise just asks. A DR-grade call is flagged `dr-grade` and passed up the same way. | C8, H16, H5, H17, H23, L21, K34, R5 | action | owed; frame: `MeetACall`, `AskAQuestion` |
| IQ10 | Work that does not wait on the answer goes on: the Question blocks only what waits on it. The captain waits on the answer once only that work is left. | S10, H16, H10 | action | owed |
| IQ11 | The wait on a Question has one way out, its answer; then the work goes on. It never ends from the wait. | H16 | property | owed; frame: `Answered` is the one step out of asking |
| IQ12 | Nothing closes and no round starts while the work waits on an answer. | H16, H10, S9 | property | owed |
| IQ13 | Every Question carries a stage label, set by whoever opens it: research, council or his-call. The stage sets its Mode (AFK, AFK, HITL) and picks its skill at any depth. A Question opens at stage research. | H23, L18, S3 | property | owed |
| IQ14 | Every Question starts with research. A call a DR clearly settles is not hard: the holder decides and cites the DR. When the research settles the call, the Question's captain answers it. | L18, H16, H23, C7 | action | owed; frame: `AnswerTheCall` |
| IQ15 | A call the research leaves open goes to him only when it is worth his time: a his-call child, Mode HITL, with his question made ready (every option, each trade-off, a recommendation, in plain words). Any other goes to a council child, Mode AFK, chaired by a fresh ticket captain, by day or night. | L18, L20, H16, H23, L23, C11, H24, W4 | action | owed |
| IQ16 | Only a research Question's captain opens a council or his-call child, and only when its research does not resolve the call. A call the truth already settles, or that is another agent's to make, never goes to him; rubber-stamp questions never reach him. | H23, L18, H16, W5, W3 | property | owed |
| IQ17 | A research Question's answer comes from its research or from its child: a council that decided gives the answer; a council that prepared the call sends it toward him as a his-call child. A decided answer is provisional: the waiting work acts on it, and nobody ratifies it. | L18, L19, L20, L23 | action | owed |
| IQ18 | A his-call Question is the envoy ticket itself, Mode HITL, carrying the blocking relations of what waits on it. When it reaches the head of his queue, the coordinator dispatches an envoy onto it, which loads `co-thinker` and writes its brief's seven slots from the ticket. After a shared understanding, once it has reported back, the envoy tells him the thread can be closed; the next envoy goes only once he has closed it. | H23, H16, H18, H19, H15 | action | owed |
| IQ19 | A his-call Question's call is his from the take: only his answer settles it, and the ticket closes only on it. | C11, H23 | property | owed |
| IQ20 | A council: its chair, a fresh ticket captain with the council-chair resource, puts one question to judges who read only, each sealed from the others, on a neutral case file: two judges from different families, picked from the roster. With one family out, it runs on what is left: a Judge council overnight decides on one family, and by day prepares for him when the call is worth his time and otherwise decides; a Review or Evaluate council runs on two different models of the remaining family. Then one exchange. A non-member synthesises the answer, its confidence (agreed, split or contested) and the disagreements; then the exits. | L18, L14, L15, L22, L23, H23 | action | owed |
| IQ21 | The council's exits. The judges agree on a two-way hard call: decided. They disagree, or the precedent points both ways: prepared for him with both positions. One-way or DR-grade: always prepared, except overnight, when a two-way, seriously blocking DR-grade call both judges agree on is decided as a proposed DR he ratifies. An irreversible call is never decided. | L18, L21, L20 | property | owed |
| IQ22 | A council is never one agent's work. | L18, L14 | property | owed |
| IQ23 | A Wayfinding captain refines the spec it cuts toward. Its spec gets one optional scout pass, by risk, run by a native helper; it returns Questions, never PASS or FAIL, and is never repeated. The captain answers or escalates those Questions like any need. When the spec changes, the map holder's Wayfinding captain reconciles the open contracts. | R4, K30, S14, S15, H4, G1 | action | owed |
| IQ24 | No review loop on a plan, spec or contract: a cut is never reviewed or merged, and gets no verdict. | R4 | property | owed |
| IQ25 | A leaf with nothing to merge (a verdict, findings, an answer, a cut, or a part its officer reviews with the whole) closes when its contract is met: a verdict once written, a his-call Question once he has answered. | R2, R6, R17, H23, C3 | action | owed; frame: `CloseLeaf` |
| IQ26 | A leaf whose work cannot be done inside its ticket goes back to its officer: what cannot be settled under a ticket goes to the officer above. | H2, H12, C20 | action | owed; frame: `HandBackTheWork` |
| IQ27 | An obvious, reversible call is decided where it arises and recorded on the ticket; it never becomes a Question. | H16, C8, G10; found at review r1 | action | owed |
| IQ28 | When he is already in an officer's thread, it may ask him a one-line question there with the host question tool, instead of a Question ticket. | H22, H6; found at review r1 | action | owed |
| IQ29 | A Question that comes back without its answer: the officer that opened it passes it up as a problem outside its ticket, or dispatches a fresh agent onto it. The work that waits on it goes on waiting. | C20, H16, S24; found at review r1 | action | owed |
| IQ30 | A proposed DR he declines is deleted: his decline and his words go on the ticket that raised the question, and the question returns to whoever raised it. | L21; found at review r1 | action | owed |
| IQ31 | A stale record the work finds, one a later record contradicts, is fixed or retired in the same piece of work. | C2, W10; found at review r1 | action | owed |
| IQ32 | A Shoulds batch's captain re-parents the shoulds into its ticket, keeps a "related: found in <KEY> rN" link on each, and marks a duplicate Duplicate. Shoulds are grouped by cohesion: minor ones on the same code may share one ticket, and one that needs much work gets its own. | S17; found at review r1 | action | owed |

## Part 3: the split

| ID | The rule | Sources | Mark | Built as |
|---|---|---|---|---|
| SP1 | A captain that splits only orchestrates: once it has cut, it never does the work itself again. | C5, K34, H2 | property | owed |
| SP2 | A review may be cut into seats. A seat from another family goes out with `delegate_task` and gets its own ticket, a sub-ticket of the Review ticket. A seat is a leaf and never delegates. | R34, S1, W24, W30 | property | owed |
| SP3 | A part gets its own ticket when anyone but the asker uses what it leaves behind; a native helper, a scout, a lookup, a native seat among several, a command or a file stays off the record. Each part goes out as a sub-ticket by `delegate_task`, its brief naming every skill its position needs with a slash; a native sub-agent never gets a Linear unit. A parallel lane, a part with children, and a part that writes in parallel or opens its own PR get their own worktree; serial hops share their ticket's. | S1, H3, K14, C18, W20 | action | owed; frame: `Dispatch`, `PartBack` |
| SP4 | Parts may be opened ahead in Todo, blocked in step order; when a part is back and another is ready, it goes out. An officer keeps its own level's lanes full toward a soft four, pulling filler on purpose when the mainline cannot: its own pickable side tickets first, then continuous tickets by the standing rule. A CI finding for an open ticket goes to that ticket's officer, as a lane inside it. | S3, H10, H8, H11, G15 | action | owed |
| SP5 | A route in fog: the captain cuts only the parts in sight, and when none is, one part that brings the next into sight (a Question, a prototype, a Wayfinding ticket). The points where he is needed are timed ahead. | C6, H10, W28, W33 | action | owed |
| SP6 | The next cut comes only after a part is back; the whole is never cut at once. | C6, H10 | property | owed |
| SP7 | With no part left to dispatch or pull, the captain checks its contract against the spec. Met: it finishes. Not met: it wayfinds the next part and cuts it. What is left all waits on Questions or his calls: it waits on them. | H10, W33 | action | owed; frame: `CheckTheContract`, `CutTheNextParts`, `CloseTheWhole` |
| SP8 | It finishes only with its contract met: the parts back, no route left in fog, nothing waiting on an answer. | H10 | property | owed |
| SP9 | A part's holder that cannot divide its slice coherently under its own ticket returns it, and this captain cuts it again another way. | H2 | action | owed |
| SP10 | What cannot be settled under this ticket goes to the officer above: a slice that cannot be cut, a part that shows the contract cannot be met, a change to its contract or a sibling's. | H2, H12, R9, C20 | action | owed; frame: `HandBackTheCut`, `HandBackTheWhole` |
| SP11 | A leaf or lane gets no full review, only an optional fresh evidence-check seat beside it: a Review hop run once the part is back, on the leaf evidence check's row. A small leaf may need none. Hops appear only under a step with its own review loop: a leaf lane or a batched should is one ticket. | R6, S4, R34, R12 | action | owed |
| SP12 | The evidence check's outcomes: the part stands; a cheap should is fixed in place before the parent merges; a blocker is fixed now, by default by the thread that wrote the part. | S17, S9, C12 | action | owed |
| SP13 | A part's need passed up to this captain: it opens the Question as the receiving officer, at stage research, and only that part waits on it. | H16, S10, L18 | action | owed |
| SP14 | A part that waits on a Question comes back only with its answer. | H16, S10 | property | owed |
| SP15 | Liveness: a hop In Progress and quiet for two hours, or a ticket In Progress with no open hop for two hours, is checked by the officer above, which comments `liveness <UTC time>: <outcome>`; quiet time runs from the later of the last change and the latest such comment. Alive snoozes the finding for one window; a record behind is brought up to date; stopped sets the hop back to Todo, then resumes the thread or dispatches a fresh agent, which attaches a new thread link while the old one stays as history. Nothing goes on from a stopped hop until then. | S24, K22, H12 | action | owed |
| SP16 | Each part whose result is a change merges into the parent as a Merge hop with no review under it, proved by the combined gates on its merge SHA and hosted CI on the feature push. | R18, R20, S12, S3 | action | owed; frame: `WireTheParts` |
| SP17 | A parent that merges is reviewed once, after wiring and before its Merge hop, for each part, the parts together, and harmony: one review phase, of as many rounds as RM4 needs, and no review before the wiring. | R2, R6, C19 | property | owed; frame: `SendTheWholeToReview` |
| SP18 | A ticket that only groups its parts needs no review of its own, and closes when they are back. | R6 | action | owed; frame: `CloseTheWhole` |
| SP19 | A review's captain works the matrix row its brief names, fills its seats preferring a family other than the author's, and posts the round's one verdict, naming each seat (model and provider) and its blockers. | R34, S8, R15, W30 | action | owed |
| SP20 | A review's one verdict comes only once all its seats are back. | S8, R34; found at review r1 | property | owed |
| SP22 | A sub-ticket the captain cuts carries its contract, the one ticket's expected outcome, in its description. The contract quotes and links the spec sections it derives from, and says how the ticket fits the ticket above it and the spec. | S15, K34; found at review r2 | action | owed |
| SP21 | When this captain re-cuts, after a part's hand-back or a fundamental blocker in a part, its own ticket stays In Progress, the open hops that no longer apply go Canceled with a one-line reason, and the re-cut appears as new sub-tickets. | S10, R9, G6; found at review r1 | action | owed |

## Part 4: review and merge

| ID | The rule | Sources | Mark | Built as |
|---|---|---|---|---|
| RM1 | A merge needs a PASS on its head. The captain that merges is reviewed first, after its own proof, by a fresh reviewer it dispatches onto the round's Review hop; one Review per round. The brief names the review level, its lenses, and the governing spec or that there is none; the first round looks with the lenses of its level, and the security baseline is always on. The reviewer comes from a family other than the author's when possible, single seats included. | C10, R23, R33, R5, S3, R3, R6, R15, R34 | action | owed; frame: `SendLeafToReview`, `SendTheWholeToReview` |
| RM2 | Each round starts only on the author's proof of the head it judges. A Fix, a mend or a repair changes the head, so its proof is owed again. Nothing is re-run the same way: a DYTEST result for the same state counts as proof, unless later changes put it in doubt. | R5, S9 | property | owed |
| RM3 | The round's holder writes the verdict on the Review sub-ticket (PASS or FAIL, with its lenses, seats and shoulds). On a PASS it writes the PR's one-line PASS, naming the Review ticket, at the same time. The parent gets no copy. | R17, S8, C7 | action | owed; frame: `TheReviewerWritesTheVerdict` |
| RM4 | The review loop: a fix between a round with a blocker and the next round, so an unfixed version is never reviewed again (the one more round after a PASS, RM5, has nothing to fix); all visible blockers reported in one pass; by default the phase ends on the first round with no new blocker, unless the officer running the rounds orders one more (RM5); a round whose blockers go to a Fix is always followed by a full-lens round. Rounds count up from one. | R8, S3, R10 | property | owed; frame: `ReviewAgain` |
| RM5 | After a PASS, the officer running the rounds may still order one more round. | R8, W1 | action | owed |
| RM6 | Blockers are routed by kind. Broken or against the spec: a Fix, then the next round. A security hole: a Fix, then the next round with the security lens first. Fundamental, or a new requirement: the officer above re-plans. Needs Balazs: up the chain at once, as a Question that blocks only what waits on it, which this captain opens as the receiving officer. After a security hole, the captain running the rounds decides whether to widen the next round's seats. | R9, S10, H16, C12, R12, G2 | action | owed; frame: `Fail`, `Replan` |
| RM7 | The routing holds: a FAIL never ends in its round; it goes back to the officer exactly when a blocker is fundamental or a new requirement; a need for Balazs always opens its Question; after a security hole the next round looks security first. | R9, R8 | property | owed |
| RM8 | At verdict time the blockers become one Fix rN ticket listing all of them, with a sub-ticket only for a big one, and Fix rNb for parallel writers. Who gets the Fix is the officer's call: by default the thread that wrote the change, while that thread is alive and has room in its context; a fresh agent when its context is heavy or the approach was wrong. The next round is a fresh look either way. | S9, W32, K22 | action | owed; frame: `Fail` |
| RM9 | Fix rN is Done when its blockers are Done and proof ran on the head; a blocker that waits on a Question is Done only once it is answered. The next Review waits on every Fix ticket of the round. | S9, S10, G5 | property | owed; frame: `TheFixIsProved`, `ReviewAgain` |
| RM10 | After round 3 comes a checkpoint, not a stop. The officer running the rounds decides on purpose: by default one more round, escalating only for a fundamental break, a new requirement or something that needs him. It writes one line on the Review: "checkpoint: one more round, because …" or "escalated: …". The fifth-FAIL stop is gone. | R10, S10, W1 | action | owed |
| RM11 | Only acting on a third round's FAIL writes the checkpoint line. It escalates exactly for a fundamental blocker, a new requirement or a need for him, and no fourth round follows a third FAIL without it. | R10, S10 | property | owed |
| RM12 | On a re-plan, the dead hops go Canceled with a one-line reason. | S10 | action | owed; frame: `Replan` |
| RM13 | A cheap should is fixed in place before the parent merges. The mended head is proved and gets a round of its own, since a PASS stands only on its head. No merge runs with one outstanding. | S17, R23, R24, W1 | action | owed |
| RM14 | `dydo merge` is the one entry point. It refuses unless the Review the PR line names is Done with a PASS on the head, or on an ancestor whose later commits are all clean syncs with their second parent already in the target; the merge with the tip is clean; and DYTEST holds a green receipt keyed by the combined tree, all selected tests passing. | R17, R23, R24, Y4, T23 | property | owed |
| RM15 | The captain runs `dydo merge` and the gate itself, and sends an implementer only for a CONFLICT or a real red gate on record. | R21 | action | owed; frame: `RunTheGates`, `Repair` |
| RM16 | A conflict is resolved only on the ticket branch, under a fresh review of its hunks. The repaired head is proved, then gets a round scoped to its hunks; nothing lands before a verdict covers them. `dydo merge` refuses a hand-shaped merge, one whose tree is not `git merge-tree --write-tree` of its two parents. | R19, R22 | property | owed |
| RM17 | Each merge is a Merge hop with no Review under it: an AFK Merge for a feature-branch merge or a two-way main merge, a HITL Merge only for a one-way one. Done means DYTEST holds a green receipt on the merge tree and hosted CI is green on the merge SHA. | R20, R18, S12 | action | owed; frame: `GreenOnTheMergeSha` |
| RM18 | A red gate after a merge: DYTEST reruns once. A flake means the hop is Done, counted as a DYTEST result, and a ticket only when it repeats. A real red on a two-way merge means the captain reverts at once and files a CI-findings ticket; a Fix follows on the ticket branch, then a fix-scoped round, then a re-land where `dydo merge` reverts the revert. The reverted Merge goes Canceled with `reverted: <gate> red on <merge SHA>`. | R22, Y9, S12 | action | owed |
| RM19 | A revert comes only after a landing, its red and its rerun, and only for a two-way change: a one-way PR's revert does not undo its effect, so it carries a recovery plan instead. | R22, R27, G12 | property | owed |
| RM20 | Into the feature branch, the captain merges freely once `dydo merge`'s evidence holds: the feature branch is the coordinator's. | R25, R18 | action | owed; frame: `Land` |
| RM21 | A two-way PR into master merges AFK, by its captain, when its evidence is complete: a PASS on the head or an ancestor, gates green on the combined tree with current master, CI green, and a PR body that declares it two-way. There is no objection window, and it waits on the coordinator's turn. | R26, R29 | property | owed |
| RM22 | A one-way PR (data migrations; publishing a package or public API; anything sent outside the repo; secrets; history rewrites; changes to what guards main) needs his click and carries a recovery plan. Paths flag it where possible; otherwise the captain declares it. This covers PRs into master; a merge into the feature branch follows RM20. | R27, C11 | property | owed |
| RM23 | Before his click, the captain watches its own CI and opens the HITL Merge only when it is green. The survey sees "PR merged, HITL Merge open" and wakes the captain, which verifies the merge on main before it closes the Merge. | R31, S12, G8 | action | owed |
| RM24 | Main-bound merges are paced. Each re-checks against current master just before landing; the coordinator lands them one at a time and gives the turn, and the captain merges. A PR forced to re-sync a third time is held while the coordinator sequences the colliding work. | R29, R26 | action | owed |
| RM25 | A PR into master lands only current with master. When master has moved, the re-check re-syncs it or finds the CONFLICT, and never lands it; the third forced re-sync is held, and only it. | R29 | property | owed |
| RM26 | A Fix that comes back undone, or a repair that cannot be done, goes to the officer above. | H2 | action | owed; frame: `HandBackTheFix` |
| RM27 | A real red after a one-way merge follows the PR's recovery plan, since a revert does not undo it; then a Fix, and a lint, test or rule so the mistake cannot land again. | G12, R27, R30, R22; found at review r1 | action | owed |
| RM28 | The PR's one-line PASS is written only with a PASS verdict, by the round's holder, at verdict time; writing the verdict moves no work. | R17, S8; found at review r1 | property | owed |
| RM29 | A should a round finds is registered as a child of the ticket where it was found, after the round's holder checks the open shoulds under that ticket and its ancestors for a duplicate. Its parent may close Done while it stays open. | S17, C13; found at review r1 | action | owed |
| RM30 | The PR's title carries the ticket key, and the captain attaches the PR's link to its ticket itself. | H3; found at review r1 | action | owed |
| RM32 | A nit is written as one line on its record and gets a ruling; it may need no fix. An inquisition round's nits go on that round's nit ticket. | S11, C14, W9; found at review r2 | action | owed |
| RM31 | A PR never adds its own authored-code exemption to the file-token ignore list: that goes through its own ticket, then a council, then his blessing. | T18; found at review r1 | property | owed |

## Not an order

Each source line below orders no step of a captain's run, or is carried by a row above only in part. The reason says which.

| Sources | Why it orders no step of the captain's run |
|---|---|
| K1, K2, K4, K5, K6, K7, K8, K9, K10, K11, K12, K13, K15, K16, K17, K18, K19, K20, K21, K23, K24, K25, K26, K27, K28, K31, K33 | The claims generator, its build, tests and the shape of the skills it built. The generator is deleted (W16, DYD-1493); none of these names a step a captain takes on its ticket. |
| K3 | How the twin's properties are written: by hand, from the rules, never from the model. It orders this register and the properties module, not a captain. |
| K22 | Which role skills exist and are rewritten. Its line on who takes a rejected change (the agent with the context, or a fresh one after repeated failure) is who holds a hop, which the model cannot see; RM8 and SP15 carry the steps. |
| K32 | How sharp a skill line is: a quality of the text, checked by reading. |
| R1 | The principle behind review placement; R2 and R6 carry its steps (SP11, SP17, RM1). |
| R3 | What a review judges, inside the round: its content, not a step. |
| R7 | Which tools a reviewer uses, inside the round. |
| R13, R14 | Which model holds which seat, and the usage balance: who the agents are, which the model cannot see. |
| R16 | Its splitter's own small change is superseded by his later words (see "Conflicts settled", 1); its no-self-review sentence is FR26. |
| R28 | Release tags are his, per Project: the landing's, not a captain's run. |
| R32 | The shape of commits and merges: not observable as a step. |
| S5 | Linear's statuses: setup. |
| S14, S16 | The spec and the contract: documents. The spec's refinement is IQ23. |
| S18, S19, S20, S21, S22, S23, S25, S26 | The landing's should check, `dydo map`, the board, the survey and its findings, the lane ledger: views of the record, no step. |
| H4, H5, C9 | The coordinator and the map holder: their own machines. Their steps on a captain's ticket (the turn, the envoy, opening a Wayfinding ticket) are in IQ4, IQ18 and RM24. |
| H6, H7 | Where his attention lands, and the chief of staff's list: no step of this run. |
| H9 | The release harvest: the map holder's. |
| H13 | Sub-captains are allowed with no cap, and bad nesting is a finding: a part is a captain on its own run (SP3). |
| H15 | What an envoy is: a ticket captain on a HITL ticket. The his-call steps are IQ18 and IQ19. |
| H19 | The envoy brief's seven slots: a brief's content. |
| H20 | dydo builds only what the host lacks: a build rule. |
| H21, C21 | The work never stops for usage, and no usage limit goes to him: the model has no usage limits. |
| H24 | A question to him names what each DR says: a question's content (IQ15). |
| L1, L2, L3, L4, L5, L6, L7, L8, L9, L10, L11, L12, L13 | The law: its axioms, principles, case law, slugs, layout and gardening authority. A call above a captain's job goes up as a Question (IQ9); the law's content is not a step. |
| L16, L17 | Which reviews convene a council of seats: how many seats a round has, inside the round (SP19). |
| T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T13, T14, T15, T16, T17, T19, T20, T21, T22 | What tests are for, the gate set and the file token limit: the content of the gates a hop runs. RM14 carries the gates' result. |
| T12 | No ticket or merge waits on mutation: nothing in the run waits on it, so the model has nothing to draw. |
| D1, D2, D3, D4, D5, D6, D7, D8, D9, D10, D11, D12 | Descriptive docs and the glossary: content. |
| Y1, Y2, Y3, Y5, Y6, Y7, Y8 | DYTEST's repo, readiness and CI app. Y4 is in RM14 and Y9 in RM18. |
| N1, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, N12, N13, N14, N15 | The landing and the release: the landing's own machine, not a captain's run. |
| C0, C1, W6, W11 | What the core page is, and its order of truth: how agents rank records, not a step. |
| C17, W12 | "Mechanical": a quality of the process as a whole. Its "who sees what" is `positions.md`'s, checked by reading. |
| C18 | Each agent gets only the skills its position needs: `positions.md` says what, and it is checked by reading. Its dispatch step is SP3. |
| C22, C23, W1, W2, W17 | What the twin is: the comprehensive model and the simple view, checked against each other by `check-chart.mjs`. They order the twin, not a captain; W1 also names process logic (RM5, RM10, RM13) and W2 approves the arrows (FR1-FR23). |
| W5, W7, W8, W13, W14, W15, W16, W18, W19, W21, W22, W23, W26, W29, W31 | Rulings on the skills, the generator, the recipe, the roster text and the coordinator: no step of a captain's run. |
| G3 | How native helpers are briefed: a helper holds no ticket. |
| G4 | That self-review is checkable from the record: a survey check, which FR26 makes a property here. |
| G7 | When nits are read: the landing's. |
| G9 | Issues surface during charting and wayfinding, before implementation: a goal for the Wayfinding captain's cut (SP5). When a need arises is inside a state; the model lets one arise wherever a captain works (IQ9). |
| G11 | Superseded with R16 (see "Conflicts settled", 1). |
| G13 | Proposed for the council in session F and not adopted there: L16's four uses do not include it. |
| G14, G16 | One Project's plan, not a rule of the process. |
| G17 | The law's gardening order: not a step of this run. |
| G18 | A skill's edge-case appendix: its shape, now under H16 and the "When things go wrong" section. |

## Conflicts settled

1. **A splitter's own small change.** R16 lets a captain that splits author a change too small to need its own review, and G11 records it as a leaf step with no Implement hop. His later words say a captain that splits only orchestrates: the core page (C5), and `AGENTS.md` as he accepted it (K34), with "or you just orchestrate and leave the writing to others completely" (DYD-1295 DECIDED). His later words win: SP1.
2. **Revert of a one-way change.** R22 has a real red after a merge reverted at once. G12 limits revert to two-way changes, and R27 gives a one-way PR a recovery plan. Read together: a real red on a two-way merge is reverted (RM18), and one on a one-way merge follows its recovery plan, then a Fix and a rule (RM27, with R30); RM19 holds the limit.
3. **A Question's stages.** An agent ruled at DYD-1454 that the stages draw nothing, reading the simple chart as the whole process. His later words (W1) make the twin hold all the logic, and H16, H23 and L18 give the stages: IQ13 to IQ22 stand.
4. **One more round, the checkpoint, the cheap mend.** An envoy read them out of the process (DYD-1440 `83827d42`, withdrawn the same day); W1 puts them back: RM5, RM10, RM11, RM13.
5. **Who opens a Question.** The approved chart's comment says only that a call becomes a Question. H16 has the receiving officer open the Question for a need passed up, so a captain opens one for a need only as its receiving officer (IQ9, SP13, RM6). A research Question's captain also opens its own council or his-call child (IQ15, H23).
6. **A flake.** Grilling C (its section C6) had a Bug filed for a flaky gate. R22 and Y9, later, count it as a DYTEST result and make a ticket only when it repeats: RM18.
7. **A fix between every two rounds, and one more round after a PASS.** R8 says both. Grilling B (its section B6) gives the first its reason: "Never re-review an unfixed version." Read together, the fix comes between a round with a blocker and the next round, and the one more round after a PASS has nothing to fix: RM4, RM5. Found at review r2.
