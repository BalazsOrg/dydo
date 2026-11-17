---
area: project
type: changelog
date: 2026-11-17
---

# DYD-964 — Survey captain-held offers

`pickable` and `uncontracted` now require a Type of Feature, Bug, Merge, Enablement or Prototype,
and exclude tickets labelled nit. Type and severity matching is case-insensitive. Assignment still
means claimed; it does not distinguish a captain's work from the map holder's work.

The commissioning captain delivered the admiral's binding ruling: exclude Inquisition entirely
from these two generic offer kinds. Explicit commission and resume remain with the map holder.
There is no reader/model expansion or skill/Decision edit. The workspace standard's pickup sentence
now names the Type boundary and the Inquisition/nit exceptions. Existing Gherkin scenarios retain
their assertions; their two offer fixtures now carry Feature labels.

## Candidate and reach

Base: `cc5e07ec5b886710e91be24cfeacba7d588fbb02`; branch: `DYD-964-survey-captain-held`;
isolated root: `/home/balazs/.t3/worktrees/DynaDocs/t3-3ae3ad83-9266c9f1446abf64`.
All five pre-edit checks passed: ticket branch, isolated root distinct from the main checkout,
base ancestry, clean tree, and writes confined to the commissioned paths.

Runtime reach is SurveyRules and its survey CLI consumers. Light includes the existing survey
scenarios, rule/command/parser tests and shared command smoke/documentation checks. The native
selection also includes three worker-lane tests whose names contain Survey. No Heavy selection
is declared for this behavior. The supported facade has no feature or changed-file coverage
selector; its whole-dotnet producer/consumer is the documented fallback, not ticket-scoped proof.

The inputs were a dirty tree at the base SHA. Per-run receipts bind the changed files to SHA-256
hashes; the commit retains those source/test/standard bytes. Raw runtime proof uses the existing
fake Linear boundary, not a live user's Linear workspace.

## RED and restoration

With final regression tests and the base version of SurveyRules, the focused command returned 1:

```text
Offers_SkipMapHolderTypesAndInquisitions(label: "Question", state: "backlog", kind: "uncontracted") [FAIL]
Offers_SkipMapHolderTypesAndInquisitions(label: "Inquisition", state: "unstarted", kind: "pickable") [FAIL]
Offers_SkipMapHolderTypesAndInquisitions(label: "Inquisition", state: "backlog", kind: "uncontracted") [FAIL]
Offers_DoNotRecommissionAReleasedTodoInquisition_OnRepeatedTicks [FAIL]
Assert.Empty() Failure: Collection was not empty
Collection: ["DYD-1"]
Failed!  - Failed: 21, Passed: 114, Skipped: 0, Total: 135
```

The original RED reproduced the Type-blind predicates before the fix. The supplied evidence already
localized the cause; no speculative instrumentation or load probe was needed. After GREEN, the
fix-reverted RED independently confirmed that the new tests reject the base implementation.
An EXIT trap restored the fix; both saved and restored SurveyRules SHA-256 were
`3e680e775e4173010ee7b93e955110e7c4207be054b19f440e6761c9ec6a378f`.
Restored GREEN returned 0: 135 passed, 0 failed, 0 skipped.

## Proof recipe

Run from the isolated root with the declared pinned toolchain provisioned. This session observed
CPython 3.14.4, Node 26.13.0, .NET SDK 10.0.303 and Microsoft.NETCore.App 10.0.14, installed the
Python and coverage-Node locks locally, restored AltCover 9.0.102 and the metrics lock. The cap's
nearest session slice was `app-t3code-agents-cbc6c2286251.slice`. Keep the host lock outside the cap.
The recipes below expand to the recorded command vectors; derive the rerunner's own nearest slice.

```bash
dyd964_slice=$(awk -F/ '{for(i=NF;i>=1;i--)if($i~/\.slice$/){print $i;exit}}' /proc/self/cgroup)
dyd964_run() {
  local dyd964_memory=$1 dyd964_deadline=$2
  shift 2
  /var/tmp/dydo-gate-lock.sh env MSBUILDDISABLENODEREUSE=1 \
    DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false \
    systemd-run --user --scope --slice="$dyd964_slice" \
    -p MemoryMax="$dyd964_memory" -p MemorySwapMax=0 -p OOMPolicy=continue \
    timeout "$dyd964_deadline" "$@"
}
dyd964_light='FullyQualifiedName~Survey|FullyQualifiedName~CommandSmokeTests|FullyQualifiedName~CommandDocConsistencyTests'
dyd964_run 4G 120 .static-gates/python/bin/python DynaDocs.Tests/coverage/gap_check.py capabilities
dyd964_run 4G 120 dotnet test DynaDocs.Tests --no-build --list-tests --filter "$dyd964_light"
dyd964_run 4G 120 dotnet test DynaDocs.Tests --no-build --list-tests
dyd964_run 8G 3600 dotnet test DynaDocs.Tests --filter FullyQualifiedName~Survey

# RED: leave final tests/fixtures in place, revert only the production fix, always restore it.
(
  dyd964_saved=$(mktemp /tmp/dyd964-survey-fixed.XXXXXX)
  cp Services/Survey/SurveyRules.cs "$dyd964_saved"
  trap 'cp "$dyd964_saved" Services/Survey/SurveyRules.cs; rm -f "$dyd964_saved"' EXIT
  git show cc5e07ec5b886710e91be24cfeacba7d588fbb02:Services/Survey/SurveyRules.cs \
    > Services/Survey/SurveyRules.cs
  dyd964_run 8G 3600 dotnet test DynaDocs.Tests --filter FullyQualifiedName~Survey
) # expected exit 1, 21 failures
dyd964_run 8G 3600 dotnet test DynaDocs.Tests --filter FullyQualifiedName~Survey # expected 0
dyd964_run 8G 3600 .static-gates/python/bin/python DynaDocs.Tests/coverage/gap_check.py \
  test --stack dotnet -- --filter "$dyd964_light"
dyd964_run 8G 3600 .static-gates/python/bin/python DynaDocs.Tests/coverage/gap_check.py \
  gate static --stack dotnet
dyd964_run 8G 3600 .static-gates/python/bin/python DynaDocs.Tests/coverage/gap_check.py \
  gate coverage --stack dotnet
dyd964_run 4G 120 .static-gates/python/bin/python DynaDocs.Tests/coverage/gap_check.py \
  gate mutation --since cc5e07ec5b886710e91be24cfeacba7d588fbb02 --stack dotnet # expected 2
dyd964_run 4G 120 dotnet bin/Debug/net10.0/dydo.dll check
```

