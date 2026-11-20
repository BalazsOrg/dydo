---
explains: ["Commands/GuardCommand.cs", "Commands/GuardCommand.WorkerLane.cs", "Services/ClaudeGuardWiring.cs", "setup-skills.mjs"]
area: understand
type: concept
---

# Guard System: Hook and Integration

[Guard System](./guard-system.md)

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

There is no identity, no staging, no per-role permission matrix: the path tiers, dangerous-command rules and command nudges apply to every caller, every time ([Decision 041](../project/decisions/041-dydo-cedes-orchestration-becomes-authoring-knowledge-layer.md)). One caller *kind* is distinguished, and it is not a role: a call carrying `agent_id` is a sub-agent (the host sends it only for a native in-process one, Claude's `Agent` tool or Codex's `spawn_agent`; a T3 `delegate_task` child is a separate session whose calls carry none, so the guard reads it as top-level and it is no evidence of this lane, DYD-1072), which may run only the `dydo` gate and read-only commands its work needs: `check`, `fix`, `gap-check`, `graph`, `validate`, `mode`, `archive` (verified archive/delete, list and create-only restore), `clean` (a Ticket Captain removes its own worktree on close-out), `pr-ready` (it checks its tree before opening a PR), `merge` (a Merge Sub-ticket's implementer integrates a reviewed PR into its feature branch), `survey` (a sub-agent reads a Project's lanes for its officer), `statechart` (crew draw a checked model; the guard reads only the first word, so its `install-skill` and `resume` verbs pass with it), `help`, `version` and `-h`/`--help`/`--version`. Setup, generation and hook commands (`init`, `adopt`, `index`, `guard`, `map`, `completions`, `_complete`) stay with the top-level session. Every command the CLI registers sits on exactly one of those two lists, kept in one place, `Commands/GuardCommand.WorkerLane.cs`; the refusal text prints them, a registry test fails on a command on neither, and a skill lint fails when prose addressed to a sub-agent names a command the worker lane refuses. A command the 3.3.0 pattern does not match (`dydo ` at the start or after `;`, `&` or `|`) is allowed exactly as in 3.3.0, so `find dydo -name '*.md'`, a continued `git diff \` line ending in `dydo src`, and `echo dydo sync` pass. A matched command is allowed only when, after line continuations are joined and quotes and escapes removed, every `dydo` token anywhere in it that is followed by a word is followed by an allowed one, so `dydo check && dydo init` and `dydo check $(dydo init)` are blocked, and so is `dydo check dydo src`, where the directory `dydo` reads as a call.

---

## Housekeeping Rides Along

Because the guard runs on every matched tool call, it carries throttled maintenance jobs: a **daily validation** (config checks, report-only, never blocks).

---

## Integration for Other AI Tools

Any coding tool can integrate through two input modes:

- **Stdin JSON** (preferred for hooks) - the payload shown above; `file_path` for file tools, `command` for bash, `path` for search tools. Exit 0 with empty stdout allows; exit 0 with the deny decision on stdout blocks.
- **CLI arguments** (for testing) - `--action {edit|write|delete|read}`, `--path <path>`, `--command <command>`. Exit 0 allows; exit 2 blocks with the message on stderr. An argument starting with `@`, such as a PowerShell here-string `--command "@'…'@"`, is text and gets a verdict; the guard never reads it as a response file.

---

## Related

- [Guard System](./guard-system.md)
