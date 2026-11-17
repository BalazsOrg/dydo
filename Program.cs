using DynaDocs.Commands;

if (args.Length > 0 && args[0] is "__statechart-cleanup" or "__statechart-probe" or
    "__statechart-posix-owner" or "__statechart-posix-anchor" or "__statechart-posix-child")
{
    try
    {
        return args[0] switch
        {
            "__statechart-cleanup" => await DynaDocs.Services.Statechart.StatechartCleanup.RunAsync(args[1..], Console.In, Console.Out),
            "__statechart-probe" => await DynaDocs.Services.Statechart.StatechartProbe.ChildAsync(args[1..]),
            "__statechart-posix-owner" => await DynaDocs.Services.Statechart.PosixStatechartOwner.RunAsync(args[1..]),
            "__statechart-posix-anchor" => await DynaDocs.Services.Statechart.PosixStatechartAnchor.RunAsync(args[1..]),
            _ => DynaDocs.Services.Statechart.PosixStatechartChild.Run(args[1..])
        };
    }
    catch (Exception error) when (error is IOException or InvalidDataException or InvalidOperationException or System.ComponentModel.Win32Exception)
    {
        Console.Error.WriteLine(error.Message);
        return 2;
    }
}

if (args.Length > 0 && (args[0] == "gap-check" || args[0] == "statechart" && !args.Skip(1).Any(argument => argument is "--help" or "-h" or "-?")))
{
    using var cancellation = new CancellationTokenSource();
    if (args[0] == "statechart" && Environment.GetEnvironmentVariable("DYDO_STATECHART_NPM_LIFETIME") == "1")
        _ = DynaDocs.Services.Statechart.StatechartOuterLifetime.ObserveAsync(Console.OpenStandardInput(), cancellation);
    ConsoleCancelEventHandler handler = (_, eventArgs) =>
    {
        eventArgs.Cancel = true;
        cancellation.Cancel();
    };
    Console.CancelKeyPress += handler;
    try
    {
        return args[0] == "statechart"
            ? await StatechartCommand.ExecuteAsync(args[1..], Environment.CurrentDirectory, cancellation.Token)
            : await GapCheckCommand.ExecuteAsync(args[1..], Environment.CurrentDirectory, cancellation.Token);
    }
    finally
    {
        Console.CancelKeyPress -= handler;
    }
}

return await CliParsing.Parse(DydoRootCommand.Create(), args).InvokeAsync();
