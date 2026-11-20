namespace DynaDocs.Tests.SkillBuild;

using System.Text.Json;
using DynaDocs.SkillBuild;
using DynaDocs.Tests.SkillBuild.Slots;

/// <summary>
/// K9's spine and scenarios, K14's dispatch template, K26's Codex metadata and K6's header and
/// destinations, as the slot fixture builds them (DYD-1196 A5, B9, B10, B11, C18).
/// </summary>
public class SpineLayoutTests
{
    private const string Folder = "skills/roles/crew/demo/";

    private static readonly IReadOnlyDictionary<string, string> Built = SkillBuilder.Build(DemoModel.With()).Files;

    [Fact]
    [Trait("Job", "contract: K9 the spine renders its job paragraph, numbered phases with their slots and links, then When things go wrong")]
    public void Spine_RendersJobPhasesAndWhenThingsGoWrongInOrder() =>
        Assert.Equal(
            """
            ---
            name: demo
            description: A demo skill.
            disable-model-invocation: true
            ---
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Demo

            You own one ticket from intake to return.
            You return to the officer who sent you.

            ## 1. Take the ticket

            - You answer for the proof of your change.

            ### Tools and surfaces

            - Your worktree and its branch.

            ### Intake

            Arrivals:

            - Read the brief first.

            Nominal path:

            - Check the worktree is clean.
            - Check the base is an ancestor of HEAD.

            Exits:

            - Stop when the lease goes stale.

            ## 2. Do the work

            ### Work

            Arrivals:

            - Start work once intake is done.

            ### Dispatch crew

            - Dispatch crew with a ticket.

            Resources:

            - [Guide](resources/guide.md)
            - [Crew brief](resources/brief.md)

            ## When things go wrong

            - Each scenario ends resolved or escalated.
            - [Stale lease](resources/stale-lease.md)

            """.ReplaceLineEndings("\n"),
            Built[Folder + "SKILL.md"]);

    [Fact]
    [Trait("Job", "contract: K9 a scenario is its own resource file, linked from When things go wrong")]
    public void Scenario_IsAResourceFileWithItsLinesInOrder() =>
        Assert.Equal(
            """
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Stale lease

            - Save the work, then resume.
            - Escalate to the officer when the lease cannot be renewed.

            """.ReplaceLineEndings("\n"),
            Built[Folder + "resources/stale-lease.md"]);

    [Fact]
    [Trait("Job", "contract: K14 a dispatch template opens with its default skill set, each skill named with a slash")]
    public void Template_OpensWithItsSkillsEachWithASlash() =>
        Assert.Equal(
            """
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Crew brief

            Skills: /demo

            """.ReplaceLineEndings("\n"),
            Built[Folder + "resources/brief.md"]);

    [Fact]
    [Trait("Job", "trap: a fenced block bulleted line by line breaks, so a multi-line line is emitted verbatim")]
    public void Resource_OpensWithItsUntitledSectionAndKeepsABlockVerbatim() =>
        Assert.Equal(
            """
            <!-- built from the model; edit there: tools/SkillBuild -->

            # Guide

            - Read this guide when a command is unclear.

            ## Commands

            ```bash
            dydo check
            ```

            """.ReplaceLineEndings("\n"),
            Built[Folder + "resources/guide.md"]);

    [Fact]
    [Trait("Job", "contract: K26/K27 an explicit-only skill's openai.yaml turns implicit invocation off and carries the model's blocks (B10, B11)")]
    public void ExplicitOnlySkill_EmitsOpenAiYamlWithThePolicyAndTheModelsBlocks() =>
        Assert.Equal(
            """
            # built from the model; edit there: tools/SkillBuild
            interface:
              display_name: Demo
            dependencies:
              tools: []
            policy:
              allow_implicit_invocation: false

            """.ReplaceLineEndings("\n"),
            Built[Folder + "agents/openai.yaml"]);

    [Fact]
    [Trait("Job", "contract: K6 a workflow skill builds to skills/productivity/<name>/, with no openai.yaml the model does not set (B9, B10)")]
    public void SkillWithNoCodexMetadata_EmitsNoOpenAiYaml()
    {
        var plain = SkillSpec.Workflow(DemoSkill.Demo, "Demo", "A demo skill.");

        Assert.Equal(["skills/productivity/demo/SKILL.md"], SkillBuilder.Build(new([], [plain])).Files.Keys);
    }

    [Fact]
    [Trait("Job", "contract: K26/K27 openai.yaml carries the model's blocks, and only an explicit-only skill turns invocation off (B10)")]
    public void OpenAiYaml_WithoutExplicitOnly_CarriesOnlyTheModelsBlocks()
    {
        var skill = SkillSpec.Officer(DemoSkill.Demo, "Demo", "d") with { OpenAi = new(Interface: "display_name: Demo") };

        var built = SkillBuilder.Build(new([], [skill])).Files;

        Assert.Equal(BuiltHeader.Yaml + "\ninterface:\n  display_name: Demo\n", built["skills/roles/officers/demo/agents/openai.yaml"]);
        Assert.DoesNotContain("disable-model-invocation", built["skills/roles/officers/demo/SKILL.md"]);
    }

    // B11/K6: a SKILL.md opens with YAML frontmatter. A plain scalar holding ": " does not parse, and
    // " #" starts a comment, so such a description is double-quoted; YAML shares JSON's escapes.
    [Theory]
    [Trait("Job", "trap: B11/K6 a description with YAML syntax in it builds frontmatter that reads back as that description")]
    [InlineData("A demo skill.", false)]
    [InlineData("Use when: review is needed.", true)]
    [InlineData("Keep the tail # of this line.", true)]
    [InlineData("Say \"hi\", then \\ and 'quote'.", true)]
    [InlineData("- a list item", true)]
    [InlineData("Yes", true)]
    [InlineData("Ends on a line break\n", true)]
    public void Description_BuildsAFrontmatterValueThatReadsBackAsItself(string description, bool quoted)
    {
        var skill = SkillSpec.Workflow(DemoSkill.Demo, "Demo", description);

        var line = SkillBuilder.Build(new([], [skill])).Files["skills/productivity/demo/SKILL.md"].Split('\n')[2];

        Assert.StartsWith("description: ", line);
        var value = line["description: ".Length..];
        Assert.Equal(quoted, value.StartsWith('"'));
        Assert.Equal(description, quoted ? JsonSerializer.Deserialize<string>(value) : value);
    }

    [Fact]
    [Trait("Job", "contract: K6/K12 a standard builds byte-identical copies under dydo/ and Scaffold/dydo/ (B9)")]
    public void Standard_BuildsBothCopiesWithItsFrontmatterAndHeader()
    {
        var standard = SkillSpec.Standard(DemoSkill.Demo, "guides", "Demo Standard", "area: guides", "type: guide") with
        {
            Sections = [new(DemoSlot.GuideLead)]
        };

        var built = SkillBuilder.Build(new([], [standard])).Files;

        Assert.Equal(["Scaffold/dydo/guides/demo.md", "dydo/guides/demo.md"], built.Keys);
        Assert.Equal("---\narea: guides\ntype: guide\n---\n" + BuiltHeader.Markdown + "\n\n# Demo Standard\n", built["dydo/guides/demo.md"]);
        Assert.Equal(built["dydo/guides/demo.md"], built["Scaffold/dydo/guides/demo.md"]);
    }

    [Fact]
    [Trait("Job", "contract: K6 every built file carries the header, right after a SKILL.md's frontmatter (B11)")]
    public void EveryBuiltFile_CarriesTheHeaderWhereTheGuardLooks()
    {
        foreach (var (path, text) in Built)
        {
            var lines = text.Split('\n');
            var header = path.EndsWith(".yaml", StringComparison.Ordinal) ? BuiltHeader.Yaml : BuiltHeader.Markdown;
            var expected = lines[0] == "---" ? Array.IndexOf(lines, "---", 1) + 1 : 0;
            Assert.Equal(header, lines[expected]);
            Assert.True(expected < BuiltHeader.Window, path);
        }
    }
}
