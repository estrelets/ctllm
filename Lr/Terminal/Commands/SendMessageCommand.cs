using Lr.UI;
using Spectre.Console.Cli;

namespace Lr.Terminal.Commands;

public class SendMessageCommand(ApplicationContext app, IUserInterface ui, IServiceProvider sp) 
    : AsyncCommand<SendMessageSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, SendMessageSettings settings)
    {
        var ct = CancellationToken.None;

        var agent = await app.Agents.GetAgent(settings.Agent, ct)
            ?? await ui.PickAgent(app, null, ct);

        var workflow = await agent.GetWorkflow(sp);
        await workflow.Run(ct);

        return 1;
    }
}
