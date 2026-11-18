---
area: reference
type: reference
explains: ["Commands/ModeCommand.cs", "Commands/ArchiveCommand*.cs", "Services/Mode*.cs", "Services/Archive*.cs", "Services/SafeDelete*.cs"]
---

# CLI Commands: Mode and Archive

[Overview](./dydo-commands.md).

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

## Worktree Commands

### dydo archive

Archive verified recovery data before deleting a branch or linked worktree:

```bash
dydo archive branch <name> [--remote <remote>] [--dry-run]
dydo archive worktree <path> [--remote <remote>] [--dry-run]
dydo archive list
dydo archive restore <id> [--branch <new-name>] [--path <new-path>] [--remote]
```

The selected upstream chooses only the remote. Only the local branch's **same-name** remote ref is
targeted; a differently named integration upstream is reported as kept. With no upstream, select
origin, the sole remote, or require `--remote` when ambiguous. Local repositories need no remote.
The remote's live default, `feature/*` branches and open PR heads/bases are protected. The accepted
`gh` dependency must establish complete PR eligibility; offline, failed or unsupported lookups retain
the local target. Public GitHub repositories use the exact remote ref's complete open-PR head
associations, including heads used in upstream and other forks, plus the selected repository's
complete open-PR head/base associations. Private repositories are eligible only under closed-world
authority: the caller's token must read `fork=false`, `forks_count=0` and consistent private
visibility/identity. Internal visibility, private forks or private repositories with forks,
and incomplete/error responses refuse. An initially absent same-name remote ref is eligible only
in a public or private closed-world repository (`fork=false`, `forks_count=0`), read with the
caller's token. Complete own-repository open-PR head/base listings must prove no PR uses the name,
even when its head ref was deleted. The command performs no push and archives/cleans up locally;
all other unproved initial absence refuses. Search indexing supplies no authority.
Queries are bounded to 30 seconds, 8 MiB of response data and 100 pages per connection;
exhaustion refuses. Identity, visibility/fork metadata, OID and PR eligibility are rechecked before
each destructive step; metadata is also reread after pagination immediately before the leased push.

After this invocation's successful expected-OID leased deletion, its internal receipt carries the
verified pre-push head check into the remaining worktree/local steps. Initially absent refs require
closed-world authority at every check. Both routes reread repository metadata, exact ref absence
and complete current own-repository
head/base associations. A recreated remote ref (even at the old OID), changed metadata or lookup
failure stops the remaining deletions and retains the complete archive. The command never deletes
that recreated ref. A PR can still open between the final API read and push without moving the OID;
the lease does not make PR state and Git deletion atomic. Remote history is fetched
into a temporary ref before an eligible real archive. Dry-run
fetches nothing and labels missing remote-object estimates unknown/lower-bound.

Entries live in `<absolute git common dir>/dydo/safe-delete-archive/<id>/`, shared by linked
worktrees. A versioned manifest, nonempty self-contained history bundle, all captured tip IDs/OIDs,
reflog/per-worktree metadata and payload hashes remain on this machine. Worktree entries additionally
contain every file (ignored and untracked included), empty directories, link/permission metadata,
the original index and a separate pack containing its referenced objects. No commit is made.
Partially staged and working versions remain distinct. The command imports into a fresh object
store without alternates or inherited Git environment, reconstructs files/index/status and verifies
the result before publishing a complete archive or deleting anything.
Saved Git file-mode, case, symlink and line-ending settings are installed in the independent proof
repository and, where enabled, the restored worktree's `config.worktree`. Verification uses Git
status without configuration overrides. Restore never changes shared or global Git settings; if
worktree configuration is disabled and inherited settings differ, restore refuses with a diagnostic.

Deletion uses an expected-OID remote lease, then removes the worktree, then deletes its local branch
with compare-and-swap. Final snapshot/ref/activity rechecks detect concurrent changes. Busy probes
support Linux and Windows; Linux observes readable process working directories, while Windows
probes folder renaming. These checks leave a small race with arbitrary external writers. Unsupported
platforms refuse worktree removal. A stranded Windows probe locks the registration and reports its
recovery path. Failed later steps keep the verified archive, report what remains and exit 2; they
cannot promise rollback of an earlier remote deletion.

Main/current/locked/missing/in-use worktrees, checked-out branches and authoritative defaults are
protected. In-progress Git operations, shallow/partial/borrowed history, LFS/gitlink dependencies,
initialized nested repositories and unsupported file metadata refuse before deletion. Unix symbolic
links are recorded without traversal; special files, hard links, extended metadata and foreign
ownership refuse. Unsupported Windows reparse points, alternate streams, hard links and filesystem
metadata also refuse. Files retain Unix modes or Windows security descriptors. Worktree restoration
requires the original platform's metadata support.

T3 targets are recognized under default, configured and previous roots and the `/.t3/worktrees/`
path form. Unreadable authoritative settings refuse worktree removal. The production T3 policy
retains recognized targets: a supported route that authoritatively checks every sharing thread,
provider session and terminal under a workspace lease has not passed native disposable-host proof.
Thread list/status/handoff alone cannot establish safety. T3's later automatic worktree recreation
does not restore archived work. The captain's native T3 gate owns enabling a safe route.

Restore is local by default and never overwrites a branch, path or conflicting recovery ref.
The primary branch uses the original local tip (or remote-only tip); a worktree uses its saved HEAD
and stays detached unless `--branch` attaches it. Every captured tip is restored under
`refs/dydo-restored/<archive-id>/<tip-id>` and is listed with its bundle/ref mapping. Identical
existing recovery refs are accepted. `--branch` renames only the primary branch. Explicit `--remote`
republishes the original same-name remote tip, even when it diverged from the local tip, using a
create-only lease; an existing remote ref refuses. Archives remain after restoration and failures.

Retention is 30 calendar days, with explicit UTC creation/expiry timestamps. Only validated actual
`archive branch`/`archive worktree` invocations purge complete entries strictly past expiry;
list, restore, help, dry-run and inactivity never purge. No size eviction or background job exists.
Malformed/incomplete entries are retained with diagnostics. Linked containers, corrupt/empty
payloads and traversal paths fail closed. Space requirements and defaults are documented in
[configuration](./configuration.md#fields).

The guard and generated Codex prefix rules admit archive/list/restore for ordinary agents and
subagents. Raw forced branch deletion, heads deletion through update-ref, push deletion/refspecs,
prune/mirror pushes and every direct worktree removal remain blocked. Nonforced `git branch -d`
remains compatible. These rules prevent accidents; they do not constrain a rogue process.

### dydo archive branch

Archive and delete the named local branch and its selected same-name remote ref.

- `--remote <remote>` selects a configured remote explicitly.
- `--dry-run` prints intent and archive/proof estimates without fetching, archiving or purging.

### dydo archive worktree

Archive every file and index version before removing one registered linked worktree and its branch.

- `--remote <remote>` selects a configured remote for the attached branch.
- `--dry-run` prints intent and estimates without fetching or changing the target.

### dydo archive list

List repository-local archive identities, expiry, completeness, deletion progress and recoverable tips.
Listing never purges entries.

### dydo archive restore

Restore a verified archive locally without overwriting existing refs or directories.

- `--branch <new-name>` renames only the primary branch, or attaches an archived detached HEAD.
- `--path <new-path>` reconstructs into a new worktree location.
- `--remote` explicitly recreates only the absent original same-name remote ref with a create-only lease.
