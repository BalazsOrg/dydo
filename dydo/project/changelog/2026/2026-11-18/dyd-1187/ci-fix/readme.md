---
area: project
type: reference
---

# DYD-1187 restoration checkpoint

This checkpoint supersedes the original implementation's documentation splits and
25-page declaration inventory. The parent directory's implementation and proof
receipts describe b0b1056485d7fcde2e1a414eda50886cfe4ee814, not this repaired tree.
The current declaration inventory has eight entries.

Per Admiral DECIDED ddba537f on DYD-1187, DYD-1191 must land first. This checkpoint
restores bodies, removes all 18 split pages, includes feature tip
56b9fd8dee8c7af7d4174572454e0f9670a9d27f, and repairs the two Explains service
static findings. It deliberately omits new `--base` reference descriptions.
It is an incomplete checkpoint, not a merge-ready result.

## Current proof

- Light: 741 executed, 740 passed, one failed. `light-results.json` captures the
  exact failure: `AllOptions_AppearInReferenceDoc`, missing `--base` for `check`
  and `pr-ready`. Assertions are preserved. The canonical-source scenario passes.
- Both Python `test_owned_policy_docs_replace_tiers_with_the_dr048_gate_set`
  cases pass; `python-doc-tests.log` is the native output.
- The retired static collector gives zero findings on the full downloaded CI source
  inventory. This is the exact collector that reported the two original Python
  static findings; it is not a fresh whole Python coverage run.
- `body-audit.json`: all eight grandfathered documents equal feature tip after
  removing only their added `explains:` line. The three oversized original docs,
  scaffold command reference and scaffold writing guide equal feature tip
  byte-for-byte. No split pages remain. No new `--base` doc lines are included.
- `candidate-inputs.json` pins current code, tests and descriptive docs
  by SHA-256. Runtime behavior is unchanged from the original candidate.

`proof.py.txt` and `commands.json` retain the rerun selection and command exits.
The recipe expects artifacts from Actions run 44216212758 under
`/tmp/dyd-1187-ci-python`; download that run's Python assurance artifact there to
provide its native inventory and static report. Run the recipe from repo root
with the session's 8G memory cap, 3600-second timeout and full-gate lock.
The full TRX stays at `/tmp/dyd-1187-ruling-proof/light/light.trx`; its digest and
native counters/failure are retained here. Test discovery exited zero.

## Attribution and limits

`coverage-failure.json` confirms the original Linux coverage job failed the same
canonical-source functional scenario as build, not a coverage threshold:
measurement complete, no coverage gaps. `red.trx` retains that pre-repair failure.
`python-static-attribution.json` retains the two PR-caused static findings.
The two Python documentation failures came from moving the word "seven" off the
original coverage reference. Body restoration repairs both regressions without
weakening tests.

No all-files DR 069 pass is claimed: `size-report.json` reports the preexisting
file-size finding tracked by DYD-1192. The oversized authored docs are untouched relative to feature tip.
No authored-file exception is introduced. DYD-1191 owns their separate reviewed
split and blocks completion of the missing `--base` documentation.

Round-2 independent review on cbfb17d9 verified the restoration and static
repair but returned FAIL for the deferred `--base` documentation. It also found
two should-level gate gaps: this evidence file's uppercase filename (DYD-1193),
and the file size (DYD-1192). The evidence file is now `readme.md`; the capped
`dydo check` rerun exits 0 with zero errors/warnings in 909 files. See
`dydo-check.log`. DYD-1192 requires a disposition beyond DYD-1191; no file-size
exception or final size pass is claimed. No code, tests or grandfathered docs
changed in the filename-repair hop, so their round-2 evidence is reused.

Mutation, statechart and native Windows proof remain unavailable and unmet.
Coverage is report-only. Hosted Actions on the pushed checkpoint remain a
separate result. This child does not watch the PR or merge at this checkpoint.
