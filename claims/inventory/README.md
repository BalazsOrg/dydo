# Claim inventory

Raw material for the skill builder (DYD-1081): every sentence of 35 skill and doc files mapped to the
rule it states, the rules grouped into claims, drifted copies side by side. It is step 1 of Grilling A
D13, under D14: a faithful-enough source, not a faithful translation, and no permanent faithfulness
test. Nothing here is a skill; edit no skill or doc prose while inventorying.

## Scope at base `dee4861a`

Base `dee4861ae2691e41d9e60985e3e0ffb0c7ea987d`. Only `.md` files, no `agents/*.yaml`, read with
`git show <base>:<path>` so later edits never move the inventory.

| Lane | Files (under `skills/roles/` unless noted) |
|---|---|
| L1 | `officers/ticket-captain/**` |
| L2 | `officers/admiral/**`, `officers/chief-of-staff/**` |
| L3 | `crew/code-writer/**`, `crew/docs-writer/**`, `crew/research/**`, `crew/scout/**`, `crew/reviewer/SKILL.md` |
| L4 | `crew/reviewer/resources/**`, `crew/inquisitor/**` |
| L5 | `skills/productivity/{wayfinder,what-would-balazs-do,overnight-mode,walkthrough}/**` |
| L6 | `dydo/guides/working-tree-standard.md` |
| L7 | `dydo/reference/linear-workspace-standard.md` |

## Units

The checker cuts each file into units; you never cut, edit or reorder them.

- Prose (paragraph or list item, continuation lines joined with single spaces) splits into sentences.
- One unit each: a fenced code block (fences included), a table body row.
- Skipped: front matter except the `description:` value, headings, blank lines, HTML comments, rules,
  table header and separator rows.
- Id `<path>#L<line>.<n>`: the line where the unit starts, `n` its index among units starting there.

`check.mjs units <lane|path>` lists them. Imperfect sentence splits stand: the checker is the authority.

## Three layers

1. `sentences/<lane>.json`: the sentence map, one file per lane, scaffolded by `check.mjs scaffold <lane>`.
   ```json
   { "lane": "L1", "base": "<sha>", "files": ["..."],
     "units": [ { "unit": "path#L12.1", "text": "...", "rules": [], "notRule": null } ] }
   ```
   Each unit gets exactly one of: `rules` (non-empty) or `notRule` `{ "category", "note" }`.
   A `rules` entry defines a rule statement `{ "id": "L1-001", "kind", "rule", "actors": [...] }` or
   continues one defined earlier in the lane `{ "id": "L1-001" }` (a rule spanning several sentences).
   Ids are `<lane>-NNN`, defined once. State `rule` in one plain sentence an agent could obey.
2. `kinds/<group>.json`: claims and findings, written after the sentence maps.
   ```json
   { "group": "K1", "kinds": ["dispatch", "capacity"],
     "claims": [ { "id": "dispatch.no-top-level-threads", "kind": "dispatch", "rule": "...",
       "actors": ["..."], "statements": ["L1-014", "L2-031"],
       "drift": [ { "type": "wording|value|action|scope", "note": "..." } ], "principle": null } ],
     "findings": [ { "type": "contradiction|homeless|drift|gap", "claims": ["..."], "note": "..." } ] }
   ```
   A group owns its kinds; its claims hold exactly the statements of those kinds, each statement in one
   claim. A claim may be re-kinded; its id is `<its kind>.<slug>`. Copies of one rule that differ go in
   one claim with a `drift` entry. Contradictions, rules with no home and gaps go in `findings`.
3. `claims.json`: `check.mjs render > claims.json`, never hand-edited. Each claim carries its prose per
   carrier (the skill or doc it came from), with source `path:line`, unit, statement and a `combined`
   flag. DYD-1081 reads this file.

## The rule test

A rule is something an agent could obey or break. When unsure, it is a rule.

Not a rule, with `notRule.category`: `rationale` (why), `example`, `description` (what a thing is or
does, nothing to obey), `pointer` (where to read), `framing` (orientation, a slogan), `other`.
Add a one-line `note`.

