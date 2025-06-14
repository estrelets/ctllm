using Common.RAG;

namespace Common;

public class OllamaModel : IModel
{
    public required string Name { get; set; }
    public required string ModelName { get; init; }
    public string? SystemPrompt { get; init; }
    public int? TopK { get; set; }
    public float? TopP { get; set; }
    public float? Temperature { get; set; }
    public int? NumPredict { get; set; }
    public IDocumentsSource[] DocumentsSources { get; set; }
}