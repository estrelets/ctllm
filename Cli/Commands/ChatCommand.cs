using System.Text;
using Common;
using Common.Configuration;
using Common.Runner;
using Common.StepResults;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console.Cli;

namespace Cli.Commands;

public class ChatCommand(IUserInterface ui, AgentRunner agentRunner, IAgentFactory agentFactory) : AsyncCommand<ChatSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ChatSettings settings)
    {
        var agents = await agentFactory.Init(default);
        var agent = agents.FirstOrDefault(x => x.Name == settings.Agent);
        if (agent == null)
        {
            await ui.ShowMessage(AuthorRole.Tool, "Agent not found", default);
            return 0;
        }

        var stepContext = new WorkflowContext();
        var message = await GenerateMessage(settings);
        if (!String.IsNullOrEmpty(message))
        {
            stepContext.Push(new InitStepResult(message));
        }
        
        await agentRunner.Run(agent, stepContext, default);
        return 0;
    }
    
    private async Task<string> GenerateMessage(ChatSettings settings)
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

        return sb.ToString().Trim();
    }
}