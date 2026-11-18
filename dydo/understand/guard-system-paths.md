---
area: understand
type: concept
---

# Guard System: Path Tiers

[Guard System](./guard-system.md)

## Layer 1: Path Tiers

`dydo/files-off-limits.md` declares two tiers, both applying to every caller.

**Off-limits** patterns hard-block **every** operation - read, write, search, or bash.

- **Covered:** secrets and credentials (`.env*`, `*.pem`, `*.key`, `secrets.json`, database configs) plus the hardcoded `dydo/_system/**` machine state.
- **Whitelist:** a `## Whitelist` section carves exceptions (e.g. `.env.example`) to the configured patterns - never to the hardcoded ones.
- **Patterns** use glob syntax: `**/` for optional directory prefix, `**` for any path, `*` within a segment, `?` for a single character; matching is case-insensitive on every platform.
- Host memory folders outside the repo (Claude's `projects/*/memory/` and `agent-memory*/`, Codex's `memories/`) are readable past these patterns, but every agent write to them is blocked ([DR 055](../project/decisions/055-native-agent-memory-off-and-guarded.md)): a lesson goes into a skill, a Decision Record or a doc.

```
BLOCKED: Path is off-limits to all agents.
  Path: .env
  Pattern: .env*
  Configure exceptions in dydo/files-off-limits.md
```

**Protected** patterns (a `## Protected Patterns` section) invert the emphasis: **every agent may read them, none may write or delete them** ([Decision 045](../project/decisions/045-flow-map-hats-review-tiers-and-working-tree-contract.md) §10). Membership is dydo's own system files - `dydo/index.md`, `dydo/files-off-limits.md` and the hardcoded `dydo.json` - because agents read them to orient themselves, `dydo/index.md` on every entry prompt's order. The tier's contract is that no agent writes them *directly*, and the user owns their content; dydo's own commands still rewrite what they manage - `dydo index` regenerates `dydo/index.md`, and `dydo fix` writes `dydo.json`. Those arrive as `dydo` command lines with no file path to extract, so they pass: the tier stops hand edits, not dydo's tooling. `CLAUDE.md`, `AGENTS.md` and harness config files stay outside the guard entirely: the harness defends its own files, and off-limits keeps its original meaning of files agents must not even read.

The tier binds on the mutating call only: the `Edit`, `Write` and `NotebookEdit` tools, Codex's `apply_patch`, a CLI `--action` of `edit`, `write` or `delete`, and every operation the bash analyzer extracts from a shell command that is not a read of that path - write, delete, move, copy, permission change. `Read`, `cat`, `head` and search pass. The whitelist does not apply.

A shell command is checked by the file operations its text names. Two kinds of code whose file operations that text does not show are refused outright, whatever they touch: inline interpreter code (`python -c`, `node -e`, `ruby -e`, `perl -e`, `php -r`) and a Python program read from stdin (`python -` or `py -`, or `python` given no script while a heredoc, here-string, pipe or `<` redirection feeds its stdin). That Python is the command's program, or the program a runner's `run` subcommand starts, by one shape for any runner: the runner's first operand is `run` and Python is the first operand after it, options allowed before each, and a word after an option may be its value, so every `run` and Python word that may be the first operand is checked. So `uv run python - <<'EOF'`, `poetry run python3 -`, `uv --directory tools run python -`, `uv --directory run run python -` and `printf ... | uv run --with requests python` are refused, while `uv run python script.py`, `uv run python -m pytest`, `poetry run pytest` and `uv pip install ...` pass. Python behind a runner with no `run` subcommand (`uvx python -`, `pyenv exec python -`) or behind a second runner that `run` starts (`uv run poetry run python -`) is not recognised, and because the shape names no tool, a fed `python` word another tool's `run` receives is refused too (`echo x | cargo run -- python`). The line is read as its shell reads it: a `<` or `|` in a comment or quoted text feeds nothing, and a pipe feeds the command it leads into also as the first command of a `( )` or `{ }` group and inside a `$(...)` or backtick substitution, double-quoted or not, so `printf ... | (python3)` and `echo "$(printf ... | python3)"` are refused. Nothing is read from stdin when nothing feeds it: a bare `python` prompts, `command -v python3` only looks it up, and `xargs python3` gets its script from xargs. Any other interpreter reading a program from stdin (`node - <<'EOF'`, `cat x.js | node`, `ruby` or `perl` fed a program) is not given Python's blanket refusal, and its program is not followed, like a script file; the ordinary raw-text and path checks still apply to its command line, a heredoc body it reads included, as shell text. So `perl <<'EOF'` whose body holds `open F, '>', 'notes.md'`, read as a redirection, is still refused as an unverifiable write, while a `node - <<'EOF'` body calling `writeFileSync` passes with a warning. A script file runs, and its own file operations are not followed: a `python tools/x.py` that writes a protected file is not stopped. The tier stops accidents, not a deliberate route around it.

```
BLOCKED: Path is protected - every agent may read it, none may write or delete it.
  Path: dydo/index.md
  Pattern: dydo/index.md
  Detected: Write via sed -i
  This file is human-owned: read it freely, and ask the user for any change.
```

---

## Related

- [Guard System](./guard-system.md)
