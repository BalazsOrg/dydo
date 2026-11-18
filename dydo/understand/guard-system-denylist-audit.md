---
area: understand
type: reference
explains: ["Services/BashCommandAnalyzer*.cs"]
---

# Guard System: Denylist Audit

[Overview](./guard-system.md).

### David Ondrej denylist audit

DYD-188 compared [the upstream denylist](https://github.com/davidondrej/skills/blob/main/hooks/dangerous-patterns.txt) (blob `02e930aaf9879b879475baff5142f78e2df52f2a`) and its [test matrix](https://github.com/davidondrej/skills/blob/main/hooks/test-guard.sh) with the built-in analyzer. The following table records the disposition of each family. Command-shaped detections use upstream's anchor: the command name, optionally path-qualified (`/usr/bin/dd`), starts the line or follows whitespace or a shell separator (`;`, `&`, `|`, `(`, `)`, `{`, `}`, a backtick, `!`, `\`). So `sudo`, `nice`, `xargs`, `ssh host`, assignments such as `LC_ALL=C` and keywords such as `then` need no wrapper list. A quote starts a command only after a shell `-c` flag or an `ssh` remote command, so `echo 'dd ... of=/dev/loop0'` is allowed. `dd` writes to `sd*`/`nvme*`/`vd*`/`mmcblk*` targets still block anywhere in the command text, as before. A command named later inside quoted prose follows whitespace and is blocked, as upstream does: `echo 'sudo dd ... of=/dev/sda'` fails closed. The password-store rule (`pass` or `gopass` with a subcommand, a flag or one name) is the exception: its shapes are read on the text, then a `pass` or `gopass` must run at command position. A heredoc body, a Python `pass` statement, a quoted `grep PASS` pattern or a PR body saying "pass" is no access, while `pass show x`, `gopass show x` and `bash -c "echo; pass show x"` are. A line that cannot be read for certain (an unbalanced quote, a pipe into a shell) keeps the text's verdict. Every rule has a 250 ms match timeout; a command that exceeds it is blocked as too complex to verify. Option values and path targets limit false positives.

| Family | Disposition | Boundary and benign example |
|---|---|---|
| Root/home delete, fork bomb, shell download-and-execute | Already covered; extended `rm` for `/Users`, `--no-preserve-root` and `curl \| sudo zsh` | `rm -rf /tmp/build-cache`, `curl ... \| jq` allowed |
| Disk write/format | Extended `dd` writes to `/dev/*` except `/dev/null`, at any command boundary above; added `mkfs*` and destructive `diskutil` verbs | `dd ... of=backup.img`, `dd ... of=/dev/null` allowed |
| Privileged delete | Added `sudo` with optional flags, flag values or `--` followed by `rm` | `sudo brew services restart` allowed |
| Remote Git rewrite/delete | Added `git push -f`, `--force`, `--delete`, `-d`, `+ref`, `:ref` | `--force-with-lease`, normal push, dry run allowed |
| Git recovery removal | Added reflog expiry `now` and gc prune `now`/`all` | dated expiry and prune allowed |
| System permissions | Added `chmod 777 /` and `chown -R ... /` | changes to a named file or directory allowed |
| GitHub CLI mutation and tokens | Added repository/release/secret/key delete, API DELETE, public visibility and `gh auth token` | repo view, API GET/POST, auth status allowed |
| Credential stores | Added named password CLIs, sensitive `op` subcommands, macOS keychain password verbs, GPG/GPG2 secret export, `.password-store` paths and named password `.app` bundles at any path, plus app launch/removal | `op --version`, certificate lookup, public GPG export, unrelated apps and `brew`/`open` allowed |
| Generic `git reset` and `git clean` | Excluded: absent from upstream and local, recoverable operations outside this ticket's contract | Existing path guard still evaluates extracted operations |

The upstream POSIX expressions are translated to .NET regexes rather than copied verbatim. The hook evaluates these before nudges or path analysis, so a matching command blocks for every caller even when it names no local file.

---
