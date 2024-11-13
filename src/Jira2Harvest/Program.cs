using Jira2Harvest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<SyncService>();
builder.Services.AddSingleton<HarvestClient>();
builder.Services.AddSingleton<JiraClient>();
builder.Services.AddSingleton<ConsoleService>();


using IHost host = builder.Build();

await Start(host.Services);

//await host.RunAsync();

static async Task Start(IServiceProvider hostServices)
{
    var syncService = hostServices.GetRequiredService<SyncService>();

    await syncService.CheckSettings();
    await syncService.SyncAll();

    Console.WriteLine("Done");

    Console.ReadLine();
}