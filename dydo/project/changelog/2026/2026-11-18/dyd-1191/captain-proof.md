---
area: project
type: changelog
date: 2026-11-18
---

# DYD-1191 captain proof of record

Ticket-captain/model: gpt-6.1-sol. Reader hop `6e51a3ede06d15f13acf1ed078b7f38b40ea4cfb`
was synchronized without conflicts with feature tip `a6eda0dba0284013ab79c3bca9057a7d5a4c5806`,
producing `3aebd86b0b2b98b78c50140408ee34765e601e37`. All native reruns below use that
integrated source. The following evidence-only packaging changes leave every product document and
test-source hash unchanged. Review judges the final committed candidate, including this evidence.

## Captain rerun

Raw receipts are in `reader-proof/captain-*.log`; each records exact argv, source hashes, HEAD,
environment, cap, output and exit. Original writer receipts remain unchanged. Commands follow the
[reader recipe](./reader-proof/readme.md#captain-proof-recipe), with literal paths in the shell.

- Host preflight: exit 0; locked Python, .NET and Node prerequisites verified. The first sandbox
  attempt could not reach systemd, exit 1; the authorized host rerun passed with the required caps.
- Native discovery: Python 232 unique IDs, .NET 116 unique IDs, exits 0.
- Pinned old readers: 27 passed, 2 failed, exit 1, reproducing the split's two .NET reader failures.
- Removing every local `--join` mention: the unchanged option assertion fails naming
  `--join (command: init)`, exit 1. Parity independently names init/adopt/merge, exit 1.
- Required-example, retired-command and canonical-guidance negative probes: each exits 1 with
  its intended diagnostic; source family bytes are verified unchanged after every disposable run.
- Restored Python Light: 232 run, 230 passed, two Windows-only skips, exit 0.
- Restored reached .NET Light: 116 passed, zero failed/skipped, exit 0.
- Changed-source native statics: zero findings/errors, exit 0; lossless raw reports in
  `reader-proof/captain-static/`, manifest SHA-256
  `39cfad1afa68d67a20e90c143f2578c0f26ccba3eaf6a888f801074798e969c4`.
- Exact docs audit: original pinned prose, 25 moves, 111 old anchors and 148 inbound fragments
  preserved; original documentation digest unchanged, exit 0.
- Native local `dydo check`: 929 files, zero errors/warnings, exit 0.
- Scaffold adopter fix/check: zero fixes/errors; the same two baseline customization warnings,
  exit 0; source bytes unchanged and owned scratch removed.
- Preservation: all 144 original Python methods, native IDs and source-bound static artifacts
  verified; all ticket changes strictly below 20,000 bytes, exit 0.

The first docs audit after synchronization passed every byte/anchor check but rejected inherited
DYD-1155 evidence as outside this ticket. The retained audit now accepts an explicit `--scope-base`
for ownership/changed-file checks while still reconstructing prose from the original pinned base.
The passing rerun uses `--scope-base a6eda0dba0284013ab79c3bca9057a7d5a4c5806`; no inherited
change is claimed as this ticket's work. `reader-proof/captain-preservation.py.txt` similarly checks
the synchronized ticket diff and the original preservation/discovery invariants.

## Evidence packaging

Retained one-off scripts now use `.py.txt`, following the prior DYD-1155 correction: historical
proof recipes are documentation artifacts, rather than new maintained Python runtime targets.
Raw receipts preserve their actual original argv. Recipe references and the two explicit audit
loaders follow the new filenames. No source-inventory exclusion or gate waiver was added. The
captain's preservation helper derives the checkout root from its location for replay elsewhere.

For a fresh documentation audit, run the renamed recipe through the capped reader wrapper with a
fresh receipt label and `--docs-only --scope-base a6eda0dba0284013ab79c3bca9057a7d5a4c5806`.
The original reader-hop `preservation.py.txt` retains its original hop scope; use
`captain-preservation.py.txt` for this synchronized candidate. Neither operation authors docs.

Statechart, mutation and native Windows assurance remain unavailable and unmet. The two Python
Windows skips are never counted as passes. Hosted Actions and independent review remain owed at
this checkpoint. The next owner is the captain, commissioning fresh Claude Opus high review.

## Related

- [Documentation design and consumers](./implementation.md)
- [Reader implementation and native proof](./reader-proof/readme.md)
