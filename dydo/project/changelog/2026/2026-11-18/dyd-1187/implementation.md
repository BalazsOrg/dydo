---
area: project
type: changelog
date: 2026-11-18
---

# DYD-1187 implementation evidence

code-writer/model: gpt-6.1-sol, high reasoning effort.

Contract: DYD-1187 updatedAt `2026-11-18T00:44:31.179Z`, read from Linear, and
DYD-1100 DECIDED comment `bdb6b643-f33d-4bd0-b801-c33566d63bca`, §3 rules 4/6.
Base `b995a1927c9ad29a78f35fc9ee1d5f82ee3b358c`, branch `DYD-1187-explains`.
The initial HEAD equalled that base, the base was an ancestor, and the isolated worktree was clean.
No extra branch, worktree, PR, subagent or Linear mutation was used for this hop.

## Behavior

`check` accepts `explains` as a scalar, inline list or block list, including quotes, comments,
CRLF-compatible frontmatter, slash/backslash separators and optional `./`. Declarations use
repository-root paths with `*`, `**`, `**/` and `?`; `*` and `?` do not cross a separator.
Empty/malformed declarations, absolute/traversing paths, unsupported glob
syntax and any unmatched entry are errors. Existing documents need not declare the field.
The existing frontmatter block boundaries and first-declaration convention are retained.

`pr-ready` prints advisory warnings naming the unchanged doc and all touched explained paths.
Its normal readiness success still returns 0 with these warnings. Updating the doc clears its
warning. The writer is told to update it or state it still holds; no acknowledgement store is added.
The comparison includes commits, staged and unstaged edits, deletions and both sides of renames.
It uses the merge base with local `origin/HEAD`, or `--base <merge-target>` for a feature target.
Missing comparison refs produce an advisory availability warning. Nothing is fetched.
`check --base <merge-target>` cheaply shares the same advisory comparison.

25 local code-descriptive pages now declare matching code paths. `declarations.json` is the concrete
inventory. This includes the implementation overview, architecture, documentation model,
scaffold/setup description and the code-descriptive customization guide, as well as references.
Oversized command, assurance and guard references were split at their existing sections so every
touched file fits 20,000 bytes. The original pages retain headings and links for existing anchors;
the preservation audit found no missing original prose lines. New syntax/option descriptions were
added in both local and shipped references.

The associated command-reference checks now read the split pages, keeping option and example
assertions active. The shared about-doc comparison permits only the added declaration line to
differ. Its other frontmatter and prose still compare. Helpers moved to a partial file to keep
the existing test file under the byte limit. No tests were disabled or removed.

## Proof and rerun recipe

From this worktree's root:

```text
python3 dydo/project/changelog/2026/2026-11-18/dyd-1187/proof.py.txt red
python3 dydo/project/changelog/2026/2026-11-18/dyd-1187/proof.py.txt green
python3 dydo/project/changelog/2026/2026-11-18/dyd-1187/proof.py.txt discover
python3 dydo/project/changelog/2026/2026-11-18/dyd-1187/proof.py.txt light
```

The red action temporarily restores only the five production files to the pinned base and always
restores their candidate bytes afterward. Expected red exit is 1; green/discover/Light expect 0.
Run green after red to refresh build outputs. The script refuses pre-existing edits in those files.
It preserves the actual output under `/tmp/dyd-1187-rerun-<action>.log`.
`commands.json`, the native logs and `raw-manifest.json` retain exact commands, discovery/selection,
exits, source identities and raw result paths. `.gz` artifacts are native outputs compressed only;
split gzip parts concatenate in numbered order before decompression.

Observed decisive results:

- Exact-base command-boundary red: exit 1, 19 failed, 0 passed. Example failures: malformed and
  unmatched declarations returned 0; unchanged docs emitted no warning. The strengthened cases
  positively assert ordinary success before removing their explained files or updating their doc.
- Additional separator-trap red: exit 1, one failed/one passed. `Services?Other.cs` wrongly matched
  `Services/Other.cs` through the shared matcher. The scoped matcher corrects it without editing Utils.
- Final surgical green: exit 0, 20 passed, 0 skipped. All new tests carry the DR067 traced-contract
  job tag citing DYD-1100 DECIDED rules 4/6 and DYD-1187.
- Final reached-feature Light plus report-only AltCover: exit 0, 733 passed, 0 skipped.
  The selection includes check, PR-ready and guard callers, scanning/frontmatter, scaffold/init,
  reference consistency, canonical-skill shared checks, help/smoke and optional-summary behavior.
- Production and test analyzer builds: both exit 0, zero build warnings/errors. Changed-file
  cognitive complexity and parameter checks: exit 0, no findings. Real compilation namespace graphs
  have no cycles. Locked jscpd scan: exit 0, eight changed C# files, zero clones at 15 lines/100 tokens.
- Native scoped `dydo check`: 25 pages, each exit 0. Final declaration/size/path audit,
  original-prose preservation audit and staged whitespace check: each exit 0.

Every test/gate command used the host's `/tmp/dydo-full-gate.lock` for final evidence and an agent
systemd scope with swap off and `OOMPolicy=continue`: tests/statics/report 8G/3600s; final focused
probes 4G/240s. The slice was `app-t3code-agents-cbc6c2286251.slice` from `/proc/self/cgroup`.
.NET used `MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false`.
The sandbox initially denied the systemd user bus; authorized escalated execution supplied the caps.
Pinned dependencies restored from the host cache; no new dependency was added.

