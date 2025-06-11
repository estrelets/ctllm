namespace Common.Configuration;

public interface IStepConfiguration
{
    string Name { get; }
}

public class RephraseStepConfiguration : IStepConfiguration
{
    public required string Name { get; set; }
    public required string Model { get; set; }
    public string? Prompt { get; set; }
    public string? PromptName { get; set; }
}
