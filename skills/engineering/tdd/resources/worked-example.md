# Worked example: a red check on dydo's runner

This example runs in the dydo repository, on its own suite and runner. In another project, take the same three runs and use the commands its `dydo/guides/testing-strategy.md` names.

## The test

`DynaDocs.Tests/Rules/OffLimitsRuleTests.cs` guards `dydo check` on the off-limits file. Its rows drive the product boundary, the `dydo check` command, through `DydoCli`, and assert the exit code and the finding a user sees. Abridged:

```csharp
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-core.md dydo check (validates the off-limits file); trap: an unclosed fence silently drops off-limits patterns")]
public sealed class OffLimitsRuleTests
{
    [Theory]
    [InlineData(Head + "```\n.env*\n", 1, "Unclosed code block")]
    // ... four more rows ...
    public void Check_ReportsTheOffLimitsFinding(string? offLimits, int expectedExit, string? expectedFinding)
    {
        using var project = new DocsProject();
        project.Write("files-off-limits.md", offLimits);

        var (code, output) = DydoCli.RunCombined("check");

        Assert.True(expectedExit == code, $"exit {code}, expected {expectedExit}:\n{output}");
        Assert.Contains(expectedFinding, output);
    }
}
```

The unclosed-fence row guards a trap: `dydo check` must report an unclosed fence as an error and exit 1, not as a warning with exit 0. The red check below measures exactly that, the severity and the exit code.

**Its tag is too broad.** The class-level tag names that trap, but the class's other rows (a missing file, a duplicate pattern, a missing literal path, a clean file) do not guard it. A tag must be true of every case it covers, so a review of this test finds the tag false. The true shape gives the trap a test of its own and leaves `contract` alone on the class:

```csharp
[Collection("Integration")]
[Trait("Job", "contract: dydo-commands-core.md dydo check (validates the off-limits file)")]
public sealed class OffLimitsRuleTests
{
    [Fact]
    [Trait("Job", "trap: dydo check reported an unclosed fence as a warning and exited 0")]
    public void Check_UnclosedFence_FailsWithExit1()
    {
        using var project = new DocsProject();
        project.Write("files-off-limits.md", Head + "```\n.env*\n");

        var (code, output) = DydoCli.RunCombined("check");

        Assert.True(code == 1, $"exit {code}, expected 1:\n{output}");
        Assert.Contains("Unclosed code block", output);
    }

    // The theory keeps its other four rows.
}
```

The three runs below were recorded on the test as it stands, with the trap as a theory row. A scratch copy of the split test, run the same way on 2026-11-28, passed 1 of 1, then failed under the same plant with `exit 0, expected 1`.

## The runner

dydo's runner is the testing facade `DynaDocs.Tests/coverage/gap_check.py`. `test --stack dotnet` runs the .NET suite in an isolated git worktree, with your uncommitted changes copied in, so a planted bug is tested without a commit. Arguments after `--` go to `dotnet test`; `--filter` selects the tests. The test row needs only the Python standard library, so the pinned interpreter or any CPython 3.14 runs it. Run it under the host's gate lock and memory cap, as testing-strategy's Runner prerequisites and shared hosts says.

```bash
python3.14 DynaDocs.Tests/coverage/gap_check.py test --stack dotnet -- \
  --filter "FullyQualifiedName~DynaDocs.Tests.Rules.OffLimitsRuleTests"
```

Each run writes a `result.json` under `DynaDocs.Tests/coverage/results/` and prints its path. Exit 0 is a pass and exit 1 a measured failure.

## The three runs

Recorded on 2026-11-28, at `352b1e579ec10c1180dbb859f0c40bc12cdbf0b2`, with .NET SDK 10.0.303 and CPython 3.14.4.

**1. Green on the code as it is.**

```text
Passed!  - Failed:     0, Passed:     6, Skipped:     0, Total:     6, Duration: 103 ms - DynaDocs.Tests.dll (net10.0)
dotnet test: PASSED (child exit 0)
Aggregate: 0
```

**2. Red with the guarded code broken.** Plant the bug in `Rules/OffLimitsRule.cs`: every format issue becomes a warning.

```diff
-                issue.IsError ? ViolationSeverity.Error : ViolationSeverity.Warning
+                ViolationSeverity.Warning
```

The same command fails on the trap row, for the reason the trap names: exit 0 where 1 was due.

```text
Failed DynaDocs.Tests.Rules.OffLimitsRuleTests.Check_ReportsTheOffLimitsFinding(offLimits: "---\ntype: config\n---\n\n# Files Off-Limits\n\n``"···, expectedExit: 1, expectedFinding: "Unclosed code block") [2 ms]
  Error Message:
   exit 0, expected 1:
WARNINGS:
  dydo/files-off-limits.md
    - Unclosed code block (``` without closing ```)
Found 0 errors, 1 warnings in 2 files.
Failed!  - Failed:     1, Passed:     5, Skipped:     0, Total:     6, Duration: 95 ms - DynaDocs.Tests.dll (net10.0)
dotnet test: FAILED (child exit 1)
Aggregate: 1
```

The other five rows stay green: they guard other findings. A row that stays green under every plausible break of its code can never fail, and that is a finding.

**3. Green again** after undoing the planted line by hand. Undo only your own plant: a `git checkout --` of the file would also throw away any other uncommitted edit in it.

```text
Passed!  - Failed:     0, Passed:     6, Skipped:     0, Total:     6, Duration: 97 ms - DynaDocs.Tests.dll (net10.0)
Aggregate: 0
```

On your ticket, record the command, the commit, the planted edit, and what each of the three runs printed.

## As a bug

Had the severity bug shipped, the same row is the reproduction: red as in run 2 before any fix. Fix the code and watch it go green. Then revert the fix once, watch the row fail again, and restore the fix.

## dydo's job tags

| Language | Tag |
|---|---|
| C# | `[Trait("Job", "<trap\|contract\|edge>: <cite>")]` on the test or its class |
| Python | a `Job: <kind>: <cite>` line in the test's docstring |
| Gherkin | a `# Job:` comment heading the feature file |
| Node | a `// Job:` comment above the `test()` call, or above the loop that registers it |

## dydo's boundaries

- **In process**: `DydoCli.Run` and `DydoCli.RunCombined` run a command line through the registered command tree, the way `Program.cs` does, and capture the exit code and output. `DocsProject` gives the test a disposable docs project. Most boundary tests start here.
- **Out of process**: `DynaDocs.Tests/EndToEnd/` runs the built `dydo.dll` as a subprocess, for what in-process parsing cannot reach: startup, real exit codes, the guard hook.
