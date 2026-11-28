# Setup-animatic provenance

Source: [mattpocock/skills, skills/in-progress/setup-animatic](https://github.com/mattpocock/skills/tree/71009050da79506823464512ee76d71bbc014ce1/skills/in-progress/setup-animatic),
commit `71009050da79506823464512ee76d71bbc014ce1`, the head of the open pull request
[#1422](https://github.com/mattpocock/skills/pull/1422), the same pin as `../animatic/`. MIT,
Copyright (c) 2026 Matt Pocock; the license text is in the repository's `THIRD-PARTY-NOTICES.md`.

`agents/openai.yaml` is byte-identical to upstream. The Markdown files carry an adapted-from header
and light dydo tailoring, nothing more:

- The config lives at `dydo/reference/animatic.md`, in place of `docs/agents/animatic.md`, and the
  seed `animatic.md` carries dydo frontmatter so `dydo check` passes. A style sheet moves to
  `dydo/reference/animatic.css` with it.
- In place of the `## Agent skills` pointer in `CLAUDE.md` or `AGENTS.md`, the skill adds one
  Contents line to `dydo/reference/_reference.md` when it is missing.
- An existing config, dydo's shipped default included, gives the recommended answers.
- A step that installs a tool asks the user first. On Linux x64, Kokoro's `setup` needs
  `npm_config_onnxruntime_node_install_cuda=skip`, and Kokoro speaks only after `setup`, so `setup`
  runs before `say` for it.

dydo ships a default config built from the seed: `Scaffold/dydo/reference/animatic.md`, which
`dydo init` writes, with local Kokoro `af_heart` as its voice so that no line leaves the machine.
