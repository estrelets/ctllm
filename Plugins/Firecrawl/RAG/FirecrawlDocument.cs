using Common.RAG;

namespace Plugins.Firecrawl.RAG;

public class FirecrawlDocument(FirecrawlClient client, string url, string format, bool onlyMainContent) : IDocument
{
    private string? _content;
    public string Id => url;
    public string Name { get; set; } = url;

    public IReadOnlyDictionary<string, string> Attributes { get; } = new Dictionary<string, string>();
    
    public async Task<string> GetContent(CancellationToken ct)
    {
        await Load(ct);
        return _content!;
    }

    public async Task<string> GetHash(CancellationToken ct)
    {
        await Load(ct);
        return _content!.GetHashCode().ToString();
    }

    private async Task Load(CancellationToken ct)
    {
        if (_content != null)
        {
            return;
        }

        var scrapeResult = await client.Scrape(url, format, onlyMainContent, ct);
        _content = scrapeResult.Success
            ? scrapeResult.Data!.Markdown!
            : "failed to load content";

        var title = scrapeResult.Data?.Metadata?.Title;
        if (!String.IsNullOrEmpty(title))
        {
            Name = title;
        }
    }
}
