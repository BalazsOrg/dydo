---
explains: ["Commands/PrReadyCommand.cs", "Services/PrReadyCheck.cs", "Services/ExplainsCheck.cs", "Commands/MergeCommand.cs", "Services/Merge/**"]
area: reference
type: reference
---

# CLI Commands: PR Readiness and Merge

[CLI Commands Reference](./dydo-commands.md)

### dydo pr-ready

Check that a tree is fit to open or ready a PR from. A PR shows the branch as the remote holds it,
so uncommitted or unpushed work is not in it.

```bash
dydo pr-ready                          # check the current directory's tree
dydo pr-ready <path>                   # check another tree
dydo pr-ready --body dydo/agents/workspace/pr-body-<KEY>.md   # also check a PR body
```

`--base <ref>` selects the merge target for warnings about changed `explains:` paths
whose explanatory doc was not updated. It defaults to local `origin/HEAD`; nothing is
fetched. Warnings name the doc and paths and do not affect readiness.

The tree must have no staged, unstaged or untracked files git does not ignore, and HEAD must have
an upstream and equal it. The upstream is read from the local remote-tracking ref; nothing is
fetched. Keep the body file in `dydo/agents/workspace/`, which git ignores, so it is not an untracked file
and the guard can read it from inside the tree. With `--body`, the body must name a ticket key (such as `DYD-123`) and hold a
`## Independent review` section with a parsable reviewer block, fenced or not:
`PASS - rubric/type: code; candidate <SHA>; base <SHA>`. The rubric must be `code`, `docs` or
`merge`; the candidate SHA must contain 7–40 hexadecimal digits and may be backticked. Fields start
after semicolons. `candidate: <SHA>` and `base: <SHA>` are also accepted, independently;
the canonical forms without colons remain valid. The dash after `PASS` may be a hyphen,
an en dash or an em dash. Prose-only PASS lines and missing or invalid candidate fields are refused
with the expected form. Base is optional: as in merge, an absent or unparsed base does not
invalidate the PASS. A parsed base uses the same SHA form. This uses the merge parser but checks grammar only: it does not
resolve review SHAs, match the candidate to HEAD or check base ancestry. Merge still applies
its own review, pin, ancestry and check policy.
Each problem prints as one `not ready:` line on stderr and the exit is `1`; a path outside a git
work tree is a problem too. When all hold, it prints one `ready:` line and exits `0`. The guard
runs the tree checks on `gh pr create`, `gh pr new`, `gh pr ready` and their `gh api` forms
([Guard System](../understand/guard-system.md)).

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
  or not, a hyphen, en dash or em dash after `PASS`, the SHA possibly backticked. The `candidate`
  and `base` field names accept an optional colon before the whitespace and SHA. An abbreviated
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
refused for a Merge implementer to resolve locally.

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

`dydo init` and `--join` write narrow allow rules for both hosts (see [dydo init](./dydo-commands-core.md#dydo-init)),
and the guard lets a sub-agent run it: a Merge Sub-ticket's implementer runs it. The guard hard blocks
the other merge routes for every caller: raw `gh pr merge`, `gh api` and GraphQL merge calls, a
`git push` to the default branch and default-branch changes
([merge-route blocks](./../understand/guard-system-enforcement.md#also-enforced)); each refusal names `dydo merge`.
The Claude bash/Codex PowerShell parity gate is planned, pending in DYD-426; writing these rules or
shipping the blocks does not prove host parity.

---

## Related

- [CLI Commands Reference](./dydo-commands.md)
