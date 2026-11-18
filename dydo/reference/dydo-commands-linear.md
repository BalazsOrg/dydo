---
explains: ["Commands/MapCommand.cs", "Services/Map/**", "Commands/SurveyCommand.cs", "Services/Survey/**", "Commands/ModeCommand.cs", "Services/ModeState.cs"]
area: reference
type: reference
---

# CLI Commands: Linear and Mode

[CLI Commands Reference](./dydo-commands.md)

## Linear Commands

### dydo map

Serve a read-only, local browser map of one Linear Project.

```bash
dydo map                  # print the URL and open it in the default browser
dydo map --no-browser     # print the URL only
```

`dydo map` reads a personal Linear API key from `LINEAR_API_KEY`. Without one it prints how to create
a key and exits `2` before any network call. Otherwise it serves `http://localhost:<free port>/`,
prints that URL, opens it unless `--no-browser` is given, and runs until Ctrl+C. The server answers
GET only; the key stays in the dydo process and never reaches the browser.

Create a personal API key in Linear under Settings > Account > Security & access > Personal API keys.
Replace `<your key>` in the commands below with that key.

On **Windows PowerShell**, set it for this session and save it for your Windows account:

```powershell
$env:LINEAR_API_KEY = '<your key>'
[Environment]::SetEnvironmentVariable('LINEAR_API_KEY', $env:LINEAR_API_KEY, 'User')
```

The first line alone is session-only and is lost when the session ends. The second persists across
restarts for future apps. Fully restart existing terminal/editor apps to pick up the saved value;
the current shell can run `dydo map` immediately because of the first line.

On **bash/zsh**, set it for the current session:

```bash
export LINEAR_API_KEY='<your key>'
```

To persist it across restarts, also add that export line to your shell startup file. For Bash, use
`~/.bashrc` and ensure your login startup file (for example, `~/.bash_profile`) sources it. For Zsh,
use `~/.zshrc`, or `$ZDOTDIR/.zshrc` if `ZDOTDIR` is set. New interactive shells will load it; run
`dydo map` from a configured shell. Running `export` alone does not save it for later sessions.

The page reads `GET /api/teams`, `GET /api/projects?team=<teamId>` and `GET /api/graph?project=<projectId>`.
These routes read Linear afresh. Opening a Project also requests `GET /api/saved?project=<projectId>`:
its response is `{ "snapshot": null }` on a miss, or a snapshot with `graph` and UTC `fetchedAt`.
The viewer captures this baseline before starting its fresh request, so a fast fresh response cannot
overwrite the old snapshot before it is read. A miss or lookup failure also starts fresh fetching
immediately. Fresh fetching overlaps saved layout and the viewing window.
The saved canvas stays visible for at least two seconds after layout and initial viewport fitting,
then fresh changes use the normal Refresh animation. Its timestamp identifies saved data. A failed
fetch retains that map and timestamp, warns, and permits Refresh to retry. An unchanged graph settles
quietly; reduced motion keeps the viewing window without motion.

The server stores only the latest successful map for each opened Project under the OS user's local
application data `dydo/map` directory, outside repositories. Opaque endpoint/key partitions isolate
credentials and key rotation; corrupt or inaccessible files are treated as misses. No Linear writes,
polling, history, or prefetching unopened Projects occur. Delete this directory to forget saved maps.
`DYDO_MAP_CACHE_DIR` overrides the cache root for isolated tests.

 The viewer is embedded when `viewer/dist/` was built before dydo; a dydo built
without it serves a page at `/` saying the viewer was not built, and `dydo map --help` says so too.

`DYDO_LINEAR_ENDPOINT` replaces the Linear GraphQL URL. It is a test seam for pointing dydo map at a
fake Linear, not a user setting.

### dydo survey

Print a read-only digest of one Linear Project's lanes.

```bash
dydo survey 4e6aa9da-f325-4b53-a318-deba4b1b4d26                          # by Project UUID
dydo survey 25acbbb9a90b                                                  # by slugId
dydo survey https://linear.app/<workspace>/project/<name>-25acbbb9a90b    # by Project URL
```

