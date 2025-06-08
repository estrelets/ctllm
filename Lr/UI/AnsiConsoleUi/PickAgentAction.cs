using Lr.Agents;
using Spectre.Console;
using static Spectre.Console.AnsiConsole;

namespace Lr.UI.AnsiConsoleUi;

public class PickAgentAction
{
    public async Task<Agent> Choose(ApplicationContext context, string? query, CancellationToken ct)
    {
        var names = await context.Agents.GetAllNames(ct);
        
        var selected = Prompt(
            new SelectionPrompt<string>()
                .PageSize(10)
                .MoreChoicesText("Choose model")
                .AddChoices(SortAgentNames(names, query))
            );

        var agent = await context.Agents.GetAgent(selected, ct);
        WriteLine($"Current agent: {agent!.Configuration.Name}");
        return agent;
    }

    private static string[] SortAgentNames(string[] names, string? query)
    {
        if (String.IsNullOrEmpty(query))
        {
            return names;
        }
        
        var sorted = names
            .OrderBy(x => x == query ? 0 : 1)
            .ThenBy(x =>
            {
                var index = x.IndexOf(query, StringComparison.InvariantCultureIgnoreCase);
                return index == -1 ? 
                    int.MaxValue 
                    : index;
            });
        
        return sorted.ToArray();
    }
}