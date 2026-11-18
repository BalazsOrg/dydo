---
area: reference
type: reference
explains: ["Commands/InitCommand.cs", "Commands/AdoptCommand.cs", "Commands/Check*.cs", "Commands/FixCommand.cs", "Commands/IndexCommand.cs", "Commands/Graph*.cs", "Commands/GuardCommand*.cs", "Commands/MapCommand.cs", "Commands/SurveyCommand.cs", "Services/Doc*.cs", "Services/Map/**", "Services/Survey/**", "Rules/**", "Services/Explains*.cs", "Services/FolderScaffolder.cs", "Services/ScaffoldTree.cs"]
---

# CLI Commands: Setup, Documentation, Guard and Linear

[Overview](./dydo-commands.md).

## Setup Commands

### dydo init

Create the project's durable knowledge tree and wire a runtime's guard hook.

```bash
dydo init <integration>              # claude, codex, all, or none
dydo init <integration> --join       # wire this machine, or an added runtime, into an existing project
```

Every mode writes `dydo.json`, scaffolds the `dydo/` folders with their framework documents,
`files-off-limits.md` and `_system/types.json`, writes the `AGENTS.md` entry point (never `CLAUDE.md`), and adds
`dydo/agents/`, `dydo/_system/.local/`, `/.claude/skills/` and `/.agents/skills/` to `.gitignore`. A
documentation or entry-point file that already exists is left as it is. A fresh init records the
running dydo version in `dydo.json` as `dydoVersion`. `none` stops there: no runtime integration.
The runtime modes add, per selected host:

| Host | Files | What they carry |
|---|---|---|
| `claude` | `.claude/settings.json` (committed) | a `PreToolUse` hook running `dydo guard --host claude` on the matched tools, `env.CLAUDE_CODE_MAX_SUBAGENT_SPAWN_DEPTH = "3"` and `"autoMemoryEnabled": false` |
| `claude` | `.claude/settings.local.json` (gitignored) | the allow entries `Bash(dydo:*)`, `PowerShell(dydo:*)`, `Bash(dydo clean:*)`, `PowerShell(dydo clean:*)`, `Bash(dydo merge:*)` and `PowerShell(dydo merge:*)` |
| `claude` | `.gitignore` | `.claude/settings.local.json`, `/.claude/skills/` and `/.agents/skills/` |
| `codex` | `.codex/hooks.json` | a `PreToolUse` hook running `dydo guard` on the matched tools |
| `codex` | `.codex/rules/dydo.rules` | exec-policy `prefix_rule`s allowing `dydo archive`, `dydo clean`, and `dydo merge` under `dydo` or `dydo.exe`, which Codex loads when the project's `.codex/` layer is trusted; dydo rewrites the file on every init and `--join` |
| `codex` | `.codex/config.toml` | `[agents]` with `max_depth = 3` and `max_concurrent_threads_per_session = 16` |
| `codex` | `.gitignore` | `/.claude/skills/` and `/.agents/skills/` |

`all` selects both hosts. Only tool calls the hook's matcher names reach `dydo guard`. The guard
hook is committed for both hosts, so a fresh clone is guarded; `settings.local.json` holds only
this machine's allow list ([DR 056](../project/decisions/056-claude-guard-in-committed-settings.md)).
Init keeps one dydo guard entry where dydo's earlier one stood, removes other dydo guard entries,
a Stop entry running `dydo guard --stop` and any dydo hook in `settings.local.json`, and leaves
hooks it did not write alone. A JSON file init or `adopt` edits keeps its text: unchanged values
keep their bytes, key order and unknown keys, the file keeps its BOM, line endings and trailing
newline, and a file with nothing to change is not written. When
`.claude/settings.json` or `.codex/config.toml` already holds a conflicting value for a key init
manages, init stops with an error before it writes anything. Init and `--join` also append a
`dydo completions` line to the shell profile they detect, when they detect one.

`--join` targets an already-initialized project: a fresh clone, or a second runtime added later. It
writes the selected hosts' rows above, and the entry point when it is missing, without re-scaffolding
or overwriting the tree, and records the integration in `dydo.json`. It leaves `dydoVersion` alone and
never adds it: a teammate's newer CLI has not adopted that release's skills and documents. `dydo adopt`
records it.

Claude Code 2.1.281 or later reads `AGENTS.md` when no `CLAUDE.md` is in the way. With `claude` or
`all`, init and `--join` therefore remove a `CLAUDE.md` that is dydo's own generated copy (equal to
`AGENTS.md` or the template). A `CLAUDE.md` you wrote is kept, with a warning that it hides
`AGENTS.md`; move its content into `AGENTS.md`, or put `@AGENTS.md` at its top. `CLAUDE.local.md` and `.claude/CLAUDE.md` are never removed, and each gets its own warning:
`CLAUDE.local.md` needs `@AGENTS.md` at its top (or Project instructions set to
`claude-md-and-agents-md`); `.claude/CLAUDE.md` needs `@../AGENTS.md` at its top.

dydo does not compile or install skills, and none ship with the package. A skill is one folder under
`skills/<category>/<name>/`: copy `skills/`, `setup-skills.mjs` and `THIRD-PARTY-NOTICES.md` (the
MIT notices of the adapted skills travel with them) from the dydo repository into the project root,
leaving out each skill's test tree, any `test` or `tests` folder inside a skill folder, which dydo
keeps and runs in its repository. With the repository cloned outside the project, run from the
project root, `<tag>` being the release to install:

