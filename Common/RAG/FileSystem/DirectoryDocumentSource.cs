namespace Common.RAG.FileSystem;

public class DirectoryDocumentSource : IDocumentsSource
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public required string Pattern { get; set; }
    public required bool Recursive { get; set; }

    public async Task<IDocument[]> Refresh(IDocumentLite[] existing, CancellationToken ct)
    {
        var directory = new DirectoryInfo(Path);
        var updates = new List<IDocument>();

        var searchOption = Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        foreach (var fileInfo in directory.GetFiles(Pattern, searchOption))
        {
            var content = await File.ReadAllTextAsync(fileInfo.FullName, ct);
            var document = new FileDocument(fileInfo, content);

            var notChanged = existing
                .Cast<FileDocument>()
                .Contains(document, FileDocumentEqualityComparer.Instance);
            
            if (notChanged)
            {
                continue;
            }
            
            updates.Add(document);
        }

        return updates.ToArray();
    }
}
