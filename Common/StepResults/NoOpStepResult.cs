namespace Common.StepResults;

public record NoOpStepResult : IStepResult
{
    /// <inheritdoc />
    public string Main => String.Empty;
}