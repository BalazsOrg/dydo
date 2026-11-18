---
area: project
type: changelog
date: 2026-11-18
---

# DYD-1191 reader implementation proof

code-writer/model: gpt-6.1-sol. Source: docs hop
`c3f914dff9a36426b722f05058eb3cbc7b967312`; posted base and integration target:
`56b9fd8dee8c7af7d4174572454e0f9670a9d27f`, `feature/dydo-4-0-0`.
Authority: active DYD-1191 description updated 2026-11-18T02:16:16.001Z and
admiral DECIDED `ddba537f-6fc2-4e7b-b2c9-e83c03b0e52a` on DYD-1187.
`pre-edit.json` proves branch, isolated root, ancestry, cleanliness and owned paths before edits.
No children, production changes, skill/claim edits, option wording, declarations, push or PR work.
All earlier docs-hop files and raw receipts remain unchanged.

## Implementation and reach

`CommandReferenceFamily` asserts the exact five-body set, existing hub and nonempty pages.
Command parsing reads each body separately, excludes hub stubs, rejects duplicate/empty sections,
and requires discovered flags. Option completeness retains reflection and every missing-option
assertion; parity retains its symmetric section/flag comparison; required-example checks retain
their assertions and additionally require actual example discovery. The retired-command scan
expands the original command document to hub plus bodies. Canonical guidance treats each original
local/Scaffold command entry as one logical family, retaining all positive, stale-path and negative
checks. Python policy reads coverage's hub and exact seven bodies, retaining every policy assertion.
No guard-specific single-file assertion needs adaptation; the docs-hop consumer inventory remains
the complete map. Help, embedded Scaffold resources and recursive Scaffold examples keep their
existing consumers and assertions.

Three C# classes become partials without changing their test identities or Reqnroll bindings.
The Python public classes keep every original method name and portable inheritance. Implementation
case classes are nested in ordinary containers so unittest collects them only through those public
classes. Helper classes have no discovered tests. All twelve Python files retain native unittest
test role; no new production module or association is needed. The AST audit proves all 144 original
Python methods remain once, including embedded child-script bytes; the sole existing method change
is the policy reader's call to the family loader. No assertion was removed or weakened.

## Observed proof

| Proof | Native result / exit | Raw receipt prefix |
|---|---|---|
| Original .NET readers in disposable baseline input | 29 discovered, 27 passed, 2 failed; exit 1 | baseline-dotnet-red |
| Original Python policy readers | 2 discovered, 2 failed; exit 1 | python-policy-before |
| Python policy after adaptation | 2 passed; exit 0 | python-policy-after |
| Moved .NET discovery | All 54 original IDs retained; reached selection widened to 116 | discovery-dotnet-before-host, discovery-dotnet-after |
| Python discovery | Exact same 232 IDs: 116 base + 116 inherited portable, no duplicates; exit 0 | discovery-python-before-host, discovery-python-final |
| Missing-option probe | Exact option test fails naming `--join (command: init)`; exit 1 | option-red |
| Option-parity probe | Names missing local `--join` for init/adopt/merge; 1 failed; exit 1 | parity-red |
| Required-example probe | Names `--root (command: dydo statechart install-skill)`; 1 failed; exit 1 | example-red-clean |
| Retired-command negative scan | Finds injected `dydo task` on utilities body; 1 failed; exit 1 | negative-scan-red |
| Canonical-policy negative scan | Finds injected `edit both` on utilities body; 1 failed, 7 passed; exit 1 | canonical-negative-red |
| Final native Python Light | 232 discovered, 230 passed, 2 Windows-only skips; exit 0 | python-light-release |
| Final reached .NET Light after all red probes | 116 passed, zero failed/skipped; exit 0 | dotnet-light-release |
| Changed-file native statics | Zero findings/errors; exit 0 | changed-statics-locked, static-v4/ |
| Exact docs reconstruction | Digest unchanged, 25 moves, 111 anchors, 148 fragment links; exit 0 | release-docs-audit |
| Native dydo check | Zero errors/warnings, 928 files; exit 0 | release-docs-check |
| Assertion/discovery preservation and whole-ticket byte gate | All original methods/IDs retained, all changed files below 20,000 bytes; exit 0 | release-preservation |
| Whitespace static, including staged new files | No findings; exit 0 | release-whitespace |

Negative probes change only the normal test adapter's disposable input. Option/parity remove
every `--join` occurrence from all local family pages, leaving registry, assertions and Scaffold
unchanged. The example probe changes only the real install-skill example line. Source family
bytes are checked after every probe, and the adapter removes its owned Git registration and tree.
Final Light and byte-gate receipts carry the final source hashes; the captain commands below
repeat the same native selections with fresh receipt names.

## Captain proof recipe

Run from this checkout. The prepared environment is linked at `.static-gates/python`;
`preflight` checks CPython 3.14.4 and every exact requirements.lock version, SDK 10.0.303,
runtime 10.0.14, Node 26.13.0 and the real selected command grammar. Native .NET discovery/test
uses the repository's isolated adapter and restores locked dependencies. Static restore explicitly
uses `--locked-mode`. The native linter's dependencies were restored offline into this checkout
with `pnpm -C DynaDocs.Tests/coverage install --frozen-lockfile --offline` (228 reused, zero
downloaded); preflight verifies every installed package and link against the lock. No unpinned
dependency was installed. The restore receipt is `node-locked-restore`; final preflight is
`preflight-locked`.

