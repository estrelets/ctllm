namespace Common;

public class ChatStep : IStep
{
    public required string Name { get; init; }
    public required IModel Model { get; set; }
    public string? CustomSystemPrompt { get; set; }
}