# Animatic provenance

Source: [mattpocock/skills, skills/in-progress/animatic](https://github.com/mattpocock/skills/tree/71009050da79506823464512ee76d71bbc014ce1/skills/in-progress/animatic),
commit `71009050da79506823464512ee76d71bbc014ce1`, the head of the open pull request
[#1422](https://github.com/mattpocock/skills/pull/1422). The skill is on neither upstream `main`
nor the pin the other adapted skills record. MIT, Copyright (c) 2026 Matt Pocock; the license
text is in the repository's `THIRD-PARTY-NOTICES.md`.

`scripts/` and `kit/` are byte-identical to upstream. The Markdown files outside `kit/` carry an
adapted-from header and light dydo tailoring: the config path `dydo/reference/animatic.md`,
reading and posting a PR on GitHub and a ticket in Linear, and the PR's merger as its viewer. The
upstream `skills/in-progress/setup-animatic/` is adopted beside it, at the same commit, as
`skills/productivity/setup-animatic/`; its own `PROVENANCE.md` records its tailoring.

## Upstream JavaScript blobs

Each upstream JavaScript file under `scripts/` at the pin, with its Git blob hash as
`git ls-tree` prints it. dydo's assurance gates measure a local file under `scripts/` whose blob
hash differs from its line here, or that has no line. A byte-identical file stays vendored upstream
code. Change this list only when the pin moves.

```upstream-blobs
aa9565e49c6b1cb1a40cc0ed8647805bccc8da03  scripts/animatic.mjs
6c622f32cf43eeb0368c71dc5d92f127931a14db  scripts/frames.mjs
```
