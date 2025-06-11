using Cli.Tui.Renders;
using Common;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console;

namespace Cli.Tui;

public class Tui : IUserInterface
{
    public string? PromptString(string? title = null, string? defaultValue = null, bool allowEmpty = true)
    {
        var caption = new TextPrompt<string?>(title ?? ": ").DefaultValue(defaultValue);
        caption.AllowEmpty = allowEmpty;
        return AnsiConsole.Prompt(caption);
    }

    public Task<string> GetMessage(CancellationToken ct)
    {
        while (true)
        {
            var caption = new TextPrompt<string>("> ").AllowEmpty();
            var line = AnsiConsole.Prompt(caption).Trim();
            
            if (!String.IsNullOrEmpty(line))
            {
                return Task.FromResult(line);
            }
        }
    }

    public Task ShowMessage(AuthorRole role, string message, CancellationToken ct)
    {
        AnsiConsole.Write(new MarkdownRenderable(message ?? String.Empty));
        return Task.CompletedTask;
    }
}
