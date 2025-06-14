namespace Common.RAG.FileSystem;

public class FileDocumentSource : IDocumentsSource
{
    public required string Path { get; set; }
    public required string Name { get; set; }
    public required IModel Embedder { get; set; }

    public Task<IDocument[]> Refresh(CancellationToken ct)
    {
        IDocument result = new FileDocument(new FileInfo(Path));
        return Task.FromResult(new[] { result });
    }
}
