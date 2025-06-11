using Cli;
using Cli.Commands;
using Cli.Tui;
using Common;
using Common.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

SerilogConfiguration.Init();

var ct = CancellationToken.None;
var services = await InitIoc(ct);

var app = new CommandApp(new DependencyInjectionRegistrar(services));
app.SetDefaultCommand<ChatCommand>();
app.Configure(a => a.AddCommand<AgentListCommand>("ls"));
return await app.RunAsync(args);


static async Task<ServiceCollection> InitIoc(CancellationToken ct)
{
    var services = new ServiceCollection();
    var agents = await new ConfigParser()
        .ParseDirectory("/home/estr/Documents/llm/models", ct);

    services.AddSingleton<Agent[]>(agents);
    services.AddCommon();
    services.AddFireCrawlClient("http://devserver.home:3002/v1/");
    services.AddSingleton<IUserInterface, Tui>();

    return services;
}
