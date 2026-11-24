---
area: project
type: reference
---

# DYD-1520 Guard-Hook Audit

The [rubric](./rubric.md) applied to the guard-hook batch: 288 cases in 14 classes at
`3f248ab5` (the hook entry, Codex `apply_patch`, the `@` argument, the refusal contract, warn
order, new-file nudges, host memory and the Claude guard wiring). One line per case is in
`guard-hook.tsv`, next to this page (`dydo check` takes only `.md` link targets). The planted-bug and mutation evidence lives under
`/var/tmp/dyd1520/guard-hook/` on the audit host. This page records what each piece showed.

## Totals

| Verdict | Cases |
|---|---|
| keep | 97 |
| rewrite | 147 |
| remove | 44 |
| keep `unsure` | 0 |

Most rewrites are tag fixes. A tag is true only when it is true of every case it covers, and
seven class tags were not:

- `GuardIntegrationTests` listed four jobs and was false of its plan-mode, search and git cases.
- `GuardWarnOrderTests` said warns run after hard blocks, which is false of the warn-only cases.
- `GuardRefusalContractTests` said a refusal is exit 0, which is false of the allow and
  other-event cases.
- `GuardAtCommandTests` and `GuardAtCommandProcessTests` named "a command" or "the argument",
  which is false of `--path`, `--action` and stdin.
- `GuardA02ProofTests` said the guard failed open, which is false of its DYD-881 must-allow
  twin.
