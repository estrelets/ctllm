using Lr.Agents;
using Lr.Agents.Configuration;
using Lr.UI;
using Microsoft.SemanticKernel.ChatCompletion;

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
        ui.PrintMessage(AuthorRole.System, agent.Description);
        await workflow.Run(ct);
    }
}
