namespace Common.StepResults;

/// <summary>
/// Stub implementation of step result with empty content
/// </summary>
public record StubStepResult() : IStepResult
{
    /// <inheritdoc />
    public string Main => String.Empty;
}