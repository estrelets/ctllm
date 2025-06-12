namespace Common.Steps;

/// <summary>
/// Step for handling chat interactions using a model
/// </summary>
public class ChatStep : IStep
{
    /// <inheritdoc />
    public required string Name { get; init; }
    
    /// <summary>
    /// Model used for chat interactions
    /// </summary>
    public required IModel Model { get; set; }
    
    /// <summary>
    /// Custom system prompt for the chat model
    /// </summary>
    public string? CustomSystemPrompt { get; set; }
    
    /// <summary>
    /// Whether to ignore the first message in the conversation
    /// </summary>
    public bool IgnoreFirstMessage { get; set; }
}