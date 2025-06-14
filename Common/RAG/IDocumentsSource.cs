namespace Common.RAG;

public interface IDocumentsSource
{
    string Name { get; }
    IModel Embedder { get; }
    Task<IDocument[]> Refresh(CancellationToken ct);
}