namespace DynaDocs.Commands;

using System.CommandLine;
using DynaDocs.Services;

public static class ArchiveCommand
{
    public static Command Create() => Create(Environment.CurrentDirectory, new SafeDeleteDependencies(), Console.Out);

    internal static Command Create(string directory, SafeDeleteDependencies dependencies, TextWriter output)
    {
        var command = new Command("archive", "Archive verified history and every file for 30 calendar days before deletion");
        foreach (var kind in new[] { "branch", "worktree" })
        {
            var target = new Argument<string>("target");
            var remote = new Option<string?>("--remote");
            var dry = new Option<bool>("--dry-run");
            var delete = new Command(kind, $"Archive and delete a {kind}; dry-run never fetches or purges");
            delete.Arguments.Add(target); delete.Options.Add(remote); delete.Options.Add(dry);
            delete.SetAction(result => new SafeDeleteService(directory, dependencies, output).Archive(kind, result.GetValue(target)!, result.GetValue(remote), result.GetValue(dry)));
            command.Subcommands.Add(delete);
        }
        var list = new Command("list", "List archives and recoverable tip refs without purging");
        list.SetAction(_ => new SafeDeleteService(directory, dependencies, output).List());
        command.Subcommands.Add(list);
        var id = new Argument<string>("id");
        var branch = new Option<string?>("--branch");
        var path = new Option<string?>("--path");
        var publish = new Option<bool>("--remote");
        var restore = new Command("restore", "Restore locally, create only; --remote recreates only the absent original same-name ref");
        restore.Arguments.Add(id); restore.Options.Add(branch); restore.Options.Add(path); restore.Options.Add(publish);
        restore.SetAction(result => new SafeDeleteService(directory, dependencies, output).Restore(result.GetValue(id)!, result.GetValue(branch), result.GetValue(path), result.GetValue(publish)));
        command.Subcommands.Add(restore);
        return command;
    }
}