- `GuardNewFileNudgeIntegrationTests` pinned behaviour with no spec or DR cite.
- The UNC tag in `GuardCommandTests` said "through '..'", which is false of five rows
  (the captain's note).

Four rewrites change a body:

- `Guard_ChainedIndirectDydo_StillCaught`: its `cd` input was also refused by the cd-chain rule,
  so a broken nudge could not turn it red. PB27 left it green at base.
- `Guard_CdGitCompound_BlocksWithCoachingMessage`: it never asserted the coaching text. PB28
  left it green at base.
- The three `HookInputExtensionsTests` methods: a unit test of the patch parser, which the hook
  reaches cheaply. They move to `GuardCodexApplyPatchTests` as Codex payloads.

The 44 removals:

- 16 `GuardIntegrationTests` cases. These are role-era read and write must-allows, a duplicate
  read, two zero-kill CLI and dydo must-allows, the git stash and merge must-allows for rules DR
  041 cut, and two bare git verbs.
- 11 `GuardCodexApplyPatchTests` rows where a newline and lane cross repeated a pair.
- 9 `GuardWarnOrderTests` cases: 8 marker-state twins and the install warn that
  `GuardCommandTests` covers.
- 3 `GuardAtCommandTests` and 4 `GuardAtCommandProcessTests` cases that repeated a stricter
  kept case.
- 1 `ClaudeGuardWiringTests` fact that the Gherkin scenario walks more strictly.

## How redundancy was shown

Each removed case names a kept survivor that runs the same input class. Where the two differ in
an input feature the guard reads (newline, lane, marker state, path location, extension, entry
point), a planted bug keyed to that feature fails the dropped case and a named kept case.
Every planted bug sits behind `DYD1520_PB=<id>` in one build of the base tree, so each run
changes one thing (`/var/tmp/dyd1520/guard-hook/planted/plant.py`). The unplanted control run
passed 281 of 288, with 7 Windows skips. Each run below turned red the cases listed, among
others in the batch.

| PB | Planted bug | Dropped red | Kept red (named in the table) |
|---|---|---|---|
| 01 | a CRLF patch names no targets | 7 Codex CRLF rows, 3 parser rows | `DeleteFileOfProtectedIndex_Denied[5]`, `UpdateFileIntoCodexMemories_Denied[3]` |
| 02 | a CRLF patch also names dydo.json as a Delete | `UpdateFileOfOrdinaryFile_Allowed[1]` | `UpdateFileOfOrdinaryFile_Allowed[2]`, `UpdateFileQuotingPatchHeader_Allowed[1]`, `[2]` |
| 03 | the worker lane ignores patch targets | 5 worker rows | `DeleteFileOfProtectedIndex_Denied[5]`, `RelativeUpdate…[3]`, `OmittedCwd…[3]`, `UpdateFileIntoCodexMemories_Denied[3]` |
| 04 | the main lane ignores patch targets | 5 main rows | `DeleteFileOfProtectedIndex_Denied[0]`, `[2]`, `RelativeUpdate…[0]`, `OmittedCwd…[0]`, `UpdateFileIntoCodexMemories_Denied[0]` |
| 05 | a hard-blocked call spends an armed warn marker | `HardBlock…[1]`, `[5]`, `[9]`, `[13]` | `HardBlock…[3]`, `[7]`, `[11]`, `[15]` |
| 06 | a hard-blocked call arms an unarmed warn marker | `HardBlock…[2]`, `[6]`, `[10]`, `[14]` | `HardBlock…[0]`, `[4]`, `[8]`, `[12]` |
| 09 | Program.cs parses with response files on | 4 E2E rows | E2E `GuardCommand_AtExistingFile…`, `…GetsAVerdict…[2]` |
| 10 | CliParsing keeps response files on | 3 in-process, 4 E2E | `CliCommand_AtExistingFile…`, `CliPath_StartingWithAt…` and 4 more |
| 12 | a CLI read of an unprotected path is refused | 3 CLI reads, `CliPath_AtExistingFileOfOptions…` | `CliPath_StartingWithAt_IsEvaluatedAsAPath` |
| 12b | a CLI file op on a nested unprotected path is refused | `ReadSourceFile`, `ReadNonAgentWorkflow` | `Guard_AllowedPath_Passes` |
| 12c | a CLI read of a .md path is refused | `ReadRootFile`, `ReadNonAgentWorkflow`, `DydoSystemFile_ReadAllows` | `Guard_ProtectedPath_ReadAllowed[0]`, `[1]` |
| 13 | a hook Read of an unprotected path is refused | `ReadBootstrapFile` | `GuardDailyValidation…[0]`-`[3]` |
| 13b | a hook Read of a root-level path is refused | `ReadBootstrapFile` | `Guard_ProtectedPath_ReadAllowed[2]` |
| 13c | a hook Read of a .md path is refused | `ReadBootstrapFile` | `Guard_ProtectedPath_ReadAllowed[0]`, `[1]` |
| 13w | a hook Write of an unprotected path is refused | `Guard_StdinHook_Write_Allows` | `FileToolWrite…[24]`, `UnreadableSettings_NameNoMemoryFolder` |
| 14 | a CLI `--command` starting with dotnet is refused | `Guard_CommandOption_Parses` | `RealLaunch_MeetsItsNudge[0]` |
| 17 | a hook Read of an all-ASCII path skips off-limits | `DirectReadOffLimits_Blocks` | `Guard_NonWorkerTraversalPath…[0]` |
| 19 | the npx nudge's npx is optional | `Guard_DirectDydo_StillWorks` | `ModeSet_WithAWarn_BlocksAndPassesTogether` and 3 more |
| 20 | every non-destructive git verb reads as destructive | 5 stash and merge rows, `OtherGitCommands[2]`, `[3]` | `OtherGitCommands[0]`, `[1]` and 4 more |

The git stash and merge rows check that a deleted rule stays gone (rubric §3, smell 2). The
cut is `2c35d299e` (DR 041). The guard reads a git verb only through `DestructiveGitCall`'s
switch and the push-route test, and stash, merge, diff and log all take the same default arm as
the kept `git status`. A bug keyed to one of those verbs would be new code naming it, not a
regression of code that reads it.

## Red proofs for rewrites

A rewrite that changes only its tag keeps its body. Its red check is a baseline mutant it
killed, named per case in the table's Evidence cell. Cases with no baseline kill were shown red
by planted bugs at base:

- PB21 (daily validation creates `dydo/` under the guard's directory) turned all three
  `Guard_LeavesNoDydoStateOutsideTheDydoRoot` rows red. The code sits in a catch-all, which is
  why mutation never killed it.
- PB22 (the force-push rule is off) turned E2E `…GetsAVerdict…[2]` and
  `GuardHookLane_HereStringOnStdin_IsDenied` red.
- PB09 turned E2E `GuardCommand_AtExistingFile…` red.
- PB23 (the npx nudge no longer needs dydo) turned `FalsePositiveSafety[0]` red.
- PB24 (a search with no path is refused) turned both `NoPath_Allows` rows red.
- PB25 (an allow answers with an allow object) turned 87 cases red, `AllowedCall_ExitsZero…`
  among them.

The body-changing rewrites are shown red on the candidate in the apply commit. The seven
Windows-only UNC rows ([2] to [8]) have no red run here: they skip on Linux, and no Windows host
or CI run is available to this hop. Their red check is owed to `guard-windows`.

## Mutation

The baseline is Stryker.NET 5.1.0 over the whole product at `4fff1ad3`, filtered to this batch,
with an 11.52% score. Production code is the same at `3f248ab5`. `redundancy.py --remove` on
the 44 removals plus the replaced parser tests lists 25 mutants that would lose their last
recorded killer:

- 19 that only `RejoiningClaude_ClearsTheReportedMissingGuard` killed. Stryker found the
  scenario but captured no coverage for it, so it ran the scenario against no mutant. A separate
  run with coverage analysis off checks these against the scenario.
- 6 recorded against one removed case. Stryker bails at a mutant's first failing test, so
  `killedBy` names one killer where several fail: install-nudge pinning, `GuardCommand.cs`
  lines 967 and 968; three patch-header strings, `HookInputExtensions.cs` lines 37 to 39; and
  the host-memory word, `BashCommandAnalyzer.cs` line 99. Each one's kept cases run the same
  input.

Targeted phase 1 ran on the base tree over the 30 methods that lose a case or change a body.
Tag-only rewrites change no executable line, so they are left out of phase 1; phase 2 runs them
anyway as kept tests. Phase 2 on the candidate is the after-check.

## Deviations

- Tag-only rewrites take a baseline mutant kill as their red check instead of a fresh planted
  run: the body is byte-identical, and the kill was observed with the same assertions.
- The parser test's order assertion ("every target, in call order") is dropped, not moved. No
  spec or DR names the order. A reversed order changes no verdict, only which of two refused
  targets a refusal names, so it is an untraced pin (rubric §3).
- `GuardIntegrationTests`' `DirectDydo` and `CommandOption_Parses`, and the E2E rows, had no
  baseline kill: their evidence is planted bugs alone.

## Related

- [Test rubric](./rubric.md) - The verdict order, the evidence a removal needs, the record line
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - Every batch's audit
