using Common;
using Common.Configuration;
using Spectre.Console.Cli;

namespace Cli.Commands;

public class AgentListCommand(IAgentFactory agentFactory): AsyncCommand
{
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var agents = await agentFactory.Init(default);
        foreach (var agent in agents)
        {
            Console.WriteLine($"{agent.Name}");
        }

        return 0;
    }
}
