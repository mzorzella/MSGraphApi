using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MSGraphApi.Downloader;
using MSGraphApi.Downloader.Operations;
using MSGraphApi.Library.Services.GraphApi;
using Spectre.Console;

using IHost host = SetupHostBuilder(args);
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var selectedOperation = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select a MS graph operation")
            .PageSize(10)
            .AddChoices(GetOperationLabels(services))
    );

    await RunApp(services, selectedOperation);
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
}

static IHost SetupHostBuilder(string[] args)
{
    var builder = Host.CreateApplicationBuilder(args);
    RegisterServices(builder);
    return builder.Build();
}

static void RegisterServices(HostApplicationBuilder builder)
{
    var settings = Settings.LoadSettings();
    builder.Services.AddSingleton(settings);
    builder.Services.AddSingleton<IGraphApi, GraphApi>();
    builder.Services.AddSingleton<IFileSystemHelpers, FileSystemHelpers>();
    builder.Services.AddSingleton<IDataStorage, DataStorage>();
    builder.Services.AddSingleton<IOperationStrategy, GroupsOperationStrategy>();
    builder.Services.AddSingleton<IOperationStrategy, UsersOperationStrategy>();
    builder.Services.AddSingleton<GraphDownloader>();
}

static string[] GetOperationLabels(IServiceProvider services)
{
    return services
        .GetRequiredService<IEnumerable<IOperationStrategy>>()
        .Select(o => o.Operation)
        .ToArray();
}

static async Task RunApp(IServiceProvider services, string operation)
{
    var downloader = services.GetRequiredService<GraphDownloader>();
    var summary = await downloader.Download(operation);

    RenderResults(operation, summary);
}

static void RenderResults(string operation, GraphDownloaderSummary summary)
{
    AnsiConsole.MarkupLine($"[green]You selected the operation: [bold]{operation}[/][/]");
    // AnsiConsole.WriteLine($"You stored to {summary.LocationPath} - {summary.ObjectsCount} objects");

    var table = new Table();

    // Add some columns
    table.AddColumn("[bold]Location[/]");
    table.AddColumn(new TableColumn("[bold]Objects[/]").Centered());

    // Add some rows
    table.AddRow($"[green]{summary.LocationPath}[/]", $"[blue]{summary.ObjectsCount}[/]");

    // Render the table to the console
    AnsiConsole.Write(table);
}
