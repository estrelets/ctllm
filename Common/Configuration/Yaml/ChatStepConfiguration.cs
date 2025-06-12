using Common.Steps;

namespace Common.Configuration.Yaml;

// ReSharper disable once UnusedType.Global
public class ChatStepConfiguration : IStepConfiguration
{
    public string Discriminator => "Chat";
    public required string Name { get; init; }
    public required string Model { get; init; }
    public string? Prompt { get; set; }
    public string? PromptName { get; set; }
    public bool IgnoreFirstMessage { get; set; }
    
    public IStep Parse(YamlParseContext context)
    {
        return new ChatStep()
        {
            Name = Name,
            Model = context.GetModel(Model),
            CustomSystemPrompt = context.GetPrompt(Prompt, PromptName),
            IgnoreFirstMessage = IgnoreFirstMessage
        };
    }
}