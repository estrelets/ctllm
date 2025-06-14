namespace Common.RAG.FileSystem;

public class DirectoryDocumentSource : IDocumentsSource
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public required string Pattern { get; set; }
    public required bool Recursive { get; set; }
    public required IModel Embedder { get; set; }

    public async Task<IDocument[]> Refresh(CancellationToken ct)
    {
        var directory = new DirectoryInfo(Path);
        var documents = new List<IDocument>();

        var searchOption = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        foreach (var fileInfo in directory.GetFiles(Pattern, searchOption))
        {
            var document = new FileDocument(fileInfo);
            documents.Add(document);
        }

        return documents.ToArray();
    }
}
