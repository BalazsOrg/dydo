---
area: reference
type: reference
---

# CLI Commands: Clean, PR Readiness, Merge and Validation

[Overview](./dydo-commands.md).

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

### dydo pr-ready

`--base <merge-target>` compares explained code against that merge base (default: local
`origin/HEAD`). Warnings name unchanged docs and touched paths; update the doc or state it
still holds. A warning does not block readiness. Nothing is fetched.

Check that a tree is fit to open or ready a PR from. A PR shows the branch as the remote holds it,
so uncommitted or unpushed work is not in it.

```bash
dydo pr-ready                          # check the current directory's tree
dydo pr-ready <path>                   # check another tree
dydo pr-ready --body dydo/agents/workspace/pr-body-<KEY>.md   # also check a PR body
```

The tree must have no staged, unstaged or untracked files git does not ignore, and HEAD must have
an upstream and equal it. The upstream is read from the local remote-tracking ref; nothing is
fetched. Keep the body file in `dydo/agents/workspace/`, which git ignores, so it is not an untracked file
and the guard can read it from inside the tree. With `--body`, the body must name a ticket key (such as `DYD-123`) and hold a
`## Independent review` section with the reviewer's `PASS - rubric/type: ...` block, fenced or not;
the dash after `PASS` may be a hyphen, an en dash or an em dash.
Each problem prints as one `not ready:` line on stderr and the exit is `1`; a path outside a git
work tree is a problem too. When all hold, it prints one `ready:` line and exits `0`. The guard
runs the tree checks on `gh pr create`, `gh pr new`, `gh pr ready` and their `gh api` forms ([dydo guard](./dydo-commands.md#dydo-guard)).

### dydo merge

Merge a reviewed PR into its `feature/*` branch with a merge commit. A merge into `master`, `main` or
the default branch is the user's click: the command refuses it, and no option overrides a refusal.

```bash
dydo merge <pr> --match-head-commit <sha> --match-base-commit <tip> --dry-run   # every read and check; merges nothing
dydo merge <pr> --match-head-commit <sha> --match-base-commit <tip>             # merge exactly <sha> into the PR's feature/* base at <tip>
```

`--match-head-commit` takes the full 40-character SHA the reviewer passed: the PR head must equal it,
and it is what merges. `--match-base-commit` takes the full 40-character SHA of the target branch tip
you pinned; read it from the branch ref (`git ls-remote origin refs/heads/<base>`), never from the
PR's `baseRefOid` or `base.sha`, which can name a tip the branch has since left. Both are required
and validated before any network call, the head pin first; a missing, short or non-hex value prints
`refused: --match-base-commit takes the full 40-character SHA of the target branch tip you pinned`.
There is no repository, base-selection or override option: the target is always the PR's base, and
the base pin only adds a refusal. In order, it checks:

- **Repository:** the one the `origin` remote of the located project's checkout names, as
  `HOST/OWNER/REPO`; the checkout's git top level must be the `dydo.json` folder. git runs without
  inherited `GIT_*` variables, and gh runs as `gh api` with `--hostname` set to origin's host and
  `GH_REPO` and `GH_HOST` removed. The PR must be in that repository.
- **A closed PR first:** when its base is a feature branch (as below) that already holds the SHA, the
  run prints `already integrated` and exits `0`, so a rerun after a lost answer is safe; any other
  closed PR is refused.
- **The PR:** not a draft, and from the same repository, not a fork.
- **Base:** refused when it is GitHub's `default_branch`, read fresh on every run, or the local
  `origin/HEAD` name, whatever the case; it must be `feature/` and one or more `/`-separated
  segments, each starting with a letter or digit (`[A-Za-z0-9][A-Za-z0-9._-]*`), and pass
  `git check-ref-format --branch`. An unreadable default branch is refused.
- **Head:** the PR head must equal the SHA.
- **Target tip:** the base's tip, read from `GET repos/<owner>/<repo>/branches/<base>`, must equal
  the pin, dry run or real; otherwise
  `refused: <base> is at <tip12>, not the pinned <pin12>; nothing merged - re-pin the target and re-contract`.
  A closed PR's reconcile compares no pin, since the run's own merge moved the tip.
- **Review:** the PR body's `## Independent review` section, up to the next level-one or level-two
  heading, must hold a `PASS - rubric/type: <rubric>; ...` line whose `candidate` is the SHA: fenced
  or not, a hyphen, en dash or em dash after `PASS`, the SHA possibly backticked. An abbreviated
  candidate of at least 7 hex digits counts only when GitHub resolves it to exactly the SHA. The
  rubric must be `code`, `docs` or `merge`, and a `FAIL` line for the same candidate refuses. When the
  PASS names a `base`, the target's tip must contain it; when it names none, a `note:` line says the
  base check was skipped.
- **Checks:** every check suite, latest check run and legacy commit status on the SHA is read to
  the last page. The fixed in-code inert-app list, matched by GitHub App slug, contains only
  `blacksmith-sh`. A listed app's suite is ignored only when its status is `queued`, `in_progress`,
  `pending`, `requested` or `waiting` and `latest_check_runs_count` is present as a valid integer
  zero. Unknown, missing or malformed status and missing or invalid count never establish the
  exception, even for `blacksmith-sh`. Every non-exempt suite is judged normally; any unfinished
  suite then refuses, whatever its run count. Unknown states always refuse.
  Suites and runs must finish with `success`,
  `neutral` or `skipped`; legacy statuses must be `success`. Every check the target's branch
  protection or rulesets require must be present. A suite refusal names the app slug and status,
  for example `refused: checks on <sha12> are not green: circleci-checks queued`.
  With no suite left after the exception, the command refuses `no checks reported`.

Then it merges: `POST repos/<owner>/<repo>/merges` names the validated base and the SHA, so a later
retarget or push cannot change where or what lands. GitHub cannot make that call conditional on a
tip, so the command reads the branch ref again just before it, refusing as above on a mismatch, and
afterwards checks that the merge commit's first parent is the pin. When the pin is an ancestor of
the SHA, where GitHub would fast-forward, it writes a two-parent commit (the SHA's tree, parents the
pin and the SHA) and moves the base atomically, only if it is still at the pin: GraphQL `updateRefs`
with `beforeOid` the pin, without force. A base that moved meanwhile, even to a commit in the SHA's
history, fails that move; the command then reads the branch ref again and, when the base is off the
pin, refuses with nothing merged. A base still at the pin is a tool error with GitHub's message. There
is no squash or rebase. The commit message is `Merge pull request #<n> from <owner>/<head-ref>`, the
PR title, and the trailers `Dydo-Merge: dydo <version>`, `Reviewed-Candidate: <sha>`,
`Target-Tip: <pin>` and `Review: <the PASS line, first 200 characters>`. A conflict (`409`) is
refused for a Merge code-writer to resolve locally.

Output on stdout, the captain's merge receipt:
`merged PR #<n> (<sha12>) into <base> at <pin12> as <merge12>; PR marked merged: yes|pending|unverified`,
or `already integrated (no new merge commit): PR #<n> (<sha12>) is in <base>`, or for a dry run
`would merge <sha12> into <base> at <pin12> (PR #<n>)`. A `warning:` line follows when the default branch changed
during the run. There is no local log; git and the PR timeline are the record.

Exit `0` for a merge, a SHA already integrated, and a `--dry-run` whose checks pass. Each refusal
prints one `refused:` line on stderr and exits `1`; a missing PASS adds a `found PASS for <sha12>`
line for each other candidate the review section passed. A tool error prints one `error:` line and exits
`2`: gh missing or signed out, the network, an origin on an SSH alias host, or an unexpected GitHub
answer such as a `403`, `422` or ruleset rejection, with GitHub's message. A merge that landed on
another tip, when the branch moved between the re-read and GitHub's merge, also exits `2`: its
`error:` line names the merge, its first parent and the pin, and says the merge landed and its actual
parents need merge review. A merge that succeeded exits `0` even when the PR could not be reread
(`unverified`) or GitHub had not yet marked it merged (`pending`: the command rereads a few times over
about four seconds, and never prints `no` after a successful merge).

`dydo init` and `--join` write narrow allow rules for both hosts (see [dydo init](./dydo-commands.md#dydo-init)),
and the guard lets a sub-agent run it: a Merge Sub-ticket's code-writer runs it. The guard hard blocks
the other merge routes for every caller: raw `gh pr merge`, `gh api` and GraphQL merge calls, a
`git push` to the default branch and default-branch changes
([dydo guard](./dydo-commands.md#dydo-guard)); each refusal names `dydo merge`.
The Claude bash/Codex PowerShell parity gate is planned, pending in DYD-426; writing these rules or
shipping the blocks does not prove host parity.

---

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
`dydo guard --host claude`, the one [`dydo init`](./dydo-commands.md#dydo-init) writes, whatever its matcher. A guard
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
