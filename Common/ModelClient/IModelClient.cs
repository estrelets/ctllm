using Microsoft.SemanticKernel.ChatCompletion;

namespace Common.ModelClient;

public interface IModelClient
{
    Task<string> Ask(string question, string? systemPrompt, CancellationToken ct);
    Task<string> GetChatAnswer(ChatHistory history, string? systemPrompt, CancellationToken ct);
}