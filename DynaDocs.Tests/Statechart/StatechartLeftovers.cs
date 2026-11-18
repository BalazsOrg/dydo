namespace DynaDocs.Tests.Statechart;

// What a statechart run leaves under the root it was given for scratch: a run that cleaned up leaves its journals and
// saved requests, never a stage or temp directory (DR 062 §6).
internal static class StatechartLeftovers
{
    public static string[] Left(string root) => Directory.Exists(root)
        ? Directory.GetDirectories(root).Where(directory => Path.GetFileName(directory) != "requests").ToArray()
        : [];
}
