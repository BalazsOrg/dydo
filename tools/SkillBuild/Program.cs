namespace DynaDocs.SkillBuild;

/// <summary>
/// <c>dotnet run --project tools/SkillBuild</c>, from the repository root, writes the production
/// model's built files; <c>-- map</c> prints the line-to-claim map instead (K23) and writes nothing.
/// Either way, before it writes or prints anything, it checks that each envoy table skill (H23), each
/// dispatch template skill and each skill the built prose names with a slash (K14) is a skill under
/// <c>skills/</c>, hand-written or built.
/// </summary>
public static class Program
{
    public static int Main(string[] args) =>
        Run(ProductionModel.Model, Directory.GetCurrentDirectory(), args, Console.Out, Console.Error);

    public static int Run(Model model, string root, string[] args, TextWriter output, TextWriter error)
    {
        if (args is not ([] or ["map"]))
        {
            error.WriteLine("usage: dotnet run --project tools/SkillBuild [-- map]");
            return 2;
        }
        try
        {
            var tree = SkillBuilder.Build(model);
            SkillNames.Check(model, tree, root);
            if (args.Length == 0) tree.Write(root);
            else foreach (var line in tree.Map) output.WriteLine(line);
            return 0;
        }
        catch (InvalidDataException failure)
        {
            error.WriteLine($"SkillBuild failed: {failure.Message}");
            return 1;
        }
    }
}
