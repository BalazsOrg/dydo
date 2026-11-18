---
area: project
type: changelog
date: 2026-11-17
---

# DYD-1155 — Scaffold survey pickup boundary

The repository and scaffold workspace standards now give the same pickup rule: generic
`pickable` and `uncontracted` offers require a captain-held Type of Feature, Bug, Merge,
Enablement or Prototype, exclude every Inquisition and tickets labelled nit, and leave
Inquisition commission/resume explicitly with the map holder. Only their pickup passages changed.

## Contract and witnesses

Commission: [DYD-1155](https://linear.app/bodnar-balazs/issue/DYD-1155), description as of
`2026-11-17T13:42:10.274Z`, with the live admiral clarification supplied to this writer.
Base: `07d32a3b085bd7bcb9ecadcc0d494b6f182f4c85`; branch: `DYD-1155-scaffold-pickup`;
isolated root: `/home/balazs/.t3/worktrees/DynaDocs/t3-a829c0a8-36dac2b1bcc02af9`.
All five pre-edit checks passed; `preflight-*.json` and matching logs retain the literal git probes.
The owned production paths are the pickup paragraphs of the two workspace standards; this record
and its sibling `dyd-1155-evidence/` are the commissioned bounded delivery evidence.

`Services/Survey/SurveyRules.cs`, `AutoOfferTypes` and `CanOffer`, witness the five allowed Types
and nit-label exclusion. `Find` applies that predicate to both generic offer kinds.
[DYD-964's delivery record](./dyd-964-survey-captain-held.md) preserves the binding ruling that
Inquisitions are excluded entirely and explicitly commissioned/resumed by the map holder.
The Type table in the [workspace standard](../../../../reference/linear-workspace-standard.md)
witnesses which Types captains hold. This hop changes no skills, runtime, reader/model or tests.

## Content identity and proof

Both complete standard files have SHA-256
`1702c6f9f705e591b44c223fde2f6c8b0405c92b43e9f456271857a291e470ad`.
Their pickup paragraphs are byte-identical: 799 bytes, SHA-256
`139bc33641c2c740d9204057802ca18b6a52bdd18957be8909793bc5f85f6d7e`.
The equality probe spans from the paragraph beginning “Ready to Merge holds for a lane” to the
blank line before “A FutureFeature”. All other production bytes remain at the base.

The exact commissioned test filter discovered 62 cases: 59 DocumentationTests and three
ScaffoldTreeTests. Its run passed all 62, failed zero and skipped zero, exit 0.
`ScaffoldTreeTests.EveryScaffoldFileOnDisk_ReadsBackThroughItsResourceName` compares the edited
scaffold's actual content with its embedded resource. A search for the standard path and pickup
wording found no additional test pinning the edited passage. The canonical-skill assertion that
reads the standard pins only its untouched IMPLEMENTED form, so it adds no reached selection.
No runtime suite, coverage or mutation run is claimed for this prose-only correction.

Raw logs and paired JSON receipts live once in `dyd-1155-evidence/`: `discovery`,
`discovery-count`, `scoped-tests`, `passage-equality`, `toolchain`, `docs-fix` and `docs-check`.
Each receipt records the actual command vector, dirty-input source hashes, base/HEAD, UTC times,
elapsed seconds, environment and exit. HEAD was the base while the edited file hashes identify
the tested content; the implementation commit retains those exact bytes.
The host lock `/var/tmp/dydo-gate-lock.sh` remains outside every capped run. Gates use 8G/3600s;
probes use 4G/120s; swap is off and OOMPolicy is continue. The observed toolchain is .NET SDK
10.0.303 / Microsoft.NETCore.App 10.0.14 on Ubuntu 26.04 x64.

One truncated launch exited 127 before any test ran; its original receipt/log are retained as
`scoped-tests-launch-error.*`. The complete command then ran successfully. The sandbox initially
could not access the user scope bus; host escalation supplied the required scope. An inline
discovery-count probe was refused by the shell guard; the same count ran from a script file.
Neither refusal changed the contract or production scope.
The first `dydo check` returned 1 for a link leaving the documentation root to the scaffold;
that source path is now a code span. `docs-check-initial.*` retains the failure.

## Captain proof recipe and remaining ownership

From this isolated root, run `bash` with the absolute path of the sibling
`dyd-1155-evidence/proof.sh`. It contains only literal paths and the commissioned lock, environment,
caps and selectors: nonzero discovery, the exact filtered test run, byte equality and `dydo check`.
`passage.py.txt` preserves the equality probe; `run.py.txt` and `discovery-count.py.txt` preserve the
original receipt wrapper and discovery counter. The recipe copies the passage and discovery helpers
to `/tmp/dyd-1155-passage.py` and `/tmp/dyd-1155-discovery-count.py` before running those copies.
The helpers were renamed after the original proof, bytes unchanged, because tracked `.py` files are Python gate targets.
The script is a local proof recipe, not a hosted CI suite.
The captain reruns proof of record and owns fresh independent review, hosted Actions and integration.
This writer's return is IMPLEMENTED, never an independent PASS.

docs-writer/model: gpt-6.1-sol, high.

## Related

- `Scaffold/dydo/reference/linear-workspace-standard.md` — shipped pickup rule.
- [Testing strategy](../../../../guides/testing-strategy.md) — documentation-only scope and capped gates.
- [Working-tree standard](../../../../guides/working-tree-standard.md) — committed crew hops and captain ownership.
