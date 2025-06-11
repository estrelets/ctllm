using Common;
using Spectre.Console.Cli;

namespace Cli.Commands;

public class AgentListCommand(Agent[] agents): AsyncCommand
{
    public override Task<int> ExecuteAsync(CommandContext context)
    {
        foreach (var agent in agents)
        {
            Console.WriteLine($"{agent.Name}");
        }

        return Task.FromResult(0);
    }
}
