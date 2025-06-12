namespace Common.StepResults;

public record FirecrawlSearchStepFailedResult : IStepResult
{
    /// <inheritdoc />
    public string Main => "Nothing found";
}