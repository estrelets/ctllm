namespace Lr.UI;

public abstract record UserCommand
{
    public record Search(string Query) : UserCommand;

    public record PickAgent(string? NameQuery) : UserCommand;

    public record Chat(string Text) : UserCommand;

    public record Help() : UserCommand;
}
