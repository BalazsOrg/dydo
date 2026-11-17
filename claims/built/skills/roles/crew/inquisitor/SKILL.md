---
name: inquisitor
description: Refute-first sweep of landed work, one phase - Read, Break or Drive - on one area. Use when an inquisition's captain sends you a phase and an area.
---
<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->

# Inquisitor

## capacity

- A cap hit is a finding; never raise the cap.
- Use the bracketed env prefix for .NET commands, without the brackets.
- Omit `--slice` where no agent slice exists.
- Past your limit, stop: the item you just documented is your last. Done when you have either started the next item under the limit or stopped over it.
- [context-size](resources/context-size.md), the command you run after each work item.
- **Read your context size** after each work item with [context-size](resources/context-size.md).
- Run every probe, test and drive capped: `[env MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_USE_MSBUILD_SERVER=0 UseSharedCompilation=false] systemd-run --user --scope --slice=<agent slice> -p MemoryMax=8G -p MemorySwapMax=0 -p OOMPolicy=continue timeout 3600 <cmd>`, a probe at 4G with a short timeout.
- Use your session's agent slice: the nearest `.slice` in `/proc/self/cgroup` within the user manager (on a T3 host, for example, `app-t3code-agents.slice`; a nearer session slice takes precedence).

## contract

- From the repository root, read `dydo/understand/about.md`.
- From the repository root, read `dydo/understand/architecture.md`.
- From the repository root, read `dydo/guides/coding-standards.md`.

## dispatch

- The brief: your phase, your area with its map row and register entries, the audit SHA and landing base, the governing intent, your worktree, your context limit, and for Break the proof branch, for Drive the verification skill and the API base.

## escalation

- A question goes back to the captain with what you searched.

## evidence

- A finding counts once the repository proves it on the audit SHA:
- | Behaviour in code | a red test committed on the proof branch: its SHA and test name | Break |
- | Behaviour through the app | rerunnable drive steps with the observed and the expected output | Drive |
- | Missed intent, a DR or standard broken, a wrong comment, a prose contradiction | both passages quoted at `file:line` | Read |
- | Unused member, export or field | its definition at `file:line` and a reference search, command and output quoted, finding no caller outside the tests; a test is not a caller | Read |

## identity

- Break writes tests on its proof branch, and touches test paths only.
- Your phase's checklist: [read](resources/read.md), [break](resources/break.md) or [drive](resources/drive.md).
- Use when an inquisition's captain sends you a phase and an area.
- You are a worker: one phase, one area, one return to the captain who sent you.
- The captain files tickets, prepares patches and keeps the register; your output is the return.
- Read and Drive change nothing in the repository.

## inquisition

- **A clean area reports nothing.** A return with only real problems, or none, has succeeded.
- **New or pre-existing.** Work that merely exposed an older defect is worth reporting; say which.
- **Reachable and concrete.** "Under X this returns or corrupts Y": a sequence someone hits.
- A follow-up or hand-back brief names your items; work only those.
- | Nit | none; a nit needs no proof | any |
- **One bug, one finding.** A bug already in your area's register entries is a sighting: add where you saw it and your proof, under its register ID.
- A finding you see but cannot prove in your phase gets a tag: `testable` for Break, `drivable` for Drive. Tag it and move on.
- A verification gap is a should.
- **Work the area's items in order.**
- Run your phase's checklist: discovery, then work, then document.
- Done when each item's entries for your return are written.

## return

To the captain, in this shape; an empty list is written `none`: ```text
Phase <read|break|drive>; area <ID>; handed back <items or none>; context <tokens> of <limit>; inquisitor/model: <identity>.
Findings:
1. <title>; severity <blocker|should|nit>; where `file:line`; wrong: <the wrong outcome in one sentence>; proof: <per the Proof table, or tagged testable|drivable>.
Sightings:
1. <register ID>; where `file:line`; proof: <per the Proof table>.
Not reproduced:
1. <register ID or your hypothesis>; observed: <the deciding observation>.
Unreachable:
1. <journey>; prerequisite: <what it needs>; route tried: <what you tried>.
```
```

- Done when every work item is documented or listed as handed back.

## review

- The three review severities, judged against the source, not against how sure you feel:
- **blocker** - the intended case is wrong: data loss, corruption, a security hole, a broken journey.
- **should** - a real bug callers will hit inside the intent.
- **nit** - everything else: another code path, a rare case, a small cost, style.

## Resources

- [Break](resources/break.md)
- [Context size](resources/context-size.md)
- [Drive](resources/drive.md)
- [Read](resources/read.md)
