using Lr.Agents;
using Lr.Agents.Configuration;
using Lr.UI;

namespace Lr;

public class ApplicationContext(
    IAgentConfigurationFactory agentConfigurationFactory,
    IServiceProvider sp,
    IUserInterface ui)
{
    public AgentCollection Agents { get; } = new(agentConfigurationFactory);

    public async Task SwitchAgent(string? query, CancellationToken ct)
    {
        var agent = await ui.PickAgent(this, query, ct);
        var workflow = await agent.GetWorkflow(sp);
        await workflow.Run(ct);
    }
}
