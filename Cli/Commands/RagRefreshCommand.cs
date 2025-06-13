using Common;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console.Cli;

namespace Cli.Commands;

public class RagCommandSettings : CommandSettings
{   
    [CommandOption("-a|--agent")]
    public string? Agent { get; init; }
    
    [CommandOption("-f|--force")]
    public string? Force { get; init; }
}

public class RagRefreshCommand(Agent[] agents, IUserInterface ui) : AsyncCommand<RagCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, RagCommandSettings settings)
    {
        var ct = CancellationToken.None;
        
        if (String.IsNullOrEmpty(settings.Agent))
        {
            await Refresh(agents, ct);
            return 0;
        }
        
        var agent = agents.FirstOrDefault(x => x.Name == settings.Agent);
        if (agent == null)
        {
            await ui.ShowMessage(AuthorRole.Tool, "Agent not found", ct);
            return 0;
        }

        await Refresh([agent], ct);
        return 0;
    }

    private async Task Refresh(Agent[] agents, CancellationToken ct)
    {
        foreach (var agent in agents)
        {
            if (!agent.DocumentsSources.Any())
            {
                await ui.ShowMessage(AuthorRole.Tool, $"Agent {agent.Name} has no document sources", ct);
                continue;
            }
            
            
        }
    }
}
