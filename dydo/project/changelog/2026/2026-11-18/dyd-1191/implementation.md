---
area: project
type: changelog
date: 2026-11-18
---

# DYD-1191 documentation split

Docs-writer hop for DYD-1191, active description updated 2026-11-18T01:56:17.734Z.
Authority: [admiral DECIDED on DYD-1187](https://linear.app/bodnar-balazs/issue/DYD-1187#comment-ddba537f-6fc2-4e7b-b2c9-e83c03b0e52a),
night orders 2263ef45 and [DR 069](../../../../decisions/069-file-token-limit.md).
Base and merge target: `feature/dydo-4-0-0`, pinned base
`56b9fd8dee8c7af7d4174572454e0f9670a9d27f`. Branch:
`DYD-1191-split-reference-pages`. The five required pre-edit checks passed before the first
repository write: `pre-edit.log`. No children, test edits, skills/claims edits, new
declarations, new option descriptions, push, PR or merge in this hop. Review is captain-owned.

## Split design

All three original filenames remain navigation hubs. Their original Markdown headings, including
the title and Related heading, preserve 111 heading anchors across the four existing source files.
Coverage's previously unheaded introduction moves to a linked platform-measurement page. Its
content is not summarized or dropped. The guard's long Bash section ends at the denylist
subheading, which has its own page; housekeeping and integration join the hook page, and
archive-before-delete joins additional enforcement.

The baseline has only the command-reference Scaffold mirror. There is no
`Scaffold/dydo/reference/coverage-tools.md` or `Scaffold/dydo/understand/guard-system.md`, and no
such product content was invented. The command mirror is split from its own pinned blob, never
copied from local documentation. Thus the original intentional differences survive exactly.

| New page suffix | Original material | Local bytes | Scaffold bytes |
|---|---|---:|---:|
| coverage-tools-platforms | Original introduction, Python/.NET platforms | 5,842 | absent |
| coverage-tools-provisioning | Provisioning, distribution requirements | 7,559 | absent |
| coverage-tools-running | Operations, memory cap, declared rows | 15,107 | absent |
| coverage-tools-results | Row states, adapter summary, tools/commands/collectors | 6,639 | absent |
| coverage-tools-statics | Static collectors, statechart platform measurement | 9,906 | absent |
| coverage-tools-coverage | Coverage collectors | 10,193 | absent |
| coverage-tools-measurement | C# campaign, schemas, CC, PDB, provenance | 10,766 | absent |
| dydo-commands-core | Setup, documentation, guard | 10,030 | 10,022 |
| dydo-commands-linear | Linear and mode | 10,064 | 9,990 |
| dydo-commands-worktrees | Archive and clean | 15,634 | 15,722 |
| dydo-commands-delivery | PR readiness and merge | 9,954 | 9,087 |
| dydo-commands-utilities | Validation, testing, utilities, exit codes | 5,452 | 5,452 |
| guard-system-hook | Hook wire contract, housekeeping, integration | 5,448 | absent |
| guard-system-paths | Path tiers | 5,515 | absent |
| guard-system-bash | Bash command analysis before denylist | 16,387 | absent |
| guard-system-denylist | David Ondrej denylist audit | 3,834 | absent |
| guard-system-nudges | Nudges | 10,005 | absent |
| guard-system-enforcement | Also Enforced, archive-before-delete | 9,392 | absent |

Every resulting page is strictly below 20,000 bytes. The largest is 16,387. Check has 9,970 bytes
of headroom in its local core page; pr-ready/merge has 10,046 in its delivery page. PR359 owns the
later option wording and declarations. The local hubs are 2,148, 4,519 and 1,702 bytes, respectively;
the Scaffold command hub is 4,519 bytes. The three authored folder navigation pages list the new
pages in Contents.

## Mapping and byte proof

`audit.py` reads all four originals directly from the pinned Git commit. It verifies a
complete, nonoverlapping partition of each original, with no omitted tail. Each moved range is
recorded by original byte offsets, SHA-256 and destination:

- Coverage: `mapping-dydo-reference-coverage-tools.json`
- Local commands: `mapping-dydo-reference-dydo-commands.json`
- Scaffold commands: `mapping-Scaffold-dydo-reference-dydo-commands.json`
- Guard: `mapping-dydo-understand-guard-system.json`

The script reconstructs every resulting byte from those originals, allowing only its explicit
frontmatter/heading/navigation wrapper and Markdown link-target transformation. Comparison is exact
UTF-8 byte equality, not a word bag, whitespace normalization or line-union test. It checks the
entire expected files, so unexpected extra prose also fails. Original code fences and code text
remain byte-identical. The source partitions include the original prefixes and Related content;
only moved bodies appear in the range manifests.

The four `anchors-*.json` artifacts map every old heading anchor to its new canonical owner.
The audit excludes fenced-code comments when identifying headings, following the repository's
anchor rules for these literal headings. It verifies all 111 old anchors still exist on the hubs,
every canonical-owner anchor exists, and all 148 tracked Markdown fragment links reaching these
families resolve, including unchanged callers. Native `dydo check` separately validates Markdown
with the product parser. Seven inbound documents now use direct owner links:
getting-started, configuration, safe-feature-merging, DR056, DR061, the DYD-415 pitfall and the
DYD-624/618 review. Intra-family fragment links also point directly to their owners.

`testing-strategy.md` is already oversized and remains byte-identical: changing its provisioning
link would make an unrelated oversized file part of this diff. Its old coverage-hub anchor still
resolves. General pointers and public URLs keep the short hubs as their destinations.

Documentation snapshot SHA-256, including hubs, split pages and link/navigation repairs:
`bc6f2a7cdd8f7562e694fc5bdc9b6096ae311cc2ee41bca57edead72563d87b2`.
The audit computes this digest from sorted path/file-SHA pairs; evidence itself is excluded.

## Consumers and proof results

`all-consumers-01.log` and `all-consumers-02.log` explicitly
classify all 175 pinned tracked files matching the three family names, including historical raw
evidence. `consumer-map.py` regenerates that inventory. The earlier bounded
`rg` discovery is retained in `consumers-before-01.log` and `consumers-before-02.log`.
Implicit wildcard, runtime, parser and test consumers are mapped in the
[reader handoff](./reader-handoff.md).

| Proof | Result | Raw evidence |
|---|---|---|
| Exact prose/anchor/size/scope audit | Exit 0; 25 moved ranges, 111 old anchors, 148 inbound fragments | `audit-01.log` |
| Local dydo fix/check | Exit 0; no fixes, zero errors/warnings | `fix-01.log`, `check-01.log` |
| Scaffold adopter fix/check | Exit 0; no fixes, zero errors, two existing foundation warnings | `scaffold-adopter-fix-01.log`, `scaffold-adopter-check-01.log` |
| Pinned Scaffold baseline check | Same two warnings, zero errors | `scaffold-baseline-check-01.log` |
| Focused .NET discovery | Exit 0; 29 selected tests, all four requested selections nonempty | `discovery-01.log` |
| Unchanged .NET readers | Exit 1; 27 passed, 2 failed, 0 skipped | `dotnet-01.log` |
| Unchanged Python policy readers | Exit 1; 2 discovered, 2 failed | `python-reader-red-01.log` |
| Runtime dydo help | Exit 0; original command hub path remains | `runtime-help-01.log` |
| Diff whitespace static check | Exit 0 | `statics-01.log` |

The .NET failures are missing option documentation in the single-file command reader and missing
canonical-source wording in the single-file guidance reader. The Python failures are the ordinary
and inherited portable policy test reading the new coverage hub alone. These are next-hop work,
not a green ticket verdict. Option parity and required-example checks currently pass on hub stubs;
that is insufficient assertion reach and is explicitly included in the handoff.

The CLI refuses `dydo fix/check Scaffold/dydo` from the repository root as outside its docs tree
(exit 2, retained in `scaffold-fix-01.log` and `scaffold-check-01.log`). The successful replacement
copies Scaffold into an isolated temporary adopter, copies the repository config, runs the native
commands there and proves fix did not change any source-document byte. A separately reconstructed
pinned baseline proves the two customization warnings pre-exist. Temporary adopter trees and
isolated test-runner worktrees were removed by their owners.

## Exact rerun recipe and remaining gates

Run from this checkout, on Linux with a usable user systemd manager, Python 3.14.4 and .NET SDK
10.0.303/runtime 10.0.14. Locked .NET dependencies restored successfully in the isolated test snapshots.
No new tools were installed. Every subprocess receipt names its command, cap and exit. The scripts
use the session slice, swap disabled, OOMPolicy=continue, 4G/120 seconds for probes and 8G/3600 seconds
for full discovery/test runs. They set the three required .NET environment values. No host gate lock
was acquired inside a cap and no concurrent full gate was launched.

```bash
python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/proof.py audit
python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/proof.py docs
python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/proof.py scaffold
python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/proof.py scaffold-baseline
python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/proof.py help
python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/proof.py statics
python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/proof.py discovery
python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/proof.py dotnet
python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/proof.py python
```

The wrapper scripts are orchestration only: each actual probe/gate runs under the recorded cap.
The last two operations are expected red on this docs-only hop. After reader edits, use the audit's
`--docs-only` flag for unchanged documentation verification, and independently audit every code/test
change and its size. Do not use `--write` to run proof: it is retained only as the original authoring
recipe. Proof reruns replace their same-named raw receipts; retain the authored-hop receipts before
running the next hop if their historical distinction matters.

The code-writer owns equal-strength reader changes, missing-option red/green, reached-feature Light
and code/test statics. Captain owns final proof, fresh Claude Opus high review after fixes with
DR058 grading, integration and PR gates. Statechart, mutation, native Windows and hosted CI are
unmet here, never passed. No independent review is claimed by this implementation return.

## Related

- [Reader handoff](./reader-handoff.md)
- [Writing documentation](../../../../../reference/writing-docs.md)
