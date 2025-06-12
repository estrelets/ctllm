namespace Common.Configuration.Yaml;

// ReSharper disable once UnusedType.Global
public class OllamaModelConfiguration : IModelConfiguration
{
    public required string Name { get; set; }
    public string? Prompt { get; set; }
    public string? PromptName { get; set; }
    public int? TopK { get; set; }
    public float? TopP { get; set; }
    public float? Temperature { get; set; }
    public int? NumPredict { get; set; }
    public string Discriminator => "Ollama";
    
    public IModel Parse(YamlParseContext context)
    {
        return new OllamaModel()
        {
            Name = Name,
            SystemPrompt = context.GetPrompt(Prompt, PromptName),
            NumPredict = NumPredict,
            Temperature = Temperature,
            TopK = TopK,
            TopP = TopP
        };
    }
}