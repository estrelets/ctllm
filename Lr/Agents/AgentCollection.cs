using Lr.Agents.Configuration;

namespace Lr.Agents;

public class AgentCollection(IAgentConfigurationFactory factory)
{
    private Dictionary<string, Agent>? _agents;
    
    private async Task Init(CancellationToken ct)
    {
        if (_agents != null)
        {
            return;
        }

        var initResults = await factory.Init(ct);
        _agents = new Dictionary<string, Agent>(StringComparer.InvariantCultureIgnoreCase);
        
        foreach (var initResult in initResults)
        {
            if (initResult is InitResult.Failed failed)
            {
                Console.WriteLine($"Init failed {failed.Message}");
            }

            if (initResult is InitResult.Ok ok)
            {
                var configuration = ok.Configuration;
                var agent = new Agent(configuration);
                _agents.Add(configuration.Name, agent);

                Console.WriteLine($"Initialized {configuration.Name}");
            }
        }
    }

    public async Task<Agent?> GetAgent(string? name, CancellationToken ct)
    {
        await Init(ct);
        if (name == null)
        {
            return null;
        }
        
        return _agents!.GetValueOrDefault(name);
    }

    public async Task<string[]> GetAllNames(CancellationToken ct)
    {
        await Init(ct);
        return _agents!.Keys.ToArray();
    }
}
