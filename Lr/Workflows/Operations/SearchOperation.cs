using System.Diagnostics;
using System.Text;
using Lr.Agents;
using Lr.Integrations;
using Lr.Integrations.FireCrawl;
using Lr.UI;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lr.Workflows.Operations;

public class SearchOperation(Ollama ollama, FireCrawlClient fireCrawl) : IOperation
{
    private abstract record GetSuggestionResult
    {
        public record UserCanceled() : GetSuggestionResult;

        public record Accepted(string Query) : GetSuggestionResult;
    }

    private abstract record SearchResult
    {
        public record Ok(string Result) : SearchResult;

        public record Error() : SearchResult;

        public record Empty() : SearchResult;
    }

    public async Task Execute(
        Agent agent,
        IWorkflow workflow,
        IUserInterface ui,
        ChatContext chat,
        string? question,
        CancellationToken ct)
    {
        Logger.Debug("Initiating search process for question: {Question}", question);
        
        if (question == null || !question.StartsWith("!"))
        {
            var getSuggestionSw = Stopwatch.StartNew();
            var getSuggestionResult = await GenerateSuggestionQuery(agent, ui, chat, question, ct);
            getSuggestionSw.Stop();
            Logger.Information("Get Suggestion Result {@GetSuggestionResult} {Elapsed}", getSuggestionResult,
                getSuggestionSw.Elapsed);
            if (getSuggestionResult is not GetSuggestionResult.Accepted suggestion)
            {
                return;
            }

            question = suggestion.Query;
        }

        var searchSw = Stopwatch.StartNew();
        var searchResult = await Search(ui, question, ct);
        searchSw.Stop();
        Logger.Information("Search result {@SearchResult} {Elapsed}", searchResult, searchSw.Elapsed);

        switch (searchResult)
        {
            case SearchResult.Empty:
                ui.PrintMessage(AuthorRole.Tool, "Nothing found");
                return;
            
            case SearchResult.Error:
                ui.PrintMessage(AuthorRole.Tool, "Error while search");
                return;
            
            case SearchResult.Ok ok:
                var processSw = Stopwatch.StartNew();
                await SummarizeSearchResult(ui, chat, ct, ok);
                processSw.Stop();
                Logger.Information("Summarize result complete {Elapsed}", processSw.Elapsed);
                return;
        }
    }

    private async Task SummarizeSearchResult(IUserInterface ui, ChatContext chat, CancellationToken ct, SearchResult.Ok ok)
    {
        Logger.Debug("Adding search results to chat history.");
        chat.History.AddMessage(AuthorRole.Tool, ok.Result);
        var response = await ollama.ChatMessageContent(chat, ct);
        Logger.Debug("Generated response from Ollama: {ResponseContent}", response.Content);
        ui.PrintMessage(AuthorRole.System, response.Content);
    }

    private async Task<SearchResult> Search(IUserInterface ui, string query, CancellationToken ct)
    {
        var searchResult = await fireCrawl.Search(query, ct);
        if (!searchResult.Success || searchResult.Data == null)
        {
            Logger.Debug("Search failed");
            ui.PrintMessage(AuthorRole.Tool, "Error while search");
            return new SearchResult.Error();
        }

        Logger.Debug("Search completed successfully. Found {ResultCount} results.", searchResult.Data.Length);

        if (searchResult.Data.Length == 0)
        {
            return new SearchResult.Empty();
        }

        var searchResultText = new StringBuilder();
        foreach (var datum in searchResult.Data)
        {
            searchResultText.AppendLine($"Source link: {datum.Url}");
            searchResultText.AppendLine($"Text: {datum.Markdown}");
            searchResultText.AppendLine("-------------");
        }

        var searchOutput = searchResultText.ToString();
        return new SearchResult.Ok(searchOutput);
    }

    private async Task<GetSuggestionResult> GenerateSuggestionQuery(Agent agent, IUserInterface ui, ChatContext chat,
        string? query,
        CancellationToken ct)
    {
        // 1. Get suggestion for query
        var getSuggestionWatch = Stopwatch.StartNew();
        var suggestionContext = chat.ChangePrompt(agent.Configuration.SearchPrompt);
        suggestionContext.History.AddUserMessage(query ?? "Поищи информацию для вопросов выше");
        var suggestionResponse = await ollama.ChatMessageContent(suggestionContext, ct);
        var suggestionQuery = suggestionResponse.Content;
        suggestionQuery = Utils.Message.CleanThinkBlock(suggestionQuery);
        getSuggestionWatch.Stop();
        Logger.Debug("Generated search suggestion: {SuggestionQuery} Elapsed {Elapsed}", suggestionQuery,
            getSuggestionWatch.Elapsed);


        // 2. Ask user for acceptance                                                                            
        query = ui.PromptString("Search prompt:", suggestionQuery);
        if (string.IsNullOrEmpty(query))
        {
            Logger.Debug("User cancelled search operation.");
            return new GetSuggestionResult.UserCanceled();
        }

        Logger.Debug("User accepted search query: {Query}", query);
        return new GetSuggestionResult.Accepted(query);
    }
}
