namespace Common.StepResults;

public record VoidStepResult : IStepResult
{
    /// <inheritdoc />
    public string Main => String.Empty;
}