---
explains: ["Commands/ArchiveCommand.cs", "Commands/CleanCommand.cs", "Services/Archive*.cs", "Services/Clean*.cs", "Services/Worktree*.cs"]
area: reference
type: reference
---

# CLI Commands: Archive and Clean

[CLI Commands Reference](./dydo-commands.md)

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

### dydo clean

Remove this repository's linked worktrees without losing work.

```bash
dydo clean --dry-run <worktree>...   # what would happen to the named worktrees; changes nothing
dydo clean <worktree>...             # remove the named worktrees, by path or branch name
dydo clean --all                     # remove every linked worktree it may remove
dydo clean --archive                 # list the archive of uncommitted work
```

Name the worktrees, by path or branch, and run from outside them; naming none, or naming some with
`--all`, is a usage error with exit `2`. Run `dydo clean --dry-run <worktree>` first, look at what would
be archived, and commit anything worth keeping, then `dydo clean <worktree>`. `--all` also reaches other
sessions' worktrees, so confirm no session is live before using it. Each worktree
gets one line: removed, archived and removed (with the archive entry), skipped (with the reason) or
failed (with the reason), followed by whether its local branch was deleted or kept; a skipped or
failed worktree always keeps its branch, and its line says so.

- **Never touched:** the main checkout, the worktree holding the current directory, a locked worktree,
  one a running process is using, and one under T3's worktree root `~/.t3/worktrees`, which T3
  owns. Each is skipped with its reason. So is a worktree whose `dydo.json`, at HEAD or
  in the tree, cannot be parsed, dry run included: which evidence it holds is unknown.
- **Submodules:** the archive reads only the worktree itself, so a worktree with an initialized
  submodule that has staged, unstaged or untracked changes, or a commit on any local ref (HEAD, a
  branch, a tag, the stash) that none of its remote refs reach, is skipped with a reason naming the
  submodule: commit and push it, or clean it, first. A worktree whose
  submodules are clean and pushed is removed as usual.
- **Uncommitted work is archived, then removed.** Staged, unstaged and untracked files git does not
  ignore, plus dydo's ignored evidence under `<root>/agents/` for every dydo root (the one
  `dydo.json` names at HEAD, the one the worktree's `dydo.json` names, and `dydo`), are copied to
  `<git common dir>/dydo/clean-archive/<UTC timestamp>-<branch>/`: `tracked.patch` (`git diff HEAD
  --binary`), `untracked/` and `evidence/` with their paths, and `manifest.json` (worktree, branch,
  HEAD, files, sizes, time, links). A link among those paths is never followed or copied: its
  `links` record keeps its path, whether it links a directory or a file, and its target. Other ignored
  files (`bin`, `obj`, `node_modules`, skill links) are regenerable and dropped, and so is build output
  under `<root>/agents/`: a file below a `bin`, `obj`, `node_modules` or `.vs` folder there, in any case,
  is not evidence, and the worktree's line counts the files skipped. A `WARNING:` on stderr lists every
  archived file, the count of build output skipped, and the archive path; there is no confirmation step. If the archive cannot be
  written, that worktree's line is `failed` and nothing is removed: no unlinking, no removal, no
  prune, and the branch is kept. When `<git common dir>/dydo` or `clean-archive` is a link, dydo does
  not follow it: no work is archived (so those worktrees fail), nothing is pruned or listed, the run
  prints `archive not pruned` with the reason and exits `2`.
- **Restore:** in a checkout of the entry's HEAD, `git apply <entry>/tracked.patch`, copy
  `untracked/` and `evidence/` back, and recreate each link in `manifest.json`'s `links` at its path,
  pointing to its target.
- **Retention:** every run except `--dry-run` and `--archive` prunes entries older than 90 days, then
  the oldest entries until the archive fits `clean.archiveSizeCapMb` in `dydo.json` (default 1024).
  Entries written by the same run are never pruned.
- **Branches:** after `git fetch --prune origin`, the local branch is deleted when its tip is
  reachable from `origin/<default>`, or when no changes are left to merge into it, as after a squash or
  rebase merge: `git merge-tree --write-tree origin/<default> <tip>` merges cleanly to exactly
  `origin/<default>`'s tree, so the branch holds nothing origin lacks. That merge runs in a scratch bare
  repository that borrows the real object store, in its object format (SHA-1 or SHA-256), and reads no repository, global or system config, no
  inherited `GIT_*` variable and no attributes, so no merge driver, builtin or configured, can resolve a change away, and even a dry run adds nothing
  to the repository. Otherwise the branch is kept and the line says why (not merged,
  no origin, fetch failed, or the squash check failed, as with a git older than 2.38). The default is the branch origin advertises now (`git ls-remote --symref origin HEAD`),
  so a stale local `origin/HEAD` never decides; only when origin cannot answer do the local
  `origin/HEAD`, then `origin/main` or `origin/master`, stand in. `git branch -d` runs first; it also
  wants the branch merged into the current checkout's HEAD, so when it refuses,
  `git update-ref -d refs/heads/<branch> <verified tip>` follows, which deletes the branch only while it
  still points at the tip found on origin: a branch that moved since is kept. Remote branches are never
  touched.
- **Removal:** every junction and symlink inside the tree is unlinked first, so nothing recursive can
  follow one out of the tree. Then `git worktree remove --force` (the archive already holds the dirty
  state); when that fails, as with paths past 260 characters on Windows, dydo deletes the folder
  itself through the extended-length path and runs `git worktree prune`.
- **Busy worktrees:** on Windows a worktree counts as in use when its folder cannot be renamed (a
  process has its working directory or an open file inside); on Linux when a process's working
  directory is inside it. macOS has no detection, so stop processes there first. If the Windows
  rename cannot be moved back, even after retries, that worktree's line is `failed`, naming where the
  folder is and the `Move-Item` that puts it back; dydo locks the worktree so `git worktree prune`
  keeps it meanwhile (unlock it after the move), and the run goes on.
- **`--dry-run`** prints the same lines and the same archive warning with `would`, and changes
  nothing: no fetch (branches are judged against the last fetch), no archive, no removal, no prune,
  no index rewrite (dydo runs git with optional locks off, so reads do not refresh a stat cache),
  and no busy check, since the Windows check renames the folder; each line says the busy check was not
  run.

Exit `0` when nothing failed; exit `2` when a removal failed, or when a named worktree was not
removed (unknown, main checkout, current directory, locked, submodule work, busy or under T3). With `--all`, skips
exit `0`.

## Related

- [CLI Commands Reference](./dydo-commands.md)
