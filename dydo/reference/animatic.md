---
area: reference
type: reference
---

<!-- Adapted from mattpocock/skills setup-animatic at 71009050da79506823464512ee76d71bbc014ce1 (PR #1422) (MIT). -->

# Animatic

The `animatic` skill's toolchain for this repo. This is dydo's shipped default, not yet tested on this machine: run `setup-animatic` to test it or change a tool, or edit it by hand.

## Flags

Pass these, unchanged, to every `animatic.mjs` command (`build`, `say`, `setup`). Speech is cached per voice flag, so a run with different flags re-voices every line.

```
--engine kokoro --voice af_heart
```

## Stages

- **Voice**: Kokoro, `af_heart` (a local model on the CPU, offline, no key, so no line leaves the machine). The first `setup` installs it under `~/.cache/animatic` (about 90MB). On Linux x64, run that `setup` with `npm_config_onnxruntime_node_install_cuda=skip` in its environment, or the install fails on a CUDA download that returns 404.
- **Renderer**: generated frames on the kit (Playwright).
- **Style**: the plain kit.
- **Stitcher**: ffmpeg, through `build`.

## Secrets

None.

## Tested

Not yet on this machine: this is dydo's shipped default.
