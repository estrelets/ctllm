namespace Common.StepResults;

/// <summary>
/// Represents the result of a processing step with a main output string
/// </summary>
public interface IStepResult
{
    /// <summary>
    /// Gets the main result string of the step
    /// </summary>
    public string Main { get; }
}