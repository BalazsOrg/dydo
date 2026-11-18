---
area: project
type: reference
---

# DYD-1187 split-page integration

Base: `be0ae28e71b71ff0a9f37593d22a363cc0e6b9ad`, the live
`feature/dydo-4-0-0` tip containing DYD-1191 / PR 360.

The two merge conflicts retain the reviewed feature's split-reference readers,
including per-page section completeness and duplicate detection. The about-page
parity assertion retains DYD-1187's allowance for repository-only `explains:`
metadata, narrowed to frontmatter. Its full body and every other field still
compare exactly with the Scaffold mirror. No runtime code changes in this hop.

Four short option-description paragraphs document `--base` for `check` and
`pr-ready`, each in its existing split page and Scaffold mirror. Eighteen
descriptive split pages gain frontmatter-only `explains:` lists, for 26 local
declarations overall. The three navigation hubs have no declarations; Scaffold
has no repository-specific declarations. Every other page body matches the live
base byte-for-byte. `body-audit.json` records the hashes, sizes and test-file
resolution. DYD-1192 records the separate preexisting file-size finding and its later deletion ruling.

## Proof

`proof.py.txt` runs from repository root under the session's 8G memory cap,
3600-second timeout and `/tmp/dydo-full-gate.lock`. `commands.json` records the
actual commands and exits. `test-results.json` records native TRX counters and
failures; concatenate each label's numbered `trx.gzpart-*` files in numeric
order and decompress with gzip for full raw receipts. Native nonempty discovery
is retained compressed. `source-hashes.json` identifies the tested source bytes.

The pre-edit command-boundary red is the integrated split-page
`AllOptions_AppearInReferenceDoc` failure for `--base` on `check` and `pr-ready`.
The same unchanged assertion supplies green. The reached Light selection keeps
the existing Explains, check, pr-ready, frontmatter, setup, Scaffold, canonical
skills, help and command-reference routes. Both Python owned-policy-doc cases
and `dydo check` (plain and against the pinned base) cover the restored consumer
routes and new declarations. See the native receipts for results.

Original runtime red/green and static receipts remain in the parent directory
at `b0b1056`; the runtime and focused test inputs are unchanged. The immediately
preceding `ci-fix` receipts are historical checkpoints, superseded by this
integration. The original drift/unlinked-why and out-of-scope skill wording
follow-ups remain in the ticket's STATE record and are not repaired here.

Coverage is report-only. Mutation, statechart and native Windows proof remain
unavailable and unmet on this host. Hosted Actions are a separate pending gate;
this child neither watches nor merges in this hop. Fresh exact-head review must
be recorded in the PR body's `## Independent review` section before handback.
