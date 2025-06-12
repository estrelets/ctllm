namespace Common.StepResults;

public record ErrorStepResult(string Error) : IStepResult
{
    /// <inheritdoc />
    public string Main => Error;
}
