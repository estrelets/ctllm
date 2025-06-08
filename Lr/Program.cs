using Lr;
using Lr.Agents.Configuration;
using Lr.Integrations.FireCrawl;
using Lr.Terminal.Commands;
using Lr.UI;
using Lr.UI.AnsiConsoleUi;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;

SerilogConfiguration.Init();
var services = InitIoc();
await Run();

async Task Run()
{
    var registrar = new DependencyInjectionRegistrar(services);
    var app = new CommandApp(registrar);
    app.SetDefaultCommand<ChatCommand>();
    await app.RunAsync(args);
}

static ServiceCollection InitIoc()
{
#pragma warning disable SKEXP0070
    var services = new ServiceCollection();

    var basePath = "/home/estr/Documents/llm";
    var modelsPath = Path.Combine(basePath, "models");

    services.AddScoped<IAgentConfigurationFactory, ConfigFromMarkdownReader>(_ =>
        new ConfigFromMarkdownReader(modelsPath));
    services.AddSingleton<ApplicationContext>();

    // AnsiConsoleUi
    services.AddScoped<IUserInterface, AnsiConsoleUi>();

    // Tools
    services.AddSingleton<FireCrawlClient>(_ => new FireCrawlClient("http://devserver.home:3002/v1/"));
    return services;
#pragma warning restore SKEXP0070
}
