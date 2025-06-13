using Cli;
using Cli.Commands;
using Cli.Tui;
using Common;
using Common.Configuration.Yaml;
using Common.RAG;
using Common.RAG.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using Plugins.Firecrawl;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Telegram.Bot.Types;

SerilogConfiguration.Init();

// var ds = new DirectoryDocumentSource()
// {
//     Path = "/home/estr/Documents/llm/RAG",
//     Name = "DS"
// };
// var documents = await ds.Refresh([], CancellationToken.None);
// var light = documents.Cast<FileDocument>().Cast<IDocumentLite>().ToArray();
// var documents2 = await ds.Refresh(light, CancellationToken.None);

var ct = CancellationToken.None;
var services = await Init(ct);

var app = new CommandApp(new DependencyInjectionRegistrar(services));
app.SetDefaultCommand<ChatCommand>();
app.Configure(a => a.AddCommand<AgentListCommand>("ls"));
return await app.RunAsync(args);


static async Task<ServiceCollection> Init(CancellationToken ct)
{
    var services = new ServiceCollection();
    services
        .AddSingleton<IUserInterface, Tui>()
        .AddCommon()
        .AddFirecrawlPlugin()
        .AddFirecrawl("http://devserver.home:3002/v1/");
    
    var agentsFactory = new YamlConfigParser("/home/estr/Documents/llm/models");
    var agents = await agentsFactory.Init(ct);
    services.AddSingleton<Agent[]>(agents);

    return services;
}
