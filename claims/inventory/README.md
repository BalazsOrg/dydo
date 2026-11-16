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
the text. The rendered prose entry is marked `combined`.

`principle` stays `null` until DYD-1078 sets principle slugs.

## Checking

`dydo/agents/workspace/DYD-1080/check.mjs`, not committed (a tracked `.mjs` would enter the CI
coverage gates); a copy is attached to DYD-1080. Run it under the memory cap.

- `node check.mjs check --phase 1 --lane L1`: units match, every unit mapped, statements valid.
- `node check.mjs check`: phase 1 for all lanes, then `kinds/` and `claims.json` matching `render`.
- `node check.mjs scaffold L1 [--force]`, `render`, `units`, `--help`.

Exit 0 only with zero errors.

## Findings summary

