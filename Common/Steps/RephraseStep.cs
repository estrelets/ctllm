namespace Common.Steps;

/// <summary>
/// Step for rephrasing text using a model
/// </summary>
public class RephraseStep : IStep
{
    /// <inheritdoc />
    public required string Name { get; init; }
    
    /// <summary>
    /// Model used for rephrasing
    /// </summary>
    public required IModel Model { get; set; }
    
    /// <summary>
    /// Custom system prompt for the model
    /// </summary>
    public string? CustomSystemPrompt { get; set; }
}