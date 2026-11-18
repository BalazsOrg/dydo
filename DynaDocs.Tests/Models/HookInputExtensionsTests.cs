namespace DynaDocs.Tests.Models;

using DynaDocs.Models;

public class HookInputExtensionsTests
{
    private static HookInput MakeInput(string? toolName, string? filePath = null, string? command = null, string? path = null)
    {
        return new HookInput
        {
            ToolName = toolName,
            ToolInput = new ToolInputData
            {
                FilePath = filePath,
                Command = command,
                Path = path,
            }
        };
    }

    [Trait("Job", "edge: DYD-396 apply_patch targets are read from the patch's structure, not its body")]
    [Fact]
    public void GetWriteTargets_ApplyPatch_ReturnsEveryTargetWithItsKindInOrder()
    {
        var patch = string.Join("\r\n",
            "*** Begin Patch",
            "*** Environment ID: env-1",
            "*** Update File: src/a.cs",
            "*** Move to: tools/moved.py",
            "@@",
            "-old",
            "+new",
            "*** Add File: tools/x.py",
            "+*** Add File: tools/not-a-header.py",
            "  *** Add File: tools/indented.py  ",
            "*** Delete File: tools/gone.py",
            @"*** Update File: C:\repo\dydo\index.md",
            "*** Delete File: /abs/dydo.json\t",
            "*** End Patch");

        var targets = MakeInput("apply_patch", command: patch).GetWriteTargets();

        Assert.Equal(
            [
                new WriteTarget("src/a.cs", WriteTargetKind.Update),
                new WriteTarget("tools/moved.py", WriteTargetKind.MoveTo),
                new WriteTarget("tools/x.py", WriteTargetKind.Add),
                new WriteTarget("tools/indented.py", WriteTargetKind.Add),
                new WriteTarget("tools/gone.py", WriteTargetKind.Delete),
                new WriteTarget(@"C:\repo\dydo\index.md", WriteTargetKind.Update),
                new WriteTarget("/abs/dydo.json", WriteTargetKind.Delete),
            ],
            targets);
    }

    // A file that quotes patch syntax: inside an Update hunk a line led by ' ', '+' or '-' is body,
    // never a header, whatever follows; the header after the hunk still counts.
    [Trait("Job", "edge: DYD-396 apply_patch targets are read from the patch's structure, not its body")]
    [Theory]
    [InlineData(" *** Update File: dydo/index.md", "\n")]
    [InlineData(" *** Update File: dydo/index.md", "\r\n")]
    [InlineData(" *** Delete File: dydo/index.md", "\n")]
    [InlineData("+*** Update File: dydo/index.md", "\r\n")]
    [InlineData("-*** Add File: dydo/index.md", "\n")]
    public void GetWriteTargets_ApplyPatch_QuotedHeaderInUpdateHunk_IsBody(string quoted, string newline)
    {
        var patch = string.Join(newline,
            "*** Begin Patch",
            "*** Update File: docs/patch-syntax.md",
            "@@ example",
            quoted,
            "-old",
            "+new",
            "*** End of File",
            "*** Delete File: docs/old.md",
            "*** End Patch");

        Assert.Equal(
            [
                new WriteTarget("docs/patch-syntax.md", WriteTargetKind.Update),
                new WriteTarget("docs/old.md", WriteTargetKind.Delete),
            ],
            MakeInput("apply_patch", command: patch).GetWriteTargets());
    }

    [Trait("Job", "edge: DYD-396 apply_patch targets are read from the patch's structure, not its body")]
    [Theory]
    [InlineData(null)]
    [InlineData("not a patch")]
    [InlineData("*** Add File:")]
    [InlineData("*** Update File:   \r\n*** Delete File:")]
    public void GetWriteTargets_UnparseablePatch_ReturnsNothing(string? patch)
    {
        Assert.Empty(MakeInput("apply_patch", filePath: "tools/x.py", command: patch).GetWriteTargets());
    }

}
