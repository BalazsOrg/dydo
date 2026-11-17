namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json.Nodes;
using DynaDocs.SkillBuild;

public class SkillBuilderTests
{
    private const string Header = "<!-- Built by tools/SkillBuild from claims/; edit the claims and rebuild. -->";

    [Fact]
    public void Build_EmitsEveryManifestFileAndTheSidecar()
    {
        var tree = new SkillBuildInput().Build();

        Assert.Equal(new[] { SkillBuilder.MapPath, SkillBuildInput.Doc, SkillBuildInput.Spine, SkillBuildInput.Resource },
            tree.Keys.Order(StringComparer.Ordinal));
    }

    [Fact]
    public void Spine_HasFrontmatterHeaderKindsInOrderACombinedUnitOnceAndItsResources()
    {
        var text = new SkillBuildInput().BuildText()[SkillBuildInput.Spine];

        Assert.Equal($"""
            ---
            name: demo
            description: A demo skill.
            ---
            {Header}

            # Demo

            ## alpha

            - Earlier, shared.
            - Later line.

            ## beta

            - Second rule.

            ## Resources

            - [Extra](resources/extra.md)

            """.ReplaceLineEndings("\n"), text);
    }

    [Fact]
    public void Spine_CarriesEveryFrontmatterLineInSourceOrder()
    {
        var input = new SkillBuildInput();
        Files(input, 0)[0]!["frontmatter"] = new JsonArray("description: A demo skill.", "disable-model-invocation: true", "name: demo");

        Assert.StartsWith("---\ndescription: A demo skill.\ndisable-model-invocation: true\nname: demo\n---\n", input.BuildText()[SkillBuildInput.Spine]);
    }

    [Theory]
    [InlineData("```text\nblock", "```text\nblock\n```")]
    [InlineData("~~~text\nblock", "~~~text\nblock\n~~~")]
    [InlineData("````text\n```\nblock", "````text\n```\nblock\n````")]
    [InlineData("```text\n~~~\nblock\n```", "```text\n~~~\nblock\n```")]
    [InlineData("```text\nblock\n```  ", "```text\nblock\n```  ")]
    [InlineData("```", "```\n```")]
    [InlineData("  ~~~text\nblock\n  ~~~~", "  ~~~text\nblock\n  ~~~~")]
    public void Unit_ClosesOnlyItsUnclosedFenceWithoutChangingProse(string prose, string expected)
    {
        var input = new SkillBuildInput();
        Prose(input, 1)["guide"]![0]!["text"] = prose;

        Assert.Contains("\n\n" + expected + "\n\n- After.\n", input.BuildText()[SkillBuildInput.Doc]);
    }

    [Fact]
    public void Resource_HasNoFrontmatter()
    {
        var text = new SkillBuildInput().BuildText()[SkillBuildInput.Resource];

        Assert.Equal($"{Header}\n\n# Extra\n\n## alpha\n\n- In the resource.\n", text);
    }

    [Fact]
    public void Doc_EmitsAMultiLineUnitVerbatimBetweenBlankLines()
    {
        var text = new SkillBuildInput().BuildText()[SkillBuildInput.Doc];

        Assert.Equal($"{Header}\n\n# Guide\n\n## alpha\n\n- Single.\n\n```text\nblock\n```\n\n- After.\n", text);
    }

    [Fact]
    public void Output_IsUtf8WithoutBomAndLfOnly()
    {
        foreach (var bytes in new SkillBuildInput().Build().Values)
        {
            Assert.NotEqual(0xEF, bytes[0]);
            Assert.DoesNotContain((byte)'\r', bytes);
            Assert.Equal((byte)'\n', bytes[^1]);
            Assert.NotEqual((byte)'\n', bytes[^2]);
        }
    }

