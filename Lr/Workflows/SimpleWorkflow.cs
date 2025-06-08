using System.Text;
using Lr.Agents;
using Lr.Integrations;
using Lr.Integrations.FireCrawl;
using Lr.UI;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lr.Workflows;

public class SimpleWorkflow(
    ApplicationContext app,
    Agent agent,
    ChatContext chat,
    IUserInterface ui,
    FireCrawlClient fireCrawl,
    Ollama ollama
) : IWorkflow
{
    public async Task Run(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var userCommand = await ui.GetNextCommand(ct);

            switch (userCommand)
            {
                case UserCommand.Search search:
                    await Search(search.Query, ct);
                    break;
                
                case UserCommand.PickAgent pickCommand:
                    await app.SwitchAgent(pickCommand.NameQuery, ct);
                    break;
                
                case UserCommand.Chat chatCommand:
                    chat.History.AddUserMessage(chatCommand.Text);
                    var response = await ollama.ChatMessageContent(chat, ct);
                    ui.PrintMessage(AuthorRole.System,  response.Content);
                    break;
            }
        }
    }

    private async Task Search(string? query, CancellationToken ct)
    {
        // 1. Get suggestion for query
        var suggestionContext = chat.ChangePrompt(agent.Configuration.SearchPrompt);
        suggestionContext.History.AddUserMessage(query ?? "Поищи информацию для вопросов выше");
        var suggestionResponse = await ollama.ChatMessageContent(suggestionContext, ct);
        var suggestionQuery = suggestionResponse.Content;

        // 2. Ask user for acceptance
        query = ui.PromptString("Search prompt:", suggestionQuery);
        if (string.IsNullOrEmpty(query))
        {
            return;
        }

        // 3. Search
        var searchResult = await fireCrawl.Search(query, ct);
        if (!searchResult.Success)
        {
            ui.PrintMessage(AuthorRole.Tool, "Error while search");
            return;
        }

        var searchResultText = new StringBuilder();
        foreach (var datum in searchResult.Data)
        {
            searchResultText.AppendLine($"Source link: {datum.Url}");
            searchResultText.AppendLine($"Text: {datum.Markdown}");
            searchResultText.AppendLine("-------------"); 
        }

        chat.History.AddMessage(AuthorRole.Tool, searchResultText.ToString());
        var response = await ollama.ChatMessageContent(chat, ct);
        ui.PrintMessage(AuthorRole.System,  response.Content);
    }
}


