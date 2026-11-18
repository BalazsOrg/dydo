---
area: understand
type: reference
explains: ["Commands/GuardCommand.cs", "Commands/GuardCommand.Refusal.cs", "Services/OffLimitsService.cs", "Services/BashCommandAnalyzer*.cs", "Services/ClaudeGuardWiring.cs"]
---

# Guard System: Hook and Path Tiers

[Overview](./guard-system.md).

## How the Hook Intercepts Tool Calls

The guard integrates with the platform through the **PreToolUse** hook event. Before each matched tool call, the platform pipes a JSON payload to `dydo guard` via stdin:

```json
{
  "session_id": "abc123",
  "tool_name": "write",
  "tool_input": {
    "file_path": "src/foo.cs",
    "content": "..."
  },
  "hook_event_name": "PreToolUse"
}
```

The guard evaluates the request and returns:

- **Exit 0, empty stdout** - action allowed (a `NOTICE:` or `WARNING:` on stderr may ride along)
- **Exit 0, one deny decision on stdout** - action blocked. Stdout holds exactly one object,
  `{"hookSpecificOutput":{"hookEventName":"PreToolUse","permissionDecision":"deny","permissionDecisionReason":"<text>"}}`,
  where the reason is everything the call wrote to stderr: the `BLOCKED: <reason>` line, its
  follow-up lines and any `WARNING:` lines. The same text still goes to stderr. Non-ASCII is
  escaped, so a shell that re-encodes stdout cannot break the object.

A block decides through stdout rather than exit 2 because Codex runs a hook through its session
shell, `powershell -NoProfile -Command "dydo guard"` on Windows, and Windows PowerShell and pwsh
both turn a native exit 2 into exit 1 there; Codex treats exit 1 as a failed hook and runs the
tool. Codex and Claude Code both honour the deny decision at exit 0 (LC-1021). A payload naming
another `hook_event_name` keeps the old exit 2 with stderr only, and so does the CLI-argument mode.

`dydo init` wires the hook into a committed file, so a fresh clone is guarded: `.claude/settings.json` on Claude, `.codex/hooks.json` on Codex ([DR 056](../project/decisions/056-claude-guard-in-committed-settings.md)). A teammate without a dydo CLI gets a non-blocking hook error on each matched call, and nothing is guarded for them. Init installs the matcher that decides which calls arrive: on Claude `Edit|Write|Read|Bash|Glob|Grep|Agent|EnterPlanMode|ExitPlanMode|PowerShell|NotebookEdit`, on Codex `Bash|apply_patch|Edit|Write|Agent|shell_command|exec|local_shell|unified_exec`. A tool outside its host's matcher never reaches the guard.

There is no identity, no staging, no per-role permission matrix: the path tiers, dangerous-command rules and command nudges apply to every caller, every time ([Decision 041](../project/decisions/041-dydo-cedes-orchestration-becomes-authoring-knowledge-layer.md)). One caller *kind* is distinguished, and it is not a role: a call carrying `agent_id` is a sub-agent (the host sends it only for a native in-process one, Claude's `Agent` tool or Codex's `spawn_agent`; a T3 `delegate_task` child is a separate session whose calls carry none, so the guard reads it as top-level and it is no evidence of this lane, DYD-1072), which may run only the `dydo` gate and read-only commands its work needs: `check`, `fix`, `gap-check`, `graph`, `validate`, `mode`, `archive` (verified archive/delete, list and create-only restore), `clean` (a Ticket Captain removes its own worktree on close-out), `pr-ready` (it checks its tree before opening a PR), `merge` (a Merge Sub-ticket's code-writer integrates a reviewed PR into its feature branch), `survey` (the admiral check-in's surveyor reads a Project's lanes), `statechart` (crew draw a checked model; the guard reads only the first word, so its `install-skill` and `resume` verbs pass with it), `help`, `version` and `-h`/`--help`/`--version`. Setup, generation and hook commands (`init`, `adopt`, `index`, `guard`, `map`, `completions`, `_complete`) stay with the top-level session. Every command the CLI registers sits on exactly one of those two lists, kept in one place, `Commands/GuardCommand.WorkerLane.cs`; the refusal text prints them, a registry test fails on a command on neither, and a skill lint fails when prose addressed to a sub-agent names a command the worker lane refuses. A command the 3.3.0 pattern does not match (`dydo ` at the start or after `;`, `&` or `|`) is allowed exactly as in 3.3.0, so `find dydo -name '*.md'`, a continued `git diff \` line ending in `dydo src`, and `echo dydo sync` pass. A matched command is allowed only when, after line continuations are joined and quotes and escapes removed, every `dydo` token anywhere in it that is followed by a word is followed by an allowed one, so `dydo check && dydo init` and `dydo check $(dydo init)` are blocked, and so is `dydo check dydo src`, where the directory `dydo` reads as a call.

---

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
