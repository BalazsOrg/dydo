---
area: general
type: guide
---

# Changelog records and gate evidence

Keep the human-written Markdown record in `dydo/project/changelog/`: what changed, why,
the deciding observations, and the ticket and commit identities. A short account of a gate's
result belongs in that record. Its generated output does not.

Gate evidence is anything produced by a gate run: console logs, TRX and other test results,
coverage reports, JSON receipts, snapshots, patches, archives and archive fragments, checksums,
binary receipts, and generated reports, including generated Markdown. Keep it in ignored local
output, agent scratch, or CI artifacts. Never depend on old evidence for runtime behaviour or a
future gate. See [DR 067](../project/decisions/067-what-tests-are-for.md).

`dydo check` fails and `dydo pr-ready` refuses tracked evidence in the configured docs root's
`project/changelog/`. This applies even to a scoped check or a command run from a subdirectory.
The check reads Git's index, so deleting or replacing a working file does not hide its tracked
receipt. A staged removal stops tracking it; pr-ready separately requires a clean, pushed tree.

`pr-ready` uses project configuration only when that project belongs to the current Git
repository. A nested repository without its own project is checked at its own conventional
`dydo/project/changelog/` path, without inheriting the outer project's configuration.
A project's own docs root linked into another repository is still refused.

The mechanical rule permits regular, non-executable UTF-8 `.md` files without NUL bytes.
It rejects every other file type and every file beneath `artifacts`, `coverage`, `evidence`,
`logs`, `receipts`, or `testresults` (case-insensitive). Those directory names are reserved for
generated output. Markdown authorship remains a review responsibility: renaming generated text
to `.md` does not make it a human-written record. `.gitignore` prevents accidental additions;
the commands still reject forced additions. Test fixtures outside the changelog are unaffected.

DYD-1206 removes existing receipts from the current tree. A separate, human-confirmed one-time
rewrite removes their historical paths from `feature/dydo-4-0-0` before landing. The rewrite
preserves the human records and every other branch, and creates a backup tag first.
