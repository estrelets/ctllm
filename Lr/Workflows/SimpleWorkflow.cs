using System.Text;
using Lr.Agents;
using Lr.Integrations;
using Lr.Integrations.FireCrawl;
using Lr.UI;
using Lr.Workflows.Operations;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lr.Workflows;

public class SimpleWorkflow(
    ApplicationContext app,
    Agent agent,
    ChatContext chat,
    IUserInterface ui,
    Ollama ollama,
    FireCrawlClient fireCrawlClient
) : IWorkflow
{
    public async Task Run(CancellationToken ct)
    {
        Logger.Debug("Workflow started. Waiting for user commands.");
        while (!ct.IsCancellationRequested)
        {
            var userCommand = await ui.GetNextCommand(ct);

            switch (userCommand)
            {
                case UserCommand.Help:
                    var helpOperation = new HelpOperation();
                    await helpOperation.Execute(agent, this, ui, chat, "", ct);
                    break;
                
                case UserCommand.Search search:
                    var searchOperation =  new SearchOperation(ollama, fireCrawlClient);
                    Logger.Debug("Processing search command with query: {Query}", search.Query);
                    await searchOperation.Execute(agent, this, ui, chat, search.Query, ct);
                    break;

                case UserCommand.PickAgent pickCommand:
                    Logger.Debug("Switching agent to: {AgentName}", pickCommand.NameQuery);
                    await app.SwitchAgent(pickCommand.NameQuery, ct);
                    break;

                case UserCommand.Chat chatCommand:
                    Logger.Debug("Received chat message: {Message}", chatCommand.Text);
                    chat.History.AddUserMessage(chatCommand.Text);
                    var response = await ollama.ChatMessageContent(chat, ct);
                    Logger.Debug("Received response from Ollama: {ResponseContent}", response.Content);
                    ui.PrintMessage(AuthorRole.System, response.Content);
                    break;
            }
        }

        Logger.Debug("Workflow cancelled.");
    }
}