using Common.StepResults;

namespace Plugins.Firecrawl.StepResults;

public record ScrapeStepResult(string Markdown) : IStepResult
{
    public string Main => Markdown;
}
