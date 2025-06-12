using System.Text;
using Common.StepResults;
using Plugins.Firecrawl.Dtos;

namespace Plugins.Firecrawl.StepResults;

public record SearchStepResult(FoundItem[] Items) : IStepResult
{
    public string Main { get; } = Compile(Items);

    /// <summary>
    /// Compiles FoundItem array into formatted string with source links and text content
    /// </summary>
    /// <param name="items">Array of FoundItem objects to compile</param>
    /// <returns>Formatted string containing compiled results</returns>
    private static string Compile(FoundItem[] items)
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