## Gates and retained evidence

| Check | Exit | Observed result |
|---|---|---|
| Facade capabilities | 0 | configured test/static/coverage; mutation declared unavailable |
| Native discovery | 0 each | Light 150; whole-dotnet coverage fallback 7,217 |
| Fix-reverted focused RED | 1 | 21 failed, 114 passed, no skips |
| Restored focused GREEN | 0 | 135 passed, no skips |
| Facade affected Light | 0 | 150 passed, no skips |
| Facade dotnet static | 0 | every selected static collector passed |
| Facade dotnet coverage | 0 | producer and consumer passed; 7,163 tests passed, 54 skipped |
| Facade dotnet mutation | 2 | unavailable: Pending DYD-103 |

SurveyRules: 100.00% line coverage (53/53) and
100.00% branch coverage (22/22).
Changed methods: Find HCRAP 6.0, cognitive 6, CanOffer HCRAP 1.0, cognitive 1.
The whole-dotnet pass does not prove the 54 skipped native/platform cases; their names are retained.
The campaign owner reported complete, cleanup confirmed and subject status 0; both owned test
worktrees were verified absent after cleanup.

Bounded original logs, exact command/exit/dirty-byte receipts, facade result receipts and
survey-specific static/coverage extracts live in `dyd-964-evidence/` beside this record.
The complete uncommitted native artifacts remain under
`DynaDocs.Tests/coverage/results/assurance/run-502651cb40dc4461919b7470de281057/` for coverage and
`DynaDocs.Tests/coverage/results/assurance/run-84dfc4f1d39944eab87c9192d2e18445/` for static;
`/tmp/dyd964-evidence/` also retains discovery output and the command wrapper.
The passing static and coverage adapters recorded the same source fingerprint
`a61709db3987bf150af59301cefb7d90b49b7a4c9ac24ef07f0e5dedff0cf32a`.
Only this evidence documentation was added after the final runtime/static measurements; the
five measured source/test/standard files were checked against their receipt hashes unchanged.
Documentation validation is retained in `dyd-964-evidence/docs.log` with its command/exit receipt.
The initial check returned 1 for a direct markdown link to a `.log` artifact; that reference was
changed to a code span, and both the initial and corrected results are retained.

## Gaps and hand-raise

Mutation is fail-closed unavailable under DYD-103. Local statechart coverage remains the inherited
unmet gate named in the Project's night orders; neither is a pass or holds the offer under those orders.
Hosted Windows proof and independent review belong to the captain's next stage.

F10 observation returned to the captain: check-in also maps `unblocked` to `commission`. This
commission's binding rule names `pickable` and `uncontracted`; `unblocked` and its skill mapping
retain their existing behavior. No skill prose is changed here.

code-writer/model: GPT-6.1-sol, high.

## Captain proof of record

The captain independently reran the writer's production-revert recipe on implementation commit
`e42a50eeeeeaadf8eb4ff3349230f93518c5809f`. The same focused survey command returned RED exit 1
with 21 failures and 114 passes, then restored GREEN exit 0 with 135 passes and no skips.
The fixed production file was restored byte for byte; its SHA-256 remains
`3e680e775e4173010ee7b93e955110e7c4207be054b19f440e6761c9ec6a378f`.
Both runs used the existing host lock, an 8G scope with swap disabled and a 3600-second deadline.
Exact argv, candidate, exits and source hashes are in `dyd-964-evidence/captain-proof.json`;
raw output is in `captain-red.log` and `captain-green.log` in that directory.
This evidence-only addition changes no measured source, tests or standard. The writer's Light,
static and coverage evidence is reused for those identical bytes; mutation and local statechart
coverage remain unavailable or unmet, never passed.

ticket-captain/model: gpt-6.1-sol, high.

## Related

- [Linear workspace standard](../../../../reference/linear-workspace-standard.md) — Type holders and pickup.
- [Testing strategy](../../../../guides/testing-strategy.md) — gate placement, prerequisites and coverage fallback.
- [Survey decision](../../../decisions/063-dydo-survey-read-only-lane-digest.md) — the existing read-only boundary.
