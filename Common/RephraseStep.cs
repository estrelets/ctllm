namespace Common;

public class RephraseStep : IStep
{
    public required string Name { get; init; }
    public required IModel Model { get; set; }
    public string? CustomSystemPrompt { get; set; }
}

public class FirecrawlSearchStep : IStep
{
    public required string Name { get; init; }
    public required string Format { get; set; }
    public int Limit { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }
}

public class PrintStep : IStep
{
    public string Name => "Print";
}