    [Fact]
    public void Sidecar_MapsEachEmittedLineRangeToItsUnitAndClaims()
    {
        var text = new SkillBuildInput().BuildText()[SkillBuilder.MapPath];

        Assert.Equal("""
            {
              "note": "Built by tools/SkillBuild: each built file's prose line ranges, with their unit and claim ids.",
              "files": [
                {
                  "path": "dydo/guides/g.md",
                  "entries": [
                    {"lines":[7,7],"unit":"dydo/guides/g.md#L2.1","claims":["alpha.one"]},
                    {"lines":[9,11],"unit":"dydo/guides/g.md#L5.1","claims":["alpha.one"]},
                    {"lines":[13,13],"unit":"dydo/guides/g.md#L1.1","claims":["alpha.two"]}
                  ]
                },
                {
                  "path": "skills/x/demo/SKILL.md",
                  "entries": [
                    {"lines":[11,11],"unit":"skills/x/demo/SKILL.md#L3.2","claims":["alpha.one","beta.two"]},
                    {"lines":[12,12],"unit":"skills/x/demo/SKILL.md#L7.1","claims":["alpha.one"]},
                    {"lines":[16,16],"unit":"skills/x/demo/SKILL.md#L9.1","claims":["beta.two"]}
                  ]
                },
                {
                  "path": "skills/x/demo/resources/extra.md",
                  "entries": [
                    {"lines":[7,7],"unit":"skills/x/demo/resources/extra.md#L4.1","claims":["alpha.two"]}
                  ]
                }
              ]
            }

            """.ReplaceLineEndings("\n"), text);
    }

    // Six lines of header and section heading, then one line per unit: 94 units make 100 lines.
    [Theory]
    [InlineData(94, false)]
    [InlineData(95, true)]
    public void Resource_OverOneHundredLines_GetsAContentsListAfterItsH1(int units, bool contents)
    {
        var input = new SkillBuildInput();
        var entries = input.ClaimList[2]!["prose"]!["demo"]!.AsArray();
        for (var line = 5; line < 5 + units - 1; line++)
            entries.Add(Entry($"Rule {line}.", SkillBuildInput.Resource, line));

        var text = input.BuildText()[SkillBuildInput.Resource];

        Assert.Equal(contents, text.StartsWith($"{Header}\n\n# Extra\n\n## Contents\n\n- [alpha](#alpha)\n\n## alpha\n", StringComparison.Ordinal));
        Assert.Equal(contents ? units + 10 : units + 6, text.Count(character => character == '\n'));
    }

    [Fact]
    public void Spine_OverOneHundredLines_GetsNoContentsList()
    {
        var input = new SkillBuildInput();
        var entries = input.ClaimList[0]!["prose"]!["demo"]!.AsArray();
        for (var line = 10; line < 120; line++) entries.Add(Entry($"Rule {line}.", SkillBuildInput.Spine, line));

        Assert.DoesNotContain("## Contents", input.BuildText()[SkillBuildInput.Spine]);
    }

