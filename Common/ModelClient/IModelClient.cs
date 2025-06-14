using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Common.ModelClient;

public interface IModelClient
{
    IEmbeddingGenerator<string, Embedding<float>> EmbeddingGenerator { get; }
    
    Task<string> Ask(string question, string? systemPrompt, CancellationToken ct);
    Task<string> GetChatAnswer(ChatHistory history, string? systemPrompt, CancellationToken ct);
    Task<float[]> EmbedAsync(string chunk, CancellationToken ct);
    Task<int> GetEmbeddingLength(CancellationToken ct);
    void AddPlugin(KernelPlugin plugin);
}