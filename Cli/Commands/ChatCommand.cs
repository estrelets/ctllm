using System.ComponentModel;
using System.Text;
using Common;
using Common.Runner;
using Common.StepResults;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console.Cli;

namespace Cli.Commands;

public class ChatCommandSettings : CommandSettings
{
    [Description("Text for LLM")]
    [CommandArgument(0, "[text]")]
    public string? Text { get; init; }
    
    [CommandOption("-a|--agent")]
    public string? Agent { get; init; }
}

public class ChatCommand(IUserInterface ui, AgentRunner agentRunner, Agent[] agents) : AsyncCommand<ChatCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ChatCommandSettings settings)
    {
        var agent = agents.FirstOrDefault(x => x.Name == settings.Agent);
        if (agent == null)
        {
            await ui.ShowMessage(AuthorRole.Tool, "Agent not found", default);
            return 0;
        }

        var stepContext = new StepContext();
        var message = await GenerateMessage(settings);
        if (!String.IsNullOrEmpty(message))
        {
            stepContext.Push(new InitStepResult(message));
        }
        
        await agentRunner.Run(agent, stepContext, default);
        return 0;
    }
    
    private async Task<string> GenerateMessage(ChatCommandSettings settings)
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