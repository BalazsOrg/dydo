---
area: project
type: reference
---

# DYD-1186 Documentation Evidence

Contract: DYD-1186 description as of `2026-11-18T00:44:06.841Z`, commissioned by its ticket-captain.
Writer: docs-writer, gpt-6.1-sol/high. Base: `b995a1927c9ad29a78f35fc9ee1d5f82ee3b358c`.
The supplied raw Linear envelopes in `linear-snapshot.txt` preserve the ticket and Balazs’s governing
DECIDED comments `bdb6b643-f33d-4bd0-b801-c33566d63bca` and
`88c79701-572c-4cba-b8e9-800088264a07`. The ticket envelope itself reports `updatedAt`
`2026-11-18T00:41:03.957Z`; the commission pins the later description timestamp above. No separate
governing feature spec was supplied for this bounded change.

## Pre-edit invariants

Observed before the first repository edit:

1. `git status --short --branch`: only `## DYD-1186-doc-rulings` (exit 0).
2. `git rev-parse --show-toplevel --git-common-dir`: isolated root
   `/home/balazs/.t3/worktrees/DynaDocs/t3-a2f64ef5-eadbcb161c93938a`; shared Git directory
   `/home/balazs/Desktop/Projects/DynaDocs/.git` (exit 0).
3. `git rev-parse HEAD`: the posted base above (exit 0);
   `git merge-base --is-ancestor b995a1927c9ad29a78f35fc9ee1d5f82ee3b358c HEAD` (exit 0).
4. The status output contained no changed or untracked paths: clean worktree.
5. Owned-path review: only non-skill documentation, its Scaffold twins, historical inbound-link
   repairs, eleven plan deletions, and this permitted documentation evidence directory. `skills/`
   and `claims/`, generated skill builds included, are excluded.

The first bounded snapshot read failed to reach the user bus inside the sandbox (exit 1). The same
command succeeded with sandbox escalation (exit 0); subsequent probes use that access. One inline
interpreter probe was rejected by the shell hook before execution; scripts thereafter were written
to `/tmp/` first. Neither refusal changed repository files.

## Witness and scope

- DYD-1100 supplies the principle’s statement and description verbatim, its citation and origin;
  withdraws the Project plan kind and F12 rename; orders deletion of the eleven finished plans.
- DYD-1114 supplies no hard depth cap, the practical ceiling, self-similar ticket-captains, cutting
  by the officer above, no self-promotion, re-cut requests upward, coherent units, and immediate use.
- Live non-skill guidance and reached Scaffold twins follow those rulings. Historical records
  retain their meaning; only broken inbound plan links are repaired with exact commit permalinks.
- General contract-word cleanup belongs to DYD-1099. Runtime behavior is not reached, so this hop
  adds no runtime tests. The documentation-only scope follows the testing strategy.

## Checks and logs

Every executable probe uses `systemd-run --user --scope
--slice=app-t3code-agents-cbc6c2286251.slice -p MemoryMax=4G -p MemorySwapMax=0
-p OOMPolicy=continue timeout 180` before its command, with the worktree above as working directory.
No full gate or .NET build/test command is run in this hop.

Final bounded check results and exact commands are recorded in `check-results.txt`.
The source/link probe and its complete output are preserved alongside that record. Source-file
digests identify the checked documentation without claiming that later evidence-file additions
were part of an earlier check.

## Gaps handed to the captain

- Statechart gate: **UNMET**, unavailable on this host; no pass claimed.
- Mutation gate: **UNMET**, unavailable on this host; no pass claimed.
- Independent review, integration and other project-declared gates remain captain-owned.
- All encountered affected skill prose is listed [for Balazs’s approval batch](./skill-prose-inventory.md);
  no excluded file is edited. No excluded inbound deleted-plan link was found.
- Git permalinks are checked against local Git objects. No network availability check is claimed.

## Related

- `deleted-plans.txt` - Exact eleven-file manifest
- `plan-link-repairs.txt` - Original link locations and targets
- `excluded-plan-links.txt` - No deleted-plan inbound sites found
- [Skill Prose Inventory](./skill-prose-inventory.md) - Unedited approval-batch handoff
