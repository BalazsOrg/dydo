---
area: reference
type: reference
---

# CLI Commands: Testing and Utilities

[Overview](./dydo-commands.md).

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

- [DynaDocs](./about-dynadocs.md) - Product boundary and operating model
- [Files Off-Limits](../files-off-limits.md) - The two path tiers `dydo guard` enforces
- [Writing Documentation](./writing-docs.md) - Documentation conventions validated by dydo
