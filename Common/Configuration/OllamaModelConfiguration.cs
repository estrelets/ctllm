namespace Common.Configuration;

public class OllamaModelConfiguration : IModelConfiguration
{
    public required string Name { get; set; }
    public string? Prompt { get; set; }
    public string? PromptName { get; set; }
    
    public int? TopK { get; set; }
    public float? TopP { get; set; }
    public float? Temperature { get; set; }
    public int? NumPredict { get; set; }
}