using Common.RAG;

namespace Common;

public interface IModel
{
    string Name { get; set; }
    string ModelName { get; }
    string? SystemPrompt { get; }
    IDocumentsSource[] DocumentsSources { get; }
}
