using Microsoft.SemanticKernel.ChatCompletion;

namespace Common.StepResults;

/// <summary>
/// Step result containing chat history content
/// </summary>
/// <param name="History">The chat history containing the last message</param>
public record ChatStepResult(ChatHistory History): IStepResult
{
    /// <inheritdoc />
    public string Main => History.LastOrDefault()?.Content ?? String.Empty;
}