Kinds: identity, dispatch, capacity, worktree, linear, contract, review, merge, evidence, escalation,
release, return, wayfinding, inquisition, overnight, walkthrough, decision, research, docs, project, other.

Actors: admiral, chief-of-staff, ticket-captain, code-writer, docs-writer, reviewer, inquisitor,
research, scout, map-holder, officer, crew, any-agent, user.

Combined lines: one sentence stating several rules carries several rule ids in its `rules`. Never split
the text. The rendered prose entry is marked `combined`. Each rule id sits in the claim that holds that obligation's
other copies: a claim groups copies of one rule, not a topic, and obligations an agent could obey or break
independently are separate statements and separate claims.

`principle` stays `null` until DYD-1078 sets principle slugs.

## Checking

`dydo/agents/workspace/DYD-1080/check.mjs`, not committed (a tracked `.mjs` would enter the CI
coverage gates); a copy is attached to DYD-1080. Run it under the memory cap.

- `node check.mjs check --phase 1 --lane L1`: units match, every unit mapped, statements valid.
- `node check.mjs check`: phase 1 for all lanes, then `kinds/` and `claims.json` matching `render`.
- `node check.mjs scaffold L1 [--force]`, `render`, `units`, `--help`.

Exit 0 only with zero errors.

## Findings summary

Recorded, not fixed: the inventory states what the 35 files say; it advises no change to any skill.

### Counts

- Files 35; sentences (units) 1798; not-a-rule 163 (framing 87, description 46, rationale 12, example 9,
  pointer 8, other 1); rule statements 1765.