### Failed attempts and proof-ordering caveat

The first temporary-repository fixture had a conflicting `feature` branch name, then lacked the
index needed for docs discovery. Those attempts are not valid behavioral red evidence. The
decisive corrected red was run with production files restored to the exact base after an initial
implementation draft had already been written. Strict red-before-first-production-edit ordering
is therefore not established; the captain must account for this deviation. The retained exact-base
red and final green do establish that the graduating assertions detect the missing implementation.

A later report run had 732/733 passing: the scaffold resource embedded in the prior DLL had one
extra trailing newline compared with the final source file. The native failure identifies the
exact strings. Rebuilding before instrumentation resolved it; the final run passed 733/733.
One initial static whitespace check exited 2 for extra EOF blanks introduced by the split; the
final whitespace check passes. A mistaken jscpd entry-point probe exited 1 before the documented
`run-jscpd.js` entry point was used successfully. The stale-resource and whitespace failures are
retained separately; the fixture setup attempts are described here.

Coverage is a report only: `coverage-report.json` and selected native OpenCover XML are provided;
neither coverage floors nor HCRAP were gated. Mutation remains unavailable/unmet. Statechart,
native Windows, hosted CI and independent review were not run here and remain unmet/captain-owned.

## Grandfathering boundaries

Prescriptive standards, work-model/lifecycle/control-flow instructions, glossaries, law, temporary
specs, navigation, evidence and unfilled project templates do not describe local implementation
paths and were not assigned artificial globs. The corpus inspection covered reference, understand,
guides, the root docs and Scaffold/dydo.

Shipped upstream product descriptions under `Scaffold/dydo/reference/` do not receive declarations
to `Commands/**` or `Services/**`: an adopting repository does not contain dydo's implementation,
so those declarations would fail its next check. They retain the new syntax documentation and split
command reference; fresh-init tests prove they remain valid. Associating installed upstream
references with an external source tree needs a separate contract if desired.

## Concrete drift and unlinked-rationale findings for STATE

No prose repair below was made in this hop.

- `dydo/reference/coverage-tools.md:13` still states the 80%/60%/HCRAP policy, contrary to DR068.
  The split `coverage-tools-running.md`, `coverage-tools-results.md`, `coverage-tools-statics.md`
  and `coverage-tools-measurement.md` retain threshold-era whole-stack/adapter instructions.
  `dydo/guides/testing-strategy.md`, `dydo/guides/coding-standards.md`, `dydo/glossary.md`, and shipped
  coding/testing guides also retain superseded coverage/HCRAP and per-module-test wording.
- `dydo/understand/architecture.md:65` and `documentation-model.md:25` still give repository plans
  and temporary specs standing homes. `understand/about.md:11` still places temporary specs in Git.
  DYD-1100 DECIDED §1 scraps architecture plans and moves Project specs to Linear.
- `dydo/reference/dydo-glossary.md:15`, `reference/feature-spec-convention.md:9`,
  `understand/task-lifecycle.md:39` and `understand/work-model.md:22` retain the reviewed architecture
  plan concept. Their shipped glossary/spec-convention counterparts carry the same stale policy.
  `understand/control-flow.md` describes the older 3.x flow and calls itself a shared truth;
  reconcile it against the new Decisions rather than adding code authority to it.
- Unlinked rationale: `dydo/understand/architecture.md:98`, “No DI framework — direct construction
  keeps the Native AOT CLI small.” No DR/principle link accompanies that reason.
- Unlinked rationale: `dydo/understand/scaffold-and-customization.md:31` and
  `dydo/guides/customizing-roles.md:50` explain why `../` climbs are forbidden through two different
  symlink resolutions. Their mechanism is described, but no DR/principle link authorizes the rule.
- Unlinked rationale: `dydo/guides/customizing-roles.md:29` says explicit roles cost no context and
  that this is why the glossary carries the taxonomy. It has no linked governing reason.

These are fix-or-cut candidates, not machine-detected drift verdicts or authority to change doctrine.

## Follow-up wording/rubric sites

- `skills/roles/crew/reviewer/resources/docs.md`, “Human-facing dydo documents”: add rule 1's
  code-wins/doc-bug distinction and an explicit finding for an unlinked why.
- `skills/roles/crew/reviewer/resources/code.md`, “Standards” or “Contract”: when explained paths
  change, check the declaring doc against the new code and read the writer's still-holds statement.
- `skills/roles/crew/docs-writer/SKILL.md`, Boundary/Method: distinguish descriptive/prescriptive,
  put invite-only admission and the solid/important/expensive-to-rebuild test before new descriptions.
- `dydo/reference/writing-docs.md` and its scaffold copy: descriptive admission/invite-only wording;
  the concept template's “What → Why → How” needs the linked-DR/principle qualification.
- `dydo/understand/_understand.md` and `Scaffold/dydo/understand/_understand.md`, “When to Add Docs
  Here”: qualify invitations to create new descriptive docs.
- `skills/productivity/writing-for-agents/SKILL.md:97`, the “cache” and “reason behind a choice”
  guidance: preserve the descriptive admission and linked-reason boundary.
- `skills/roles/crew/code-writer/SKILL.md` still calls for HCRAP fixes and older test ordering;
  reconcile that operational wording with DR067/068 in its own follow-up.

`skills/` and `claims/` were read where relevant and have no edits in this commit.
