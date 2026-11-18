namespace DynaDocs.Commands;

using System.Text.RegularExpressions;
using DynaDocs.Models;
using DynaDocs.Utils;

/// <summary>
/// The built-file nudge (DR 064 §5, K6): a write to a file the skill builder wrote is a warn that
/// names the file and points at the model. The shipped guard holds no model, so it knows a built file
/// by the builder's header in its first lines; a copy of tools/SkillBuild's <c>BuiltHeader</c>, which
/// a test keeps equal. Guards stop accidents: a retry of the same write passes.
/// </summary>
public static partial class GuardCommand
{
    internal const string BuiltHeaderMarkdown = "<!-- built from the model; edit there: tools/SkillBuild -->";
    internal const string BuiltHeaderYaml = "# built from the model; edit there: tools/SkillBuild";
    internal const int BuiltHeaderWindow = 16;

    private static List<WarnStep> BuiltFileWarns(IEnumerable<string> targets, string cwd, string? projectRoot) => targets
        .Select(target => PathUtils.CollapseRelativeSegments(Path.Combine(cwd, target)))
        .Where(CarriesBuiltHeader)
        .Distinct(StringComparer.Ordinal)
        .Select(path => new WarnStep(BuiltFileNudge(projectRoot is null ? path : ProjectRelativePath(path, projectRoot) ?? path),
            Match.Empty, $".nudge-built-file-{ComputeNudgeHash(path)}", "Retry the same write to proceed anyway."))
        .ToList();

    private static NudgeConfig BuiltFileNudge(string path) => new()
    {
        Severity = "warn",
        Message = $"{path} is built from the model; edit the model in tools/SkillBuild, then rebuild with `dotnet run --project tools/SkillBuild`."
    };

    private static bool CarriesBuiltHeader(string path)
    {
        if (!File.Exists(path)) return false;
        using var reader = new StreamReader(path);
        for (var index = 0; index < BuiltHeaderWindow && reader.ReadLine() is { } line; index++)
            if (line.Trim() is BuiltHeaderMarkdown or BuiltHeaderYaml) return true;
        return false;
    }
}
