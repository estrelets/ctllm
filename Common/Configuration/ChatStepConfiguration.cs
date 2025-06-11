namespace Common.Configuration;

public class ChatStepConfiguration : IStepConfiguration
{
    public required string Name { get; init; }
    public required string Model { get; init; }
    public string? Prompt { get; set; }
    public string? PromptName { get; set; }
}

public class PrintStepConfiguration : IStepConfiguration
{
    public string Name => "Print";
}