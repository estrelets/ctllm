using Common.Steps;

namespace Common.Configuration.Yaml;

// ReSharper disable once UnusedType.Global
public class RephraseStepConfiguration : IStepConfiguration
{
    public required string Name { get; set; }
    public required string Model { get; set; }
    public string? Prompt { get; set; }
    public string? PromptName { get; set; }
    
    public IStep Parse(YamlParseContext context)
    {
        return new RephraseStep()
        {
            Name = Name,
            Model = context.GetModel(Model),
            CustomSystemPrompt = context.GetPrompt(Prompt, PromptName),
        };
    }
}