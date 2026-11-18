---
explains: ["Commands/ValidateCommand.cs", "Commands/GapCheckCommand.cs", "Commands/StatechartCommand.cs", "Services/Statechart/**", "Commands/CompletionsCommand.cs", "Commands/DydoRootCommand.cs", "Commands/HelpCommand.cs"]
area: reference
type: reference
---

# CLI Commands: Validation, Testing and Utilities

[CLI Commands Reference](./dydo-commands.md)

## Validation Command

### dydo validate

Validate `dydo.json` deserialization and nudge definitions: each nudge's pattern, message,
severity, audience, and `match` (absent, `command`, or `new-file`).

```bash
dydo validate
dydo validate --root <dir>
```

When `dydo.json` records `integrations.claude: true`, validate also checks that the committed
`.claude/settings.json` runs the guard: some `PreToolUse` hook's `command` is exactly
`dydo guard --host claude`, the one [`dydo init`](./dydo-commands-core.md#dydo-init) writes, whatever its matcher. A guard
only in `.claude/settings.local.json`, or a bare `dydo guard`, does not count. When the file is
missing, cannot be read, does not run the guard, or is not valid settings JSON, validate prints one
warning against `.claude/settings.json`: Claude agents run without the dydo guard. The fix is
`dydo init claude --join`. Make an unreadable file readable first. Fix the JSON first when the file
is not valid JSON, is not an object, or has a `hooks` that is not an object or a
`hooks.PreToolUse` that is not an array, because `dydo init claude --join` refuses such a file.
The warning alone leaves the exit code `0`. The guard's daily validation prints the same warning
and still allows the tool call.

This validates dydo's local configuration. It does not validate or provision Linear.

---

## Testing Command

### dydo gap-check

Run the project-configured coverage gap check.

```bash
dydo gap-check
dydo gap-check --force-run
```

The nearest `dydo.json` must contain `testing.runner`: a nonempty string array whose first item is
the executable and whose remaining items are fixed arguments. `dydo gap-check` starts it directly in
the configuration directory and appends every caller argument exactly as supplied. `--force-run`
above is such an argument: dydo's example runner defines it, and dydo itself has no such option. Its exit code is the
runner's exit code; configuration, startup, and cancellation failures exit `2`.

---

## Utility Commands

### dydo statechart

```bash
dydo statechart SPEC.tla [--config FILE] [--view FILE] [--out DIR] [--skill DIR]
```

Runs on Windows and Linux; other platforms are refused, and the skill's own
launchers remain available there. Captures the caller and original model inputs.
Missing prerequisites produce a locked JSON plan and saved request with exit `3`;
the initial invocation starts no installer. A verified ready invocation checks the
model without installing. Only complete checker verdicts `0` and `1` publish output.

### dydo statechart resume

```bash
dydo statechart resume --request ABSOLUTE_REQUEST_JSON
```

Submit the exact saved continuation through the host\'s normal installation
authorization mechanism. Resume verifies saved inputs and identities, installs
missing locked prerequisites into the planned managed roots, proves readiness and
continues the captured chart. A local flag or receipt is never host approval.
Failures preserve the pending request; changed inputs require a fresh plan.
Completed requests return the recorded verdict only while their output still matches.
Managed dependencies live in a folder named for their dependency lock. A dydo whose lock
changed installs into a new folder and keeps the old one, which another installed dydo
may still use; remove an old lock folder only when no installed dydo uses that lock.
Extracted assets are kept per release and payload for the same reason.

### dydo statechart install-skill

```bash
dydo statechart install-skill --root ABSOLUTE_PROJECT
```

Installs this executable's embedded statechart skill into the explicitly named
project and projects it into both agent discovery roots. The skill's receipt records
its installed files; after a dydo update, a skill whose files still match that record
is replaced with this release's. A receipt without that record is replaced only when
its files are already this release's. Foreign skills, local edits and any project setup
script stay untouched, and the refusal names the changed files and the remedy.
`--skill DIR` on chart invocation verifies the installed skill's exact release,
protocol, payload and file identities before checking; an unchanged skill from an
earlier release is refused with the `install-skill` command that updates it.

### dydo statechart doctor

```bash
dydo statechart doctor
```

On Windows, verifies and extracts the installed binary's exact statechart payload,
then inspects local Node/Java candidates and an explicit TLC jar without downloads.
Probes run hidden under registered native ownership. Diagnostic JSON distinguishes
candidate compatibility from verified checking readiness. Exit `2` means checking
readiness is not established; it is never a chart verdict or installation approval.
This operation changes no PATH, home settings or managed prerequisites.

### dydo completions

Generate a completion script for the requested shell.

```bash
dydo completions bash
dydo completions zsh
dydo completions powershell
```

### dydo version

```bash
dydo version
```

### dydo help

```bash
dydo help
```


## Exit Codes

| Code | Meaning |
|------|---------|
| `0` | Success or action allowed |
| `1` | Validation errors or command failure |
| `2` | Tool error or guard block |

## Related

- [CLI Commands Reference](./dydo-commands.md)
