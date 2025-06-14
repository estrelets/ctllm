using Cli;
using Cli.Commands;
using Cli.Tui;
using Common;
using Common.Configuration;
using Common.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Plugins.Firecrawl;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

SerilogConfiguration.Init();

var ct = CancellationToken.None;
var services = await Init(ct);

var app = new CommandApp(new DependencyInjectionRegistrar(services));
app.SetDefaultCommand<ChatCommand>();
app.Configure(a =>
{
    a.AddCommand<AgentListCommand>("ls");
    a.AddBranch<RagSettings>("rag", rag =>
    {
        rag.AddCommand<RagLoadCommand>("load");
    });
});
return await app.RunAsync(args);


static async Task<ServiceCollection> Init(CancellationToken ct)
{
    var services = new ServiceCollection();
    services
        .AddSingleton<IUserInterface, Tui>()
        .AddCommon()
        .AddSingleton<IAgentFactory, YamlConfigParser>(sp => new YamlConfigParser(sp, "/home/estr/Documents/llm/models"))
        .AddFirecrawlPlugin()
        .AddFirecrawl("http://devserver.home:3002/v1/")
        ;

    return services;
}
