namespace Common.Steps;

/// <summary>
/// Interface representing a step in a process
/// </summary>
public interface IStep
{
    /// <summary>
    /// The name of the step
    /// </summary>
    string Name { get; }
}