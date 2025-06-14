namespace Common.Configuration.Yaml;

// ReSharper disable once UnusedType.Global
public class OllamaModelConfiguration : IModelConfiguration
{
    public string Name { get; set; }
    public string? Prompt { get; set; }
    public string? PromptName { get; set; }
    public int? TopK { get; set; }
    public float? TopP { get; set; }
    public float? Temperature { get; set; }
    public int? NumPredict { get; set; }
    public string[]? Documents { get; set; }
    
    public IModel Parse(string key, YamlParseContext context)
    {
        return new OllamaModel()
        {
            Name = key,
            ModelName = Name,
            SystemPrompt = context.GetPrompt(Prompt, PromptName),
            NumPredict = NumPredict,
            Temperature = Temperature,
            TopK = TopK,
            TopP = TopP,
            DocumentsSources = [] // Parsing later
        };
    }

    public void BindDocuments(IModel model, YamlParseContext context)
    {
        if (model is OllamaModel ollamaModel)
        {
            ollamaModel.DocumentsSources = Documents == null
                ? []
                : Documents
                    .Select(name => context.Documents[name])
                    .ToArray();
        }
    }
}