    public static TheoryData<string, Action<SkillBuildInput>> BrokenInputs => new()
    {
        { "carrier other file skills/x/other/SKILL.md (claim beta.two) is missing", input => Prose(input, 0).Add("other", new JsonArray(Entry("x", "skills/x/other/SKILL.md", 1))) },
        { "file skills/x/demo/resources/gone.md (claim beta.two) is missing", input => Demo(input, 0).Add(Entry("x", "skills/x/demo/resources/gone.md", 1)) },
        { "file dydo/guides/g.md (claim beta.two) is missing", input => Demo(input, 0).Add(Entry("x", SkillBuildInput.Doc, 8)) },
        { "claim beta.two has no prose", input => input.ClaimList[0]!["prose"] = new JsonObject() },
        { "unit skills/x/demo/SKILL.md#L3.2 has no prose", input => { Demo(input, 0)[0]!["text"] = " "; Demo(input, 1)[1]!["text"] = " "; } },
        { "unit skills/x/demo/SKILL.md#L3.2 differs between alpha.one and beta.two", input => Demo(input, 0)[0]!["text"] = "Other." },
        { "does not match source", input => Demo(input, 0)[1]!["source"] = "skills/x/demo/SKILL.md:8" },
        { "does not match source", input => Demo(input, 0)[1]!["unit"] = "skills/x/demo/SKILL.md:9" },
        { "claim beta.two is not of its kind alpha", input => input.ClaimList[0]!["kind"] = "alpha" },
        { "skills/x/demo/resources/extra.md has no prose in claims.json", input => input.ClaimList.RemoveAt(2) },
        { "base c is not the inventory's base b", input => input.Carriers["base"] = "c" },
        { "claims.json:", input => input.ClaimList[0]!.AsObject().Remove("kind") },
        { "carriers.json:", input => input.Carriers["extra"] = "unknown fields are refused" },
        { "blank or duplicate carrier 'demo'", input => input.CarrierList.Add(input.CarrierList[0]!.DeepClone()) },
        { "skills/x/demo/SKILL.md is listed twice", input => Files(input, 1).Add(Files(input, 0)[0]!.DeepClone()) },
        { "has no title", input => Files(input, 1)[0]!["title"] = "" },
        { "needs frontmatter with name and description exactly when it is a SKILL.md", input => Files(input, 0)[0]!["frontmatter"]!.AsArray().RemoveAt(1) },
        { "needs frontmatter with name and description exactly when it is a SKILL.md", input => Files(input, 0)[0]!["frontmatter"] = new JsonArray("name: ", "description: d") },
        { "needs frontmatter with name and description exactly when it is a SKILL.md", input => Files(input, 0)[0]!["frontmatter"] = new JsonArray("name: demo", "description:   ") },
        { "needs frontmatter with name and description exactly when it is a SKILL.md", input => Files(input, 0)[0]!["frontmatter"] = null },
        { "needs frontmatter with name and description exactly when it is a SKILL.md", input => Files(input, 0)[1]!["frontmatter"] = new JsonArray("name: extra", "description: extra") },
        { "must hold one SKILL.md with its resources, or one system doc", input => Files(input, 1).Add(FileEntry("dydo/guides/h.md")) },
        { "skills/x/demo/notes.md is not a resources/*.md of skills/x/demo/SKILL.md", input => Files(input, 0).Add(FileEntry("skills/x/demo/notes.md")) },
        { "skills/x/demo/resources/deep/a.md is not a resources/*.md", input => Files(input, 0).Add(FileEntry("skills/x/demo/resources/deep/a.md")) },
        { "skills/x/demo/resources/data.json is not a resources/*.md", input => Files(input, 0).Add(FileEntry("skills/x/demo/resources/data.json")) },
    };

    [Theory]
    [MemberData(nameof(BrokenInputs))]
    public void Build_FailsLoudlyOnMissingOrInconsistentInput(string message, Action<SkillBuildInput> breakInput)
    {
        var input = new SkillBuildInput();
        breakInput(input);

        var failure = Assert.Throws<InvalidDataException>(() => input.Build());

        Assert.Contains(message, failure.Message);
    }

    [Fact]
    public void Build_FailsLoudlyOnANullDocument() =>
        Assert.Contains("claims.json: the document is null",
            Assert.Throws<InvalidDataException>(() => SkillBuilder.Build("null"u8.ToArray(), "{}"u8.ToArray())).Message);

    private static JsonObject Entry(string text, string path, int line) =>
        new() { ["text"] = text, ["source"] = $"{path}:{line}", ["unit"] = $"{path}#L{line}.1" };

    private static JsonObject FileEntry(string path) => new() { ["path"] = path, ["title"] = "T" };

    private static JsonObject Prose(SkillBuildInput input, int claim) => input.ClaimList[claim]!["prose"]!.AsObject();

    private static JsonArray Demo(SkillBuildInput input, int claim) => Prose(input, claim)["demo"]!.AsArray();

    private static JsonArray Files(SkillBuildInput input, int carrier) => input.CarrierList[carrier]!["files"]!.AsArray();
}
