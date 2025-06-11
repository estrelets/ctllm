using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OllamaSharp;

#pragma warning disable SKEXP0070

namespace Common.ModelClient;

public class OllamaModelClient : IModelClient
{
    private readonly OllamaModel _model;
    private readonly OllamaPromptExecutionSettings _ollamaSettings; 
    private readonly IChatCompletionService _chatCompletionService;

    public OllamaModelClient(OllamaModel model)
    {
        _model = model;
        _ollamaSettings = new OllamaPromptExecutionSettings()
        {
            Temperature = model.Temperature,
            TopK = model.TopK,
            NumPredict = model.NumPredict,
            TopP = model.TopP,
            ModelId = model.Name
        };
        
        var ollamaClient = new OllamaApiClient(new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:11434"),
            Timeout = TimeSpan.FromMinutes(10)
        });

        var kernel = Kernel
            .CreateBuilder()
            .AddOllamaEmbeddingGenerator(ollamaClient)
            .AddOllamaChatCompletion(ollamaClient)
            .AddOllamaTextGeneration(ollamaClient)
            .Build();
        
        _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> Ask(string question, string? systemPrompt, CancellationToken ct)
    {
        var chat = new ChatHistory();
        AddSystemPrompt(chat, systemPrompt);
        chat.AddUserMessage(question);
        
        var response = await _chatCompletionService.GetChatMessageContentAsync(
            chat, 
            executionSettings: _ollamaSettings, 
            cancellationToken: ct
        );

        return response.Content ?? "Error while ask";
    }

    public async Task<string> GetChatAnswer(ChatHistory history, string? systemPrompt, CancellationToken ct)
    {
        var historyClone = new ChatHistory();
        AddSystemPrompt(historyClone, systemPrompt);

        foreach (var message in history.SkipWhile(x => x.Role == AuthorRole.System))
        {
            historyClone.Add(message);
        }

        var response = await _chatCompletionService.GetChatMessageContentAsync(
            historyClone, 
            executionSettings: _ollamaSettings, 
            cancellationToken: ct
        );

        return response.Content ?? "Error while ask";
    }

    private void AddSystemPrompt(ChatHistory history, string? systemPrompt)
    {
        systemPrompt ??= _model.SystemPrompt;
        if (systemPrompt != null)
        {
            history.AddSystemMessage(systemPrompt);
        }
    }
}