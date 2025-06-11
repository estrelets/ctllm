namespace Common;

public interface IModel
{
    string Name { get; }
    string? SystemPrompt { get; }
}