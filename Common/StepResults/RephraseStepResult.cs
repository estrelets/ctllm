namespace Common.StepResults;

/// <summary>
/// Step result containing rephrased text content
/// </summary>
/// <param name="Rephrased">The rephrased text content</param>
public record RephraseStepResult(string Rephrased): IStepResult
{
    /// <inheritdoc />
    public string Main => Rephrased;
}