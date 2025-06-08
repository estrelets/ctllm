#pragma warning disable SKEXP0070
using System.Text;
using Lr.Agents;
using Lr.Integrations.FireCrawl;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.AI.Ollama;
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
