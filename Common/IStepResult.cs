using System.Text;
using Common.Services.FireCrawl;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Common;

/// <summary>
/// Represents the result of a processing step with a main output string
/// </summary>
public interface IStepResult
{
    /// <summary>
    /// Gets the main result string of the step
    /// </summary>
    public string Main { get; }
}

/// <summary>
/// Stub implementation of step result with empty content
/// </summary>
public record StubStepResult() : IStepResult
{
    /// <inheritdoc />
    public string Main => String.Empty;
}

/// <summary>
/// Step result containing rephrased text content
/// </summary>
/// <param name="Rephrased">The rephrased text content</param>
public record RephraseStepResult(string Rephrased): IStepResult
{
    /// <inheritdoc />
    public string Main => Rephrased;
}

/// <summary>
/// Step result containing chat history content
/// </summary>
/// <param name="History">The chat history containing the last message</param>
public record ChatStepResult(ChatHistory History): IStepResult
{
    /// <inheritdoc />
    public string Main => History.LastOrDefault()?.Content ?? String.Empty;
}

/// <summary>
/// Step result containing initial text content
/// </summary>
/// <param name="Text">The initial text content</param>
public record InitStepResult(string Text): IStepResult
{
    /// <inheritdoc />
    public string Main => Text;
}

public record FirecrawlSearchStepResult(FireCrawlFoundItem[] Items) : IStepResult
{
    public string Main { get; } = Compile(Items);

    private static string Compile(FireCrawlFoundItem[] items)
    {
        var output = new StringBuilder();
        foreach (var item in items)
        {
            output.AppendLine($"Source link: {item.Url}");
            output.AppendLine($"Text: {item.Markdown}");
            output.AppendLine("-------------");    
        }

        return output.ToString();
    }
}

public record FirecrawlSearchStepFailedResult : IStepResult
{
    public string Main => "Nothing found";
}

public record NoOpStepResult : IStepResult
{
    public string Main => String.Empty;
}