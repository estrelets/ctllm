using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OllamaSharp;
using OllamaSharp.Models;

#pragma warning disable SKEXP0070

namespace Common.ModelClient;

public class OllamaModelClient : IModelClient
{
    private readonly OllamaModel _model;
    private readonly OllamaApiClient _ollamaClient;
    private readonly OllamaPromptExecutionSettings _ollamaSettings;
    private readonly IKernelBuilder _kernelBuilder;
    
    private int? _embeddingLength;
    private Kernel _kernel;
    
    public IEmbeddingGenerator<string, Embedding<float>> EmbeddingGenerator => _ollamaClient;

    public OllamaModelClient(OllamaModel model)
    {
        _model = model;
        _ollamaSettings = new OllamaPromptExecutionSettings()
        {
            Temperature = model.Temperature,
            TopK = model.TopK,
            NumPredict = model.NumPredict,
            TopP = model.TopP,
            ModelId = model.ModelName,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };
        
        _ollamaClient = new OllamaApiClient(new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:11434"),
            Timeout = TimeSpan.FromMinutes(10)
        }, model.ModelName);

        _kernelBuilder = Kernel
            .CreateBuilder()
            .AddOllamaEmbeddingGenerator(_ollamaClient)
            .AddOllamaChatCompletion(_ollamaClient)
            .AddOllamaTextGeneration(_ollamaClient)
            ;
        
        _kernel = _kernelBuilder.Build();
    }

    public async Task<string> Ask(string question, string? systemPrompt, CancellationToken ct)
    {
        var chat = new ChatHistory();
        AddSystemPrompt(chat, systemPrompt);
        chat.AddUserMessage(question);
        
        var chatService = _kernelBuilder.Build().GetRequiredService<IChatCompletionService>();
        var response = await chatService.GetChatMessageContentAsync(
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

        var chatService = _kernel.GetRequiredService<IChatCompletionService>();
        var response = await chatService.GetChatMessageContentsAsync(historyClone, _ollamaSettings, _kernel, ct);
        foreach (var content in response)
        {
            history.Add(content);    
        }
        
        return response.First().Content ?? "Error while ask";
    }

    public async Task<float[]> EmbedAsync(string chunk, CancellationToken ct)
    {
        var request = new EmbedRequest()
        {
            Model = _model.ModelName,
            Options = new RequestOptions()
            {
                Temperature = _model.Temperature,
                TopK = _model.TopK,
                NumPredict = _model.NumPredict,
                TopP = _model.TopP
            },
            Input = [chunk]
        };
        
        var embedResult = await _ollamaClient.EmbedAsync(request, ct);
        return embedResult.Embeddings.First();
    }

    public async Task<int> GetEmbeddingLength(CancellationToken ct)
    {
        if (_embeddingLength != null)
        {
            return _embeddingLength.Value;
        }

        var modelInfo = await _ollamaClient.ShowModelAsync(_model.ModelName, ct);
        var info = modelInfo?.Info?.ExtraInfo;
        var embeddingIsNotSupported = new NotSupportedException($"Embedding for {_model.ModelName} is not supported");

        if (info == null)
        {
            throw embeddingIsNotSupported;
        }

        var embeddingKey = info.Keys.FirstOrDefault(x => x.EndsWith("embedding_length"));
        if (embeddingKey == null)
        {
            throw embeddingIsNotSupported;
        }

        if (!info.TryGetValue(embeddingKey, out var value))
        {
            throw embeddingIsNotSupported;
        }

        if (value is not JsonElement jsonElement || jsonElement.ValueKind != JsonValueKind.Number)
        {
            throw embeddingIsNotSupported;
        }

        _embeddingLength = jsonElement.GetInt32();
        if (_embeddingLength < 1)
        {
            throw embeddingIsNotSupported;
        }
        
        return _embeddingLength.Value;
    }

    public void AddPlugin(KernelPlugin plugin)
    {
        _kernelBuilder.Plugins.Add(plugin);
        _kernel = _kernelBuilder.Build();
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