`dydo survey` reads one Project once, with the user's `LINEAR_API_KEY`, and prints plain text. It
writes nothing, keeps nothing, and never polls: the caller decides when to run it
([DR 063](../project/decisions/063-dydo-survey-read-only-lane-digest.md)). Without a key it prints
the same key help as [dydo map](#dydo-map) and exits `2` before any request. It reads the Project's
non-archived issues, the issues linked from them, their `blocks` and `related` relations, each
issue's priority, and the Project body.

The ledger is the body section under the first `##` heading containing "ledger", in any case, up to
the next `##` heading, read as markdown. Its rows are table rows, with or without edge pipes, and
list items with their continuation lines and the prose under them, quotes included; a nested list
item or table row is a row of its own, and code holds no rows, no identifiers and no headings. A row
names every issue identifier in it, bare or inside a Linear link. Closed means a `completed`, `canceled` or
`duplicate` state; every other state, backlog and triage included, is open.

The first line is `dydo survey <Project name> <UTC ISO time>: <n> findings`. Each finding follows on
its own line, `<kind> <KEY> <title> — <reason>`, sorted by kind and then key:

| Kind | Finding |
|---|---|
| `no-ledger` | The body has no ledger section; its key is `-`. Every open issue is then `unlaned`. |
| `pickable` | An unstarted, unassigned Project issue with no open blocker, as the map draws it, and not already `unblocked`. |
| `question-no-priority` | An open Project issue labelled `Question` with no priority. |
| `stale-row` | A ledger row whose named issues in the read graph are all closed, unless it is a table row whose `State` cell, the column under a `State` header, is `done` (trimmed, any case, backticks allowed). A list item, or a row of a table with no `State` column, is always stale then. |
| `started-unassigned` | A started Project issue with no assignee. |
| `unblocked` | An open, unassigned issue in the Project or linked from it, with at least one `blocks` relation into it and every blocker closed. For a linked issue, only the blockers in the Project's graph are seen. |
| `uncontracted` | An unassigned Backlog Project issue with no open blocker, waiting to be contracted to `Todo`. |
| `unlaned` | An open Project issue that no ledger row names, nor its parent's. |

A successful read exits `0`, findings or not. An argument that is not one Project UUID, slugId or
Linear Project URL, and an unknown Project, are usage errors with exit `2`; the message names what
Linear lacked. An API or network
failure exits `2` with a message naming it. `DYDO_LINEAR_ENDPOINT` is the same test seam as for
`dydo map`.

---

## Mode Command

### dydo mode

Show or set this machine's mode flags.

```bash
dydo mode show                 # list every flag with its value
dydo mode set overnight on     # set one flag
```

The flags live in one file per user and machine, `dydo/mode.json` under the user's local application
data (`%LOCALAPPDATA%` on Windows, `~/.local/share` on Linux), so every project and agent on the machine
reads the same values. One flag ships: `overnight`, `on` or `off`, default `off`. `set` refuses an
unknown flag or value with exit `2` and changes nothing; on success it prints every flag as `show` does.

`dydo mode set` from an agent meets a built-in warn nudge that `dydo.json` cannot remove or weaken. The
first try stops with the reminder: run this only if you are the chief of staff readying overnight
mode or ending it; otherwise stop. On every host and direct guard invocation, the same command's
retry passes the warning. Its marker uses the existing command-warn key: pattern hash and full
command with whitespace runs normalized. A different command, including the opposite transition,
gets its own warning. Hard blocks run before warnings and preserve their markers. The guard reads the
command as the shell that runs it would, quotes, escapes, here-strings, heredocs and line
continuations included, and splits it only on separators outside quotes. Claude Code's `Bash` tool runs
bash and its `PowerShell` tool PowerShell; Codex, whose payload carries a `turn_id`, runs PowerShell on
Windows and the user's shell elsewhere. A call whose shell is unknown, such as `Bash` from an unmarked
hook on Windows, and any `cmd /c` string are read both as bash and as PowerShell or cmd would. A dydo call at command
position meets the nudge: past environment assignments and wrappers such as `env -i`, `sudo`, `nohup`,
`xargs` and the `&` call operator, as `dydo` under any path or case (`"C:/Program Files/dydo.exe"`,
`DYDO.EXE`), `dotnet ...dydo.dll` or `dotnet run --` with no `--project` or one naming dydo or DynaDocs. The command string of `cmd /c`,
`pwsh -Command`, `bash -c`, `eval` or `Invoke-Expression` and every `$(...)` or backtick substitution
is read with that shell's rules, and `Start-Process dydo -ArgumentList "mode", "set", ...` counts too,
as does an `@(...)` array or `(...)` group as the argument list. A PowerShell `<# ... #>` block comment is skipped. Arguments
of any other program are text, so a commit message, a PR body or a here-string that mentions
`dydo mode set`, even after a `;` inside the quotes, does not meet it. A command the guard cannot read,
such as one with an unbalanced quote or one piping text into a shell, meets it whenever it contains
dydo, mode and set in that order. The reminder applies equally to Claude, Codex, T3 and unknown
hosts, independent of permission mode or hook marker. It prevents accidents, without authenticating
the chief-of-staff role or proving human approval. After actual execution of either transition,
independently run `dydo mode show` and verify the intended value before announcing activation or
shutdown. A setter exit, intent, warning retry or schedule is insufficient. On failure or mismatched
readback, report the transition incomplete and the observed state; report unknown if unreadable.

---

## Related

- [CLI Commands Reference](./dydo-commands.md)
