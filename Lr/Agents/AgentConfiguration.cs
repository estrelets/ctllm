#pragma warning disable SKEXP0070
using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace Lr.Agents;

public class AgentConfiguration
{
    public required string Name { get; init; }
    public required string Prompt { get; init; }
    public required string SearchPrompt { get; init; }
    public required string ModelName { get; set; }
    public required string Workflow { get; init; }
    public required OllamaPromptExecutionSettings Parameters { get; init; }
}
