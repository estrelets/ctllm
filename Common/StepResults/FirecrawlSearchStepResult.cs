using System.Text;
using Common.Services.FireCrawl;

namespace Common.StepResults;

public record FirecrawlSearchStepResult(FireCrawlFoundItem[] Items) : IStepResult
{
    public string Main { get; } = Compile(Items);

    /// <summary>
    /// Compiles FireCrawlFoundItem array into formatted string with source links and text content
    /// </summary>
    /// <param name="items">Array of FireCrawlFoundItem objects to compile</param>
    /// <returns>Formatted string containing compiled results</returns>
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