using System.Text;
using Lr.UI;
using Spectre.Console.Cli;

namespace Lr.Terminal.Commands;

public class ChatCommand(ApplicationContext app, IUserInterface ui, IServiceProvider sp) 
    : AsyncCommand<SendMessageSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, SendMessageSettings settings)
    {
        Logger.Information("{@Settings}", settings);
        var ct = CancellationToken.None;
        

        var agent = await app.Agents.GetAgent(settings.Agent, ct)
            ?? await ui.PickAgent(app, null, ct);

        var workflow = await agent.GetWorkflow(sp);
        await SendMessage(settings);
        await workflow.Run(ct);

        return 1;
    }

    private async Task SendMessage(SendMessageSettings settings)
    {
        var sb = new StringBuilder();
        sb.AppendLine(settings.Text ?? "");
        
        if (Console.IsInputRedirected)
        {
            sb.AppendLine("```");
            using var sr = new StreamReader(Console.OpenStandardInput());
            var stdIn = await sr.ReadToEndAsync(default);
            sb.AppendLine(stdIn);
            sb.AppendLine("```");
        }

        var message = sb.ToString().Trim();
        if (!String.IsNullOrEmpty(message))
        {
            ui.PushCommand(new UserCommand.Chat(message));
        }
    }
}
