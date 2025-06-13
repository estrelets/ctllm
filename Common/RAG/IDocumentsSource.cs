namespace Common.RAG;

public interface IDocumentsSource
{
    string Name { get; }
    Task<IDocument[]> Refresh(IDocumentLite[] existing, CancellationToken ct);
}

public interface IDocument
{
    string Id { get; }
    string Name { get; }
    string Content { get; }
    IReadOnlyDictionary<string, string> Attributes { get; }
}

public interface IDocumentLite
{
    string Id { get; }
    string Name { get; }
    IReadOnlyDictionary<string, string> Attributes { get; }
}