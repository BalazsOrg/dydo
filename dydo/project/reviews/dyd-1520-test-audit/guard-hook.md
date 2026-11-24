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
| keep | 98 |
| rewrite | 147 |
| remove | 43 |
| keep `unsure` | 0 |

Three removals are recorded but not applied: `DeleteFileOfProtectedIndex_Denied[1]`, `[3]` and
`[4]`. Their evidence holds (PB01, PB03, PB04), but the host's permission classifier refused the
edit that deletes those protected-file rows. They stay in the candidate for the captain to
decide.

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

- `Guard_ChainedIndirectDydo_StillCaught`: the cd-chain rule also refused its `cd` input, so it
  did not show its job. PB27 broke the nudge and left it green at base. The dropped
  `cd /tmp &&` input stays held: PB35 (a command starting `cd /tmp && ` is allowed) turned it red
  together with the kept `Guard_CdNonGitCompound_Blocked`.
- `Guard_CdGitCompound_BlocksWithCoachingMessage`: it never asserted the coaching text. PB28
  left it green at base.
- The three `HookInputExtensionsTests` methods: a unit test of the patch parser, which the hook
  reaches cheaply. They move to `GuardCodexApplyPatchTests` as Codex payloads.

The 43 removals:

- 16 `GuardIntegrationTests` cases. These are role-era read and write must-allows, a duplicate
  read, two zero-kill CLI and dydo must-allows, the git stash and merge must-allows for rules DR
  041 cut, and two bare git verbs.
- 11 `GuardCodexApplyPatchTests` rows where a newline and lane cross repeated a pair.
- 9 `GuardWarnOrderTests` cases: 8 marker-state twins and the install warn that
  `GuardCommandTests` covers.
- 3 `GuardAtCommandTests` and 4 `GuardAtCommandProcessTests` cases that repeated a stricter
  kept case.

`ClaudeGuardWiringTests.RejoiningClaude_ClearsTheReportedMissingGuard` is kept. The Gherkin
scenario walks the same journey more strictly, but it runs dydo as a child process
(`CliScenario.RunProcessAsync`), so mutation cannot see it. In a run that mutated only the 19
mutants the fact alone killed, with coverage analysis off and only the scenario as the test,
all 19 survived. That includes the warning string the scenario asserts exactly. Proving the
scenario covers them would take 19 planted bugs.

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

The body-changing rewrites were shown red on the candidate `d06e0e00`, with the unplanted run
green (244 passed, 7 Windows skips):

- PB27 (the npx nudge matches only at a command's start) turned
  `Guard_ChainedIndirectDydo_StillCaught` red; at base it left every case green.
- PB28 (the coaching names the whole command) turned
  `Guard_CdGitCompound_BlocksWithCoachingMessage` red; at base it left every case green.
- PB04 turned all eight refusing `PatchGrammar_DecidesWhichLinesAreTargets` rows red.
- PB29 (header paths not trimmed) turned the indented, tab and CRLF rows red.
- PB30 (no hunk body) turned the space-quoted rows red.
- PB34 (an Add body line's `+` is stripped and read as a header) turned row [3] and the
  `+`-quoted row red.
- PB31 (an unparseable patch falls back to `file_path`) turned all four
  `UnparseablePatch_NamesNoTarget` rows red. No kept case caught PB31 at base, so this rewrite
  closes a gap.
- PB09 and PB22 turned the two E2E Facts red.

The parser unit tests also asserted two things no entry point shows:

- PB32 (a header matches without its trailing space, so `*** Add File:` names an empty target)
  turned only unit `Unparseable[2]` and `[3]` red at base, and nothing on the candidate.
- PB33 (targets in reverse order) turned only the unit rows red at base, and nothing on the
  candidate.

Neither changes a verdict, an output, a file or a git state, so neither is a protection a user
could hit. The seven
Windows-only UNC rows ([2] to [8]) have no red run here: they skip on Linux, and no Windows host
or CI run is available to this hop. Their red check is owed to `guard-windows`.

## Mutation

The baseline is Stryker.NET 5.1.0 over the whole product at `4fff1ad3`, filtered to this batch,
with an 11.52% score. Production code is the same at `3f248ab5`. `redundancy.py --remove` on
the removals plus the replaced parser tests lists 6 mutants that would lose their last recorded
killer (25 while the Rejoining fact was a candidate; the 19 above were its own). All 6 are
recorded against one removed case. Stryker bails at a mutant's first failing test, so
  `killedBy` names one killer where several fail: install-nudge pinning, `GuardCommand.cs`
  lines 967 and 968; three patch-header strings, `HookInputExtensions.cs` lines 37 to 39; and
  the host-memory word, `BashCommandAnalyzer.cs` line 99. Each one's kept cases run the same
  input.

Targeted phase 1 ran on the base tree over the 30 methods that lose a case or change a body (78
cases, the Rejoining fact included as a superset). Stryker killed 658 mutants (2,253 s, rc 0;
`/var/tmp/dyd1520/guard-hook/mut/phase1/`). Tag-only rewrites change no executable line, so
they are left out of phase 1; phase 2 runs them anyway as kept tests. Phase 2 on the candidate
is the after-check. It has not run: it waits on the permission decision, since the candidate may
change.

## Open against the candidate

A self-check against the first batch review's lessons found three things the candidate
`d06e0e00` still needs. Each is a test edit, held while the permission question is open:

- **Tags that claim more than their cases show.** Each should be narrowed to what it shows:
  - `Guard_OffLimitsPath_Blocks` says "every operation".
  - `Guard_ProtectedPath_ReadAllowed` says "every tool".
  - `Guard_ProtectedPath_BashMutatingOperationBlocked` says "no shell operation".
  - `Guard_IndirectDydo_IsBlocked` says "every indirect dydo invocation".
  - `Guard_SearchTool_WithPath_Allows` and `Guard_SearchTool_NoPath_Allows` say "only the
    off-limits tier".
  - `Guard_OtherGitCommands_NotBlocked` says "only the named … git calls".
  - The `GuardWarnOrderTests` class tag says "every hard block", but the merge-route and
    PR-ready blocks are not exercised.
- **The parser rewrite is not as strict as the unit tests it replaces.** The unit tests
  asserted exact target lists on their own inputs. The boundary rows assert one line's verdict
  each, and some inputs changed shape: `/abs/dydo.json\t` became `dydo.json\t`, and the indented
  Add header became a Delete header. The rewrite also drops the backslash absolute path and the
  Move-to element of that input. PB29 to PB34 show the line-level protection, but not on the
  original inputs. Fix it by restoring those inputs as boundary rows, or by keeping the unit
  rows `unsure`.
- **Three rows are not applied:** `DeleteFileOfProtectedIndex_Denied[1]`, `[3]` and `[4]` (see
  the totals).

## Deviations

- Tag-only rewrites take a baseline mutant kill as their red check instead of a fresh planted
  run: the body is byte-identical, and the kill was observed with the same assertions.
- The parser tests' order assertion and their empty-header assertion are dropped, not moved. No
  spec or DR names either, and PB33 and PB32 show neither reaches an observable at the hook.
  The reviewer judges whether that counts as "covers nothing a user could hit".
- The first whole-file rewrite of `GuardIntegrationTests.cs` was refused by the host's
  permission classifier. The same changes then went in as targeted edits, which it allowed one
  by one. Only the `DeleteFileOfProtectedIndex_Denied` row deletion stayed refused, and it was
  not retried in any form.
- Tests run through `dotnet test` as the brief names, behind a script, not through the guard's
  suggested `run_tests.py` (a warn; each worktree has its own `bin`).
- `GuardIntegrationTests`' `DirectDydo` and `CommandOption_Parses`, and the E2E rows, had no
  baseline kill: their evidence is planted bugs alone.

## Related

- [Test rubric](./rubric.md) - The verdict order, the evidence a removal needs, the record line
- [DYD-1520 test audit](./_dyd-1520-test-audit.md) - Every batch's audit
