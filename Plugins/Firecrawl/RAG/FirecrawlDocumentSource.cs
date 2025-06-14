using Common;
using Common.RAG;

namespace Plugins.Firecrawl.RAG;

public class FirecrawlDocumentSource(FirecrawlClient client) : IDocumentsSource
{
    public required string Name { get; init; }
    public required string[] Urls { get; init; }
    public required string Format { get; set; }
    public required bool OnlyMainContent { get; set; }
    public required IModel Embedder { get; set; }

    public Task<IDocument[]> Refresh(CancellationToken ct)
    {
        IDocument[] docs = Urls
            .Select(url => new FirecrawlDocument(client, url, Format, OnlyMainContent))
            .Cast<IDocument>()
            .ToArray();
        return Task.FromResult(docs);
    }
}