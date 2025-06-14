namespace Common.RAG;

public interface IDocument
{
    string Id { get; }
    string Name { get; }
    
    IReadOnlyDictionary<string, string> Attributes { get; }
    Task<string> GetContent(CancellationToken ct);
    Task<string> GetHash(CancellationToken ct);
}
