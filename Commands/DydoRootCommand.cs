namespace DynaDocs.Commands;

using System.CommandLine;
using DynaDocs.Utils;

/// <summary>
/// The dydo command tree: every subcommand the CLI registers on its root. Program.cs runs it, and the
/// tests that must see every registered command (the worker-lane classification, the command
/// documentation) read it here instead of parsing Program.cs.
/// </summary>
public static class DydoRootCommand
{
    public static RootCommand Create()
    {
        var rootCommand = new RootCommand("DynaDocs (dydo) - Durable project knowledge and native skills for AI coding assistants.");

        rootCommand.Subcommands.Add(CheckCommand.Create());
        rootCommand.Subcommands.Add(FixCommand.Create());
        rootCommand.Subcommands.Add(IndexCommand.Create());
        rootCommand.Subcommands.Add(InitCommand.Create());
        rootCommand.Subcommands.Add(AdoptCommand.Create());
        rootCommand.Subcommands.Add(GraphCommand.Create());
        rootCommand.Subcommands.Add(GuardCommand.Create());
        rootCommand.Subcommands.Add(CompletionsCommand.Create());
        rootCommand.Subcommands.Add(CompleteCommand.Create());
        rootCommand.Subcommands.Add(ValidateCommand.Create());
        rootCommand.Subcommands.Add(GapCheckCommand.Create());
        rootCommand.Subcommands.Add(MapCommand.Create());
        rootCommand.Subcommands.Add(SurveyCommand.Create());
        rootCommand.Subcommands.Add(ModeCommand.Create());
        rootCommand.Subcommands.Add(CleanCommand.Create());
        rootCommand.Subcommands.Add(PrReadyCommand.Create());
        rootCommand.Subcommands.Add(MergeCommand.Create());
        rootCommand.Subcommands.Add(StatechartCommand.Create());

        var versionCommand = new Command("version", "Display version information");
        versionCommand.SetAction(_ =>
        {
            Console.WriteLine($"dydo version {DydoVersion.Current}");
            return 0;
        });
        rootCommand.Subcommands.Add(versionCommand);

        rootCommand.Subcommands.Add(HelpCommand.Create());

        return rootCommand;
    }
}
