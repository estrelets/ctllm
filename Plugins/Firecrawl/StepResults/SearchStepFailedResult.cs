using Common.StepResults;

namespace Plugins.Firecrawl.StepResults;

public record SearchStepFailedResult : IStepResult
{
    /// <inheritdoc />
    public string Main => "Nothing found";
}