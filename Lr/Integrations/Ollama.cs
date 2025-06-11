#pragma warning disable SKEXP0070
using Lr.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace Lr.Integrations;

public class Ollama(Kernel kernel, OllamaPromptExecutionSettings configurationParameters)
{
    private readonly IChatCompletionService _chat = kernel.GetRequiredService<IChatCompletionService>();
    
    public async Task<ChatMessageContent> ChatMessageContent(ChatContext chat, CancellationToken ct)
    {
        return await _chat.GetChatMessageContentAsync(
            chat.History, 
            executionSettings: configurationParameters, 
            cancellationToken: ct
            );
    }
}
