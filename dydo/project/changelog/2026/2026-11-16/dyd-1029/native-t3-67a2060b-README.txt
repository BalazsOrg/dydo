# DYD-1029 native T3 retained-target proof

Result: PASS for native refusal while the owned child is active, retained-target integrity,
and subsequent T3 status/worktree-list/thread-list/thread-read health.
The named T3_RELEASE_UNPROVEN finding remains. No T3 release/removal path was enabled or tested.

## Identity

- Source HEAD and fixture branch OID: `ab242bdb33d85c521e5ea1a852de00007bf0ec0d`.
- Branch: `t3/prove-native-t3-refusal`.
- Exact target: `/home/balazs/.t3/worktrees/DynaDocs/t3-c0bbb510-5de29838258bd706`.
- Thread: [DYD-1029 native T3 retained-target proof](t3-thread://v2/f7f7b47f-c6f2-4990-a113-851014c04ff0/thread:delegated-task:command%3Amcp%3A596543af-f8e1-4636-ad79-8d0fc638da6f%3Adelegate-task%3A67a2060b-d11b-457f-83d2-27cec1eb4cd6).
- Active run: `run:thread:thread%3Adelegated-task%3Acommand%253Amcp%253A596543af-f8e1-4636-ad79-8d0fc638da6f%253Adelegate-task%253A67a2060b-d11b-457f-83d2-27cec1eb4cd6:ordinal:1`.
- Before and after native invocation: running, `settled=false`, same activeRunId and worktreePath.
- Own DLL: `artifacts/bin/DynaDocs/release/dydo.dll`.
- DLL SHA256: `0be33d13a59324798d901412d2784e4150f37f9371d63969866572bc3da796c7`.

## Observed native command

The launcher ran from `/tmp`; the actual CLI cwd was the owned directory
`/tmp/dyd1029-native-t3-67a2060b/repository-context`, outside the target.
This directory contains only an unregistered `.git` discovery file pointing at the existing
common repository. It does not create a worktree, change the target, or inject a T3 policy.
The CLI has no repository-path option and strips inherited GIT_* variables.

```
dotnet /tmp/dyd1029-native-t3-67a2060b/artifacts/bin/DynaDocs/release/dydo.dll archive worktree /home/balazs/.t3/worktrees/DynaDocs/t3-c0bbb510-5de29838258bd706
```

Native exit: **2**. Exact stdout is in `native-refusal.stdout`:

```
archive: T3 ownership/activity unavailable: no supported release route with all sharing threads, provider sessions, terminals and workspace lease has passed native disposable-host proof; target retained; archives retained; inspect reported deletion progress before retrying
```

`native.strace` and `native-executed-processes.txt` retain all 11 exec attempts, including
interleaved unfinished execve lines. Local `git remote` and upstream configuration reads
precede the T3 refusal. No network remote lookup (`ls-remote`), fetch, GitHub query,
destructive Git subprocess, or archive-root access occurred.

## State and health assertions

- `before-snapshot.json` and `after-snapshot.json` are equal: all 2,478 tracked paths,
  file bytes/modes/link targets, target branch OID, HEAD/index/gitfile and exact Git registration.
- The owned untracked binary sentinel was byte-for-byte unchanged after refusal.
- Archive child listings were unchanged; no archive entry was created.
- `t3-pre-invoke.json`, `t3-after.json` and `t3-worktree-list-after.json` retain only relevant
  structured metadata. Status/list/read calls succeeded and continued to identify the fixture.
- `cleanup.json` proves only the owned sentinel was removed. Final Git status is clean;
  the T3 checkout remains intact at the requested SHA.

## Recipe and containment

`runner.py` is an orchestrator; it invokes the existing shared `/var/tmp/dydo-gate-lock.sh`
outside each capped systemd scope. All native proof/state phases use the nearest session slice
`app-t3code-agents-cbc6c2286251.slice`, MemoryMax=4G, MemorySwapMax=0,
OOMPolicy=continue, timeout 120. Build uses 8G/3600. The phase `*-containment.json`
files read back actual cgroup memory and swap limits. Compiler-server disabling variables
are recorded in each phase command/result receipt.

Commands actually run, in order:

```
python3 /tmp/dyd1029-native-t3-67a2060b/runner.py inspect
python3 /tmp/dyd1029-native-t3-67a2060b/runner.py build
python3 /tmp/dyd1029-native-t3-67a2060b/runner.py before
python3 /tmp/dyd1029-native-t3-67a2060b/runner.py invoke
python3 /tmp/dyd1029-native-t3-67a2060b/runner.py after
python3 /tmp/dyd1029-native-t3-67a2060b/runner.py cleanup
python3 /tmp/dyd1029-native-t3-67a2060b/runner.py audit
```

T3 metadata checks bracketed the invocation; the T3 worktree-list call followed cleanup.
The audit was repeated after correcting its initial parsing of interleaved strace exec lines;
the final audit includes every exec attempt. This correction did not rerun the native command.
Every inner command has argv/cwd/exit plus raw stdout/stderr. The candidate build and native
invocation have separate raw receipts. Source bytes and their tracked identity are retained
under `source/` and `source-identity.json`.

The sandbox initially refused the user bus; capped operations then used escalation.
No uncapped candidate run occurred. Bootstrap read-only discovery/cgroup/skill reads preceded
containment setup; see `audit.json` for this procedural limitation.

## Exact limitations

- No inactive T3 deletion is proven.
- No archive or source-independent restore of a T3 target is proven.
- No stale-missing-thread health is proven.
- No supported release path is enabled. These outcomes were neither simulated nor claimed.
- Thread listing pins this child, rather than claiming an exhaustive ownership/activity census.
  Production refuses recognized T3 targets unconditionally while release proof is unavailable.
- Passing live retained-target list/read calls do not prove deletion safety or future host health.
- No production edits, commits, remote/gh mutations, ticket status changes, T3 database/session/
  terminal changes, or checkout removal were performed.

The retained contract explicitly permits the rest of the command to ship with
T3_RELEASE_UNPROVEN recorded and recognized T3 targets retained.