Each `proof.py` call derives the nearest session slice from `/proc/self/cgroup`, sets
`MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false`, and runs
`systemd-run --user --scope --slice=<nearest slice> -p MemoryMax=8G -p MemorySwapMax=0
-p OOMPolicy=continue timeout 3600 <command>` for `--full`, or 4G/120 seconds otherwise.
The wrapper is outside its child cap; any required host lock also belongs outside it.
Receipt names are exclusive: choose a fresh prefix on a repeated run. The following `captain-*`
names are unused in this hop. Expected-red commands return 1; the rest return 0.

```bash
p=dydo/project/changelog/2026/2026-11-18/dyd-1191/reader-proof
python3 -B "$p/proof.py" captain-preflight .static-gates/python/bin/python -B "$p/preflight.py"
python3 -B "$p/proof.py" captain-python-discovery python3 -B "$p/discover.py"
python3 -B "$p/proof.py" captain-baseline-red python3 -B "$p/baseline.py"
python3 -B "$p/proof.py" captain-option-red python3 -B "$p/negative.py" option
python3 -B "$p/proof.py" captain-parity-red python3 -B "$p/negative.py" parity
python3 -B "$p/proof.py" captain-example-red python3 -B "$p/negative.py" example
python3 -B "$p/proof.py" captain-negative-red python3 -B "$p/negative.py" negative
python3 -B "$p/proof.py" captain-canonical-red python3 -B "$p/negative.py" canonical
python3 -B "$p/proof.py" --full captain-python-light python3 -m unittest discover -s DynaDocs.Tests/coverage/tests -p 'test_testing_facade*.py' -v
python3 -B "$p/proof.py" --full captain-dotnet-discovery python3 DynaDocs.Tests/coverage/run_tests.py -c Release --filter 'FullyQualifiedName~CommandDocConsistencyTests|FullyQualifiedName~HelpCommandTests|FullyQualifiedName~ScaffoldTreeTests|FullyQualifiedName~CanonicalSkillAssertionTests|FullyQualifiedName~OneCanonicalSkillTreeReachesEverySupportedHostFeature|FullyQualifiedName~RunsTestsAndAssuranceHonestlyFeature' --list-tests
python3 -B "$p/proof.py" --full captain-dotnet-light python3 DynaDocs.Tests/coverage/run_tests.py -c Release --filter 'FullyQualifiedName~CommandDocConsistencyTests|FullyQualifiedName~HelpCommandTests|FullyQualifiedName~ScaffoldTreeTests|FullyQualifiedName~CanonicalSkillAssertionTests|FullyQualifiedName~OneCanonicalSkillTreeReachesEverySupportedHostFeature|FullyQualifiedName~RunsTestsAndAssuranceHonestlyFeature'
python3 -B "$p/proof.py" --full captain-statics .static-gates/python/bin/python -B "$p/statics.py" captain-static
python3 -B "$p/proof.py" captain-docs-audit python3 -B dydo/project/changelog/2026/2026-11-18/dyd-1191/audit.py --docs-only
python3 -B "$p/proof.py" captain-docs-check dydo check
python3 -B "$p/proof.py" captain-preservation python3 -B "$p/preservation.py"
python3 -B "$p/proof.py" captain-whitespace git diff --check
```

`preservation.py` compares current ASTs with the pinned source and retained before/final native IDs,
checks test roles and owned paths, and enforces strictly less than 20,000 bytes on every changed or
new file across the whole ticket. It works before and after this hop's commit. Docs audit verifies
the earlier snapshot digest `bc6f2a7cdd8f7562e694fc5bdc9b6096ae311cc2ee41bca57edead72563d87b2`.
`proof.py` receipts carry exact argv, source SHA-256, HEAD, environment, native output and exit.
Raw text is losslessly split into at most 16,000-byte parts. Static native commands and reports are
losslessly gzip-compressed and split into the same bound; concatenate a manifest's `gzip_parts`,
decompress, and verify its SHA-256 to recover the exact raw bytes. `static-v4/static-manifest.json`
points to the passing report, which lists each raw command's parts and hashes. Temporary static
directories are removed after retaining their bytes. Earlier unsuccessful wrapper/provisioning and
audit attempts remain recorded; they are superseded by the named successful receipts, never passes.
The first static run's Vulture-only findings led to removing custom discovery hooks and analyzing
dead-code readers over the complete program before restricting its verdict to changed files.
The staged whitespace check found trailing blank lines in three newly split helper files; those
were removed and the final statics and Light selections were rerun on the resulting source bytes.

## Gaps and next owner

DR068 governs: coverage thresholds and HCRAP are not gates, and no coverage report was produced in
this test-reader hop. Mutation, statechart assurance and native Windows proof remain unavailable
and unmet. Python's two Windows-only cases are discovered but skipped on Linux, never passed.
Independent review, captain rerun, ticket-wide final gates and integration remain captain-owned.
This evidence is an implementation return, not independent review PASS or ticket completion.

## Related

- [Reader handoff](../reader-handoff.md)
- [Documentation design and byte proof](../implementation.md)
