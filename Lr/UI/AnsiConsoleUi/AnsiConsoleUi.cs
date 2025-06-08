using Lr.Agents;
using Lr.UI.AnsiConsoleUi.Renders;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console;
using static Spectre.Console.AnsiConsole;

namespace Lr.UI.AnsiConsoleUi;

public class AnsiConsoleUi : IUserInterface
{
    public Task<UserCommand> GetNextCommand(CancellationToken ct)
    {
        return Task.FromResult(GetNextSync());
    }
    
    public void PrintMessage(AuthorRole role, string? message)
    {
        Write(new MarkdownRenderable(message ?? String.Empty));
    }

    public async Task<Agent> PickAgent(ApplicationContext context, string? query, CancellationToken ct)
    {
        var action = new PickAgentAction();
        return await action.Choose(context, query, ct);
    }

    public string? PromptString(string? title = null, string? defaultValue = null, bool allowEmpty = true)
    {
        var caption = new TextPrompt<string?>(title ?? ": ").DefaultValue(defaultValue);
        caption.AllowEmpty = allowEmpty;
        return Prompt(caption);
    }

    private UserCommand GetNextSync()
    {
        var caption = new TextPrompt<string>("> ").AllowEmpty();
        var line = Prompt(caption).Trim();
        
        if (String.IsNullOrEmpty(line))
        {
            return GetNextSync();
        }

        if (line.StartsWith("@"))
        {
            var agentQuery = line.Length == 1
                ? null
                : line.Substring(1);

            return new UserCommand.PickAgent(agentQuery);
        }

        if (line.StartsWith(":") && line.Length > 1)
        {
            var searchQuery = line.Substring(1);
            return new UserCommand.Search(searchQuery);
        }

        return new UserCommand.Chat(line);
    }
}
