namespace DynaDocs.Tests.SkillBuild;

using System.Security.Cryptography;
using System.Text;
using DynaDocs.SkillBuild;

/// <summary>
/// DYD-1372 A6 over the production model: the truth-checked ticket-captain prototype takes the legacy
/// ticket captain's folder, and nothing else moves by a byte. Each digest below was taken from the
/// built files of the base, <c>bf3fea83</c>: every other role skill, both standards in both copies, the
/// 31 legacy machines, and the legacy ticket captain, which still renders them, unbuilt. A digest is
/// SHA-256 over each file's path, a NUL, its bytes and a NUL, in ordinal path order. The chief of staff's
/// digest leaves out its muster resource, which DYD-1402 rebuilt (<see cref="MusterResourceTests"/>).
/// </summary>
public class TicketCaptainSwapTests
{
    private const string Captain = "skills/roles/officers/ticket-captain/";
    private const string Muster = "skills/roles/officers/chief-of-staff/resources/muster.md";

    private static readonly string[] LegacyMachines =
    [
        "adopter", "backfill", "claim-build", "council", "dispatch", "docs", "envoy", "feature-merge", "findings", "hops", "host-setup",
        "ignore-entry", "landing", "law", "liveness", "map", "master-route", "merge-command", "mutation", "overview", "project", "proof",
        "review", "role-evaluation", "safe-delete", "should", "slug-check", "staff", "statechart-run", "ticket-status", "tribunal"
    ];

    private static readonly string[] Standards =
    [
        "dydo/guides/working-tree-standard.md", "Scaffold/dydo/guides/working-tree-standard.md",
        "dydo/reference/linear-workspace-standard.md", "Scaffold/dydo/reference/linear-workspace-standard.md"
    ];

    private static readonly Lazy<BuiltTree> Built = new(() => SkillBuilder.Build(ProductionModel.Model));

    [Theory]
    [Trait("Job", "contract: DYD-1372 A6 every other role skill, both standards and every legacy machine build the bytes of the base bf3fea83")]
    [InlineData("coordinator", "679f370c85ac46d7ae8e01c9e7bb2efc01f5c7db574a7eda5275493bcfb9b865")]
    [InlineData("chief-of-staff", "52e5f4dec6d03fcc079fa754fb82a612de7115c4d7391a6d128e4d84ba157ca5")]
    [InlineData("crew", "41f191ec23aed1c1542e46a3d868d816fe7449fafb48c6db203455e3d7c14650")]
    [InlineData("standards", "2b04b76df87dc0ed019b355f210a0e4b9909756399fc672f57e4886c90f0ce61")]
    [InlineData("machines", "a1a167eb913a99c5135210bdb9477728bb4e048679b2f960adcf161a4a5a16f2")]
    public void EverythingButTheTicketCaptain_BuildsTheBytesOfTheBase(string group, string digest)
    {
        var files = Built.Value.Files.Where(file => In(group, file.Key)).ToArray();

        Assert.NotEmpty(files);
        Assert.Equal(digest, Digest(files));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A6 the legacy ticket captain's lines stay in the model and still render its base files, 21 of them, unbuilt")]
    public void LegacyTicketCaptain_StillRendersItsBaseFilesUnbuilt()
    {
        var legacy = Built.Value.Unbuilt;

        Assert.Equal(21, legacy.Count);
        Assert.All(legacy.Keys, path => Assert.StartsWith(Captain, path));
        Assert.Equal("0720bc8040d825bf935ccd2ac74254140a9b5706bf84fb74f7b98bf05129ba56", Digest(legacy));
    }

    [Fact]
    [Trait("Job", "contract: DYD-1372 A6 the ticket-captain the model renders is the truth-checked prototype, and its render holds nothing legacy; DYD-1439 it is rendered, never written")]
    public void BuiltTicketCaptain_IsTheTruthCheckedPrototype()
    {
        var prototype = ProductionModel.Model.Skills.Single(skill => skill.Readers.Count > 0);
        var built = Built.Value.Unwritten.Keys.Where(path => path.StartsWith(Captain, StringComparison.Ordinal)).ToArray();

        Assert.Equal((CaptainSkill.TicketCaptain, Skill.TicketCaptain), (prototype.Id, prototype.Ledger!.Legacy));
        Assert.Contains(Captain + "SKILL.md", built);
        Assert.DoesNotContain(Built.Value.Files.Keys, path => path.StartsWith(Captain, StringComparison.Ordinal));
        Assert.Equal([Captain], Built.Value.HandWritten);
        Assert.All(built, path => Assert.NotEqual(Built.Value.Unbuilt.GetValueOrDefault(path), Built.Value.Unwritten[path]));
        Assert.All(Built.Value.Map.Where(line => line.Path.StartsWith(Captain, StringComparison.Ordinal)).SelectMany(line => line.Claims),
            id => Assert.DoesNotContain(id, Built.Value.Map.Where(line => !line.Path.StartsWith(Captain, StringComparison.Ordinal)).SelectMany(line => line.Claims)));
    }

    [Theory]
    [Trait("Job", "contract: DYD-1372 A3b the build lists the legacy lines that name each command the CLI does not have (DYD-1295 71e649c0)")]
    [InlineData("`dydo test`")]
    [InlineData("`dydo test gate`")]
    [InlineData("`dydo code-check`")]
    [InlineData("`dydo usage`")]
    [InlineData("`dydo survey board`")]
    public void LegacyLinesNamingAMissingCommand_AreListed(string command) =>
        Assert.Contains($" names {command}: ", Built.Value.Files["dydo/reference/claim-review/legacy-commands.md"]);

    private static bool In(string group, string path) => group switch
    {
        "standards" => Standards.Contains(path),
        "machines" => LegacyMachines.Any(machine => path.StartsWith($"claims/statechart/{machine}/", StringComparison.Ordinal)),
        "crew" => path.StartsWith("skills/roles/crew/", StringComparison.Ordinal),
        _ => path.StartsWith($"skills/roles/officers/{group}/", StringComparison.Ordinal) && path != Muster
    };

    internal static string Digest(IEnumerable<KeyValuePair<string, string>> files)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (var (path, text) in files.OrderBy(file => file.Key, StringComparer.Ordinal))
        {
            hash.AppendData(Encoding.UTF8.GetBytes(path + "\0"));
            hash.AppendData(Encoding.UTF8.GetBytes(text + "\0"));
        }
        return Convert.ToHexStringLower(hash.GetHashAndReset());
    }
}