- Claims 1010: 979 after phase 2, then 51 cross-group merge sets removed 63 claims and moved 89 statements into
  another group (the survivor's kind now owns them), leaving 916; the un-bundling pass then added 99 claims and
  retired 7 (1008); the narrowing pass added 7 and retired 5. Claims with 2+ statements 402; with copies in 2+
  carriers 263.
- Narrowing pass: 27 claims kept after the un-bundling had their rule, drift or actors narrowed to the obligation
  their statements still hold (102 claims audited); 9 statements were added, the L4-199 copy joined its other copies in
  `return.following-fix-ticket-if-dependent`, and the proof-work pre-edit checks became three claims (proof branch,
  clean worktree, test paths only) with the clean-worktree copies joining `worktree.clean-before-first-edit`.
- Un-bundling pass: 96 sentences, each stating several independent obligations, carry one rule entry per obligation
  (their rendered prose is `combined`); 93 statements were narrowed to their own obligation; 138 statements were
  added; 24 claims were split into one claim per rule.
- Claims with drift 209 (a claim may carry several types): scope 133, action 58, wording 67, value 13.
- Findings 75: drift 25, gap 28, homeless 15, contradiction 7.

### R1 to R4, as found

- R1 memory cap: `capacity.run-commands-memory-capped` holds L1-030, L3-051, L3-174, L4-020 (captain, code-writer,
  reviewer, inquisitor). `capacity.crew-brief-names-memory-cap` (L1-029, the brief names the cap) stays a second claim:
  naming the cap in a brief and running under it are two obligations on two actors.
- R2 Codex concurrency cap: `capacity.codex-concurrency-cap` holds L1-026, L1-209, L2-025, L2-117, L6-068. The
  retry-on-429 and never-maximise rules of the same sentence sit in their own claims, as L1 maps them:
  `capacity.capacity-429-retry-not-blocker` (L1-027, L2-313, L6-069) and `capacity.never-maximise-parallelism`
  (L1-028, L2-314, L6-070).
- R3 fifth consecutive FAIL: `escalation.fifth-consecutive-fail` holds L1-119, L2-041.
- R4 no top-level threads for crew: `dispatch.no-top-level-threads-for-crew` holds L1-035, L2-026, L2-214, L6-058.

### Contradictions (7)

- `escalation.path-crew-captain-admiral-user`: the admiral is in the path only when invoked (L1-056) or always (L2-039).
- `linear.captain-ticket-carries-type-and-mode` / `linear.status-futurefeature-meaning`: every ticket carries one Type (L7-041), a FutureFeature has none (L7-012).
- `merge.officer-watches-captain-pr` / `merge.unwatch-before-handoff`: the captain unwatches the PR (L1-136) but the owning officer is the watcher (L1-135, L2-084, L6-100).
- `evidence.captain-rerun-is-proof-of-record` / `evidence.rerun-only-on-concern`: the captain reruns every proof (L1-128) while others rerun only on a stated concern (L1-121, L1-122, L3-172).
- `inquisition.record-delivered-through-docs-chain` / `inquisition.bug-filing-fields`: the record branch and Bugs assume a feature branch (L1-293, L1-263); an audit of main has none (L1-265, L6-119).
- `project.no-invented-work` / `project.four-lanes`: no work invented to fill capacity (L2-146) versus keep four lanes occupied (L2-166).
- `decision.below-dr-present-plain-question` / `decision.dr-grade-present-prepared-question`: a plain Question (L5-202) versus always a prepared Question (L5-222).

### Ten most consequential gap, homeless and drift findings

1. gap `escalation.fifth-consecutive-fail`: no statement says who counts the FAILs or where the count lives.
2. drift `review.plan-second-fail-to-user` / `escalation.fifth-consecutive-fail`: stop at the second FAIL for a plan, the fifth for a ticket, none stated for docs or merge.
3. homeless `release.dead-session-treated-as-release`: the owner is the map holder (L1-193), the admiral (L2-140) or any agent (L6-103).
4. gap `capacity.run-commands-memory-capped`: the probe timeout is "short" in four copies and 120 s in the reviewer copy; "short" is undefined.
5. drift `walkthrough.skeleton-ticket-blocks-next-milestone` / `overnight.skeleton-walkthrough-blocks-nothing`: L2-092 and L7-103 state the block without the overnight exception that L2-058 and L7-104 carry.
6. drift `worktree.clean-own-worktrees` / `worktree.dydo-clean-branch-deletion-limit`: "merged" means merged (L1-155) or held by origin's default branch (L6-130).
7. drift `review.should-filed-as-own-ticket` / `review.should-stays-in-project`: L1-105 files a should in the source's Project (a team Backlog ticket only outside a Project) and blocks the landing Merge Ticket when it exists; L2-064 files it in the Project without the block; L2-074 keeps it in the Project as filler unless it blocks.
8. gap `worktree.prove-checks-before-first-edit`: only the code-writer skill lists the pre-edit checks; docs-writer says "read the standard" (L3-106).
9. homeless `dispatch.native-captain-no-codex-reviewer`: stated only in the working-tree standard (L6-064).
10. gap `inquisition.start-on-user-confirmation` / `inquisition.status-path`: nowhere is it stated in one place who moves an Inquisition from Todo to In Progress (L1-174, L2-120, L7-095).

### Rules their actor never reads

Computed from `claims.json` (role actors admiral, chief-of-staff, ticket-captain, code-writer, docs-writer, reviewer,
inquisitor, research, scout; group actors excluded): of 784 claims with a role actor, 107 have no carrier among their
actors' own skills. By actor: ticket-captain 57, chief-of-staff 29, admiral 13, reviewer 7, code-writer 4, inquisitor 1
(a claim counts once per actor). 33 of the 107 are `overnight.*` rules reached only by loading `overnight-mode`. Ten others:

- `dispatch.native-captain-no-codex-reviewer` (ticket-captain): working-tree standard only.
- `identity.mutation-testing-is-reviewers` (reviewer): code-writer skill only.
- `return.pass-block-form` (reviewer): Linear standard only; `return.fail-block-form` likewise.
- `review.corrected-merge-fresh-review` (ticket-captain): working-tree standard only.
- `worktree.delete-integrated-ticket-branch` (ticket-captain): working-tree standard only.
- `contract.parallel-crew-disjoint-paths` (ticket-captain): working-tree standard only.
- `contract.no-retrospective-gate-cancel` (ticket-captain): Linear standard only.
- `linear.ready-to-merge-meaning` (ticket-captain): Linear standard only.
- `inquisition.patch-cherry-pick-red-test` (code-writer): ticket-captain's inquisition resource only.
- `decision.chief-of-staff-never-runs-the-skill` (chief-of-staff): `what-would-balazs-do` only.

