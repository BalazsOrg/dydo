namespace DynaDocs.Tests;

using DynaDocs.Commands;

/// <summary>
/// Runs a dydo command line through the registered command tree, the way Program.cs does, and
/// captures its exit code and output. Tests that use it share the process's current directory and
/// console, so their classes belong to the "Integration" collection.
/// </summary>
public static class DydoCli
{
    public static (int Code, string Stdout, string Stderr) Run(params string[] args) =>
        ConsoleCapture.All(() => CliParsing.Parse(DydoRootCommand.Create(), args).Invoke());

    /// <summary>The run's stdout and stderr together, for asserting a finding wherever it is printed.</summary>
    public static (int Code, string Output) RunCombined(params string[] args)
    {
        var (code, stdout, stderr) = Run(args);
        return (code, stdout + stderr);
    }
}
