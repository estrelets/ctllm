using Common;
using Common.Configuration;
using Common.RAG;
using Microsoft.SemanticKernel.ChatCompletion;
using Spectre.Console.Cli;

namespace Cli.Commands;

public class RagLoadCommand(IAgentFactory agentFactory, IDocumentVectorizer vectorizer, IUserInterface ui)
    : AsyncCommand<RagSettings.Load>
{
    public override async Task<int> ExecuteAsync(CommandContext context, RagSettings.Load settings)
    {
        var agents = await agentFactory.Init(default);
        var ct = CancellationToken.None;

        if (String.IsNullOrEmpty(settings.Agent))
        {
            await Refresh(agents, ct);
            return 0;
        }

        var agent = agents.FirstOrDefault(x => x.Name == settings.Agent);
        if (agent == null)
        {
            await ui.ShowMessage(AuthorRole.Tool, "Agent not found", ct);
            return 0;
        }

        await Refresh([agent], ct);
        return 0;
    }

    private async Task Refresh(Agent[] agents, CancellationToken ct)
    {
        foreach (var agent in agents)
        {
            if (!agent.DocumentsSources.Any())
            {
                await ui.ShowMessage(AuthorRole.Tool, $"Agent {agent.Name} has no document sources", ct);
                continue;
            }

            foreach (var documentsSource in agent.DocumentsSources)
            {
                var embedder = agent.Models.Get(documentsSource.Embedder);

                await ui.ShowMessage(AuthorRole.Tool,
                    $"Start learning " +
                    $"document {documentsSource.Name} " +
                    $"for {agent.Name} " +
                    $"using {documentsSource.Embedder.ModelName}", ct);
                
                await vectorizer.Load(embedder, documentsSource, ct);

                await ui.ShowMessage(AuthorRole.Tool, $"Finished learning", ct);
            }
        }
    }
}
