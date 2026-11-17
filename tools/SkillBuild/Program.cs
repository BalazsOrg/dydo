namespace DynaDocs.SkillBuild;

/// <summary><c>dotnet run --project tools/SkillBuild</c>, from the repository root.</summary>
public static class Program
{
    public static int Main() => Run(Directory.GetCurrentDirectory(), Console.Error);

    public static int Run(string root, TextWriter error)
    {
        var claims = Path.Combine(root, "claims");
        try
        {
            var tree = SkillBuilder.Build(File.ReadAllBytes(Path.Combine(claims, "inventory", "claims.json")),
                File.ReadAllBytes(Path.Combine(claims, "carriers.json")));
            BuiltTree.Write(Path.Combine(claims, "built"), tree);
            return 0;
        }
        catch (Exception failure)
        {
            if (failure is not (InvalidDataException or IOException)) throw;
            error.WriteLine($"SkillBuild failed: {failure.Message}");
            return 1;
        }
    }
}
