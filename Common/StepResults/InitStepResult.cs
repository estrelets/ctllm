namespace Common.StepResults;

/// <summary>
/// Step result containing initial text content
/// </summary>
/// <param name="Text">The initial text content</param>
public record InitStepResult(string Text): IStepResult
{
    /// <inheritdoc />
    public string Main => Text;
}