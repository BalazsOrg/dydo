namespace DynaDocs.Tests.SkillBuild;

using System.Text;
using System.Text.Json.Nodes;

/// <summary>
/// A small synthetic claims.json and carriers.json: a skill <c>demo</c> with one resource and a system
/// doc <c>guide</c>. Claims arrive out of order, one claim's entries out of source order, one unit is
/// shared by two claims and the doc holds a multi-line block.
/// </summary>
public sealed class SkillBuildInput
{
    public const string Spine = "skills/x/demo/SKILL.md";
    public const string Resource = "skills/x/demo/resources/extra.md";
    public const string Doc = "dydo/guides/g.md";

    public JsonNode Claims { get; } = JsonNode.Parse("""
        {
          "base": "b",
          "note": "unmapped fields are ignored",
          "claims": [
            { "id": "beta.two", "kind": "beta", "rule": "ignored", "prose": {
              "demo": [
                { "text": "Earlier, shared.", "source": "skills/x/demo/SKILL.md:3", "unit": "skills/x/demo/SKILL.md#L3.2", "combined": true },
                { "text": "Second rule.", "source": "skills/x/demo/SKILL.md:9", "unit": "skills/x/demo/SKILL.md#L9.1" } ] } },
            { "id": "alpha.one", "kind": "alpha", "prose": {
              "demo": [
                { "text": "Later line.", "source": "skills/x/demo/SKILL.md:7", "unit": "skills/x/demo/SKILL.md#L7.1" },
                { "text": "Earlier, shared.", "source": "skills/x/demo/SKILL.md:3", "unit": "skills/x/demo/SKILL.md#L3.2", "combined": true } ],
              "guide": [
                { "text": "```text\nblock\n```", "source": "dydo/guides/g.md:5", "unit": "dydo/guides/g.md#L5.1" },
                { "text": "Single.", "source": "dydo/guides/g.md:2", "unit": "dydo/guides/g.md#L2.1" } ] } },
            { "id": "alpha.two", "kind": "alpha", "prose": {
              "guide": [ { "text": "After.", "source": "dydo/guides/g.md:1", "unit": "dydo/guides/g.md#L1.1" } ],
              "demo": [ { "text": "In the resource.", "source": "skills/x/demo/resources/extra.md:4", "unit": "skills/x/demo/resources/extra.md#L4.1" } ] } }
          ]
        }
        """)!;

    public JsonNode Carriers { get; } = JsonNode.Parse("""
        {
          "base": "b",
          "carriers": [
            { "carrier": "demo", "files": [
              { "path": "skills/x/demo/SKILL.md", "title": "Demo", "name": "demo", "description": "A demo skill." },
              { "path": "skills/x/demo/resources/extra.md", "title": "Extra" } ] },
            { "carrier": "guide", "files": [ { "path": "dydo/guides/g.md", "title": "Guide" } ] }
          ]
        }
        """)!;

    public JsonArray ClaimList => Claims["claims"]!.AsArray();

    public JsonArray CarrierList => Carriers["carriers"]!.AsArray();

    public IReadOnlyDictionary<string, byte[]> Build() =>
        DynaDocs.SkillBuild.SkillBuilder.Build(Bytes(Claims), Bytes(Carriers));

    public IReadOnlyDictionary<string, string> BuildText() =>
        Build().ToDictionary(file => file.Key, file => Encoding.UTF8.GetString(file.Value), StringComparer.Ordinal);

    public static byte[] Bytes(JsonNode node) => Encoding.UTF8.GetBytes(node.ToJsonString());
}
