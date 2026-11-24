namespace DynaDocs.Tests.Statechart;

using DynaDocs.Services.Statechart;
using System.Text.Json.Nodes;

/// <summary>
/// The statechart skill <c>install-skill</c> places, judged by what is on disk: this release's skill validates; an
/// unchanged earlier release is replaced; a local edit, an unverifiable earlier release or a foreign skill is refused
/// and left exactly as it was.
/// </summary>
public class StatechartSkillTests
{
    [Theory]
    [Trait("Job", "contract: DR 062 Consequences (the installed skill binds the payload and release: a skill that does not match them as installed does not validate)")]
    [InlineData("payload")]
    [InlineData("protocol")]
    [InlineData("source")]
    [InlineData("missing")]
    [InlineData("earlier release")]
    public async Task OnlyThisReleasesUneditedSkillValidates(string change)
    {
        using var scope = new StatechartTestDirectory();
        var assets = new StatechartAssets();
        var payload = await assets.ExtractAsync(scope.Path, CancellationToken.None);
        var skill = Path.Combine(payload, "skills", "engineering", "statechart");
        var identity = new JsonObject { ["schema"] = 1, ["protocol"] = change == "protocol" ? 2 : 1,
            ["release"] = assets.Release, ["payload"] = change == "payload" ? "other" : assets.Digest, ["lock"] = assets.LockDigest };
        if (change != "missing") File.WriteAllText(Path.Combine(skill, ".statechart-skill.json"), identity.ToJsonString());
        if (change == "source") File.AppendAllText(Path.Combine(skill, "SKILL.md"), "\nlocal edit");
        if (change == "earlier release") FromAnEarlierRelease(skill, inventory: true);
        Assert.Throws<IOException>(() => StatechartSkill.Validate(skill, assets));
        if (change == "source") Assert.EndsWith("local edit", File.ReadAllText(Path.Combine(skill, "SKILL.md")));
        if (change == "earlier release") Assert.EndsWith("Earlier release wording.", File.ReadAllText(Path.Combine(skill, "SKILL.md")));
    }

    [Fact]
    [Trait("Job", "contract: DR 062 Consequences (install-skill replaces an installed skill unchanged since its install)")]
    public async Task AnUnchangedSkillFromAnEarlierReleaseIsReplacedWithThisRelease()
    {
        using var scope = new StatechartTestDirectory();
        var (project, payload, skill) = await Installed(scope.Path);
        FromAnEarlierRelease(skill, inventory: true);
        Assert.Equal(skill, StatechartSkill.Place(project, payload, new StatechartAssets()));
        Assert.Equal(skill, StatechartSkill.Validate(skill, new StatechartAssets()));
        Assert.DoesNotContain("Earlier release wording", File.ReadAllText(Path.Combine(skill, "SKILL.md")));
        Assert.Equal(["statechart"], Directory.GetFileSystemEntries(Path.GetDirectoryName(skill)!).Select(Path.GetFileName));
    }

    [Fact]
    [Trait("Job", "contract: DR 062 Consequences (install-skill leaves local edits untouched)")]
    public async Task LocalEditsToAnEarlierReleaseAreRefusedUntouched()
    {
        using var scope = new StatechartTestDirectory();
        var (project, payload, skill) = await Installed(scope.Path);
        FromAnEarlierRelease(skill, inventory: true);
        File.AppendAllText(Path.Combine(skill, "SKILL.md"), "\nlocal edit");
        var receipt = File.ReadAllBytes(Path.Combine(skill, ".statechart-skill.json"));
        Assert.Throws<IOException>(() => StatechartSkill.Place(project, payload, new StatechartAssets()));
        Assert.EndsWith("local edit", File.ReadAllText(Path.Combine(skill, "SKILL.md")));
        Assert.Equal(receipt, File.ReadAllBytes(Path.Combine(skill, ".statechart-skill.json")));
    }

    // Receipts written before the inventory existed record no files, so only this release's exact bytes are safe to keep.
    [Fact]
    [Trait("Job", "contract: dydo-commands-utilities.md install-skill (a receipt without an inventory is replaced when its files are already this release's)")]
    public async Task AnEarlierReceiptWithoutAnInventoryIsReplacedWhenItsFilesAreThisRelease()
    {
        using var scope = new StatechartTestDirectory();
        var (project, payload, skill) = await Installed(scope.Path);
        Restamp(skill, inventory: false);
        StatechartSkill.Place(project, payload, new StatechartAssets());
        Assert.Equal(skill, StatechartSkill.Validate(skill, new StatechartAssets()));
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-utilities.md install-skill (a receipt without an inventory whose files are not this release's is refused and its skill left untouched)")]
    public async Task AnEarlierReceiptWithoutAnInventoryIsRefusedUntouchedWhenItsFilesAreNotThisRelease()
    {
        using var scope = new StatechartTestDirectory();
        var (project, payload, skill) = await Installed(scope.Path);
        FromAnEarlierRelease(skill, inventory: false);
        Assert.Throws<IOException>(() => StatechartSkill.Place(project, payload, new StatechartAssets()));
        Assert.EndsWith("Earlier release wording.", File.ReadAllText(Path.Combine(skill, "SKILL.md")));
    }

    [Fact]
    [Trait("Job", "contract: dydo-commands-utilities.md install-skill (a foreign skill stays untouched: install-skill refuses it and leaves its file as it was)")]
    public async Task ForeignCanonicalSkillIsPreservedBeforeProjectionOrPrerequisiteLaunch()
    {
        using var scope = new StatechartTestDirectory();
        var project = Path.Combine(scope.Path, "project");
        var skill = Path.Combine(project, "skills", "engineering", "statechart");
        Directory.CreateDirectory(skill);
        var sentinel = Path.Combine(skill, "SKILL.md");
        File.WriteAllText(sentinel, "human-owned");
        var home = new StatechartHome(Path.Combine(scope.Path, "assets"), Path.Combine(scope.Path, "deps"), Path.Combine(scope.Path, "state"));
        await Assert.ThrowsAsync<IOException>(() => StatechartSkill.InstallAsync(project, home, new StatechartAssets(),
            "win-x64", TextWriter.Null, CancellationToken.None));
        Assert.Equal("human-owned", File.ReadAllText(sentinel));
        Assert.False(Directory.Exists(Path.Combine(project, ".agents")));
        Assert.False(Directory.Exists(home.Dependencies));
    }

    private static async Task<(string Project, string Payload, string Skill)> Installed(string root)
    {
        var assets = new StatechartAssets();
        var payload = await assets.ExtractAsync(Path.Combine(root, "assets"), CancellationToken.None);
        var project = Path.Combine(root, "project");
        Directory.CreateDirectory(project);
        return (project, payload, StatechartSkill.Place(project, payload, assets));
    }

    // The skill as an earlier dydo release left it: other SKILL.md bytes under a receipt naming that release.
    private static void FromAnEarlierRelease(string skill, bool inventory)
    {
        File.AppendAllText(Path.Combine(skill, "SKILL.md"), "\nEarlier release wording.");
        Restamp(skill, inventory);
    }

    private static void Restamp(string skill, bool inventory)
    {
        var receipt = new JsonObject { ["schema"] = 1, ["protocol"] = 1, ["release"] = "0.0.1",
            ["payload"] = "earlier-payload", ["lock"] = "earlier-lock" };
        if (inventory) receipt["files"] = StatechartInventory.Capture(skill, ".statechart-skill.json");
        File.WriteAllText(Path.Combine(skill, ".statechart-skill.json"), receipt.ToJsonString());
    }
}