```bash
git clone https://github.com/bodnarbalazs/dydo <clone>
git -C <clone> archive -o dydo-skills.tar <tag> -- skills setup-skills.mjs THIRD-PARTY-NOTICES.md ":(exclude,glob)skills/**/test/**" ":(exclude,glob)skills/**/tests/**"
tar -xf <clone>/dydo-skills.tar
```

A skill only the project has keeps its own tests. Commit the files and run `node setup-skills.mjs`
to create the host discovery links. The script always
creates both `.claude/skills/` and `.agents/skills/`, and init always gitignores both (the
`.gitignore` rows above), in every mode including `none`.

### dydo adopt

Record the dydo release whose skills and documents the project has adopted.

```bash
dydo adopt 3.5.0                     # or v3.5.0; the leading v is dropped
```

Sets `dydoVersion` in `dydo.json`, rewriting the file the way `--join` does. The `update-dydo` skill
runs it once an update is proven. The value is a release like `3.5.0` or `3.5.0-beta.3`. Anything else, a missing
`dydo.json`, or one that does not parse, exits `2` and leaves the file as it was.

---

## Documentation Commands

### dydo check

`--base <merge-target>` adds advisory explained-code warnings to declaration validation.

Validate documentation naming, frontmatter, titles, links, and project-specific rules. It also validates
the off-limits file, legacy FutureFeature shape under `project/future-features/`, retired v2 work records,
uncustomized foundation docs (warning), and `dydo.json` itself; config errors count toward exit `1`.

```bash
dydo check
dydo check <path>
dydo check --root <dir>          # check the project containing <dir>, not the current one
dydo check --root <dir> <path>   # <path>: absolute or cwd-relative (not root-relative), inside that docs tree
```

Exit `0` means no errors. Exit `1` means validation errors were found. Exit `2` means a tool error,
such as a `<path>` outside the docs tree or a `--root` in no dydo project.

### dydo fix

Apply supported documentation repairs.

```bash
dydo fix
dydo fix <path>
```

Repairs include filename normalization, wikilink conversion, and restoration of
required scan exclusions. Review the Git diff afterward.

### dydo index

Regenerate documentation indexes from the configured structure.

```bash
dydo index
dydo index <path>
```

### dydo graph

Show incoming and outgoing documentation links for one file.

```bash
dydo graph <file>
dydo graph <file> --incoming
dydo graph <file> --degree 2
dydo graph <file> --root <dir>
```

### dydo graph stats

Show repository-wide documentation graph statistics.

```bash
dydo graph stats
dydo graph stats --top 20
dydo graph stats --root <dir>
```

---

## Guard Command

### dydo guard

Evaluate one tool call against the two path tiers, the dangerous-command rules, and the project's
nudges. Runtime hooks invoke this command automatically; the argument form is for diagnostics.

```bash
# Hook mode
echo '{"session_id":"manual","tool_name":"Edit","tool_input":{"file_path":"src/file.cs"}}' | dydo guard

# Diagnostic mode
dydo guard --action edit --path src/file.cs
dydo guard --command "git status"
```

`--host claude` marks the Claude Code hook `dydo init` writes; it is not a diagnostic argument, so
the call still reads its payload from stdin. The Codex hook carries no marker. An argument starting
with `@`, such as a PowerShell here-string `--command "@'…'@"`, is text and gets a verdict; the guard
never reads it as a response file.

In hook mode (a PreToolUse payload on stdin, no arguments) exit `0` with empty stdout allows the
action, and exit `0` with one `permissionDecision: "deny"` object on stdout blocks it; the reason
carries the `BLOCKED:` text, which still goes to stderr too. See [Guard System](../understand/guard-system.md)
for why a block is not exit `2`. In diagnostic mode (the CLI-argument examples above) exit `0`
allows and exit `2` blocks with `BLOCKED:` on stderr. **Off-limits** paths block
every operation, reads included. **Protected** paths are readable by any tool and writable by none,
Bash included. Both tiers bind on every caller; [Files Off-Limits](../files-off-limits.md) declares
them, their glob syntax, and the whitelist that lifts off-limits patterns. Nudges are configured in
`dydo.json` - see [DynaDocs](./about-dynadocs.md). A nudge matches shell command lines by default; one
with `"match": "new-file"` matches the project-relative path of a file that Write, Edit, NotebookEdit
or a Codex `apply_patch` is about to create, and never fires for a file that already exists or for a
file a shell command creates.
A plain `gh pr create`, `gh pr new` or `gh pr ready` is blocked while the tree fails the tree checks
of [dydo pr-ready](./dydo-commands.md#dydo-pr-ready).

The merge-route hard blocks apply to every caller, top-level or sub-agent. They refuse raw
`gh pr merge` into any base, `gh api` REST and GraphQL merge calls, and `git push` updates to
`master`, `main` or the branch the local `origin/HEAD` names. They also refuse default-branch
changes through `gh repo edit --default-branch` or a `gh api` PATCH of the repository with a
`default_branch` field. Each refusal directs the caller to [dydo merge](./dydo-commands.md#dydo-merge) for reviewed
feature-branch integration; the default branch remains the user's. These text checks stop
accidents, not rogue agents.

---

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
the same key help as [dydo map](./dydo-commands.md#dydo-map) and exits `2` before any request. It reads the Project's
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
