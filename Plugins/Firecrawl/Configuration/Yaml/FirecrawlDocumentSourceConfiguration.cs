using Common.Configuration.Yaml;
using Common.RAG;
using Microsoft.Extensions.DependencyInjection;
using Plugins.Firecrawl.RAG;

namespace Plugins.Firecrawl.Configuration.Yaml;

public class FirecrawlDocumentSourceConfiguration : IDocumentSourceConfiguration
{
    public required string[] Urls { get; init; }
    public required string Format { get; set; }
    public required bool OnlyMainContent { get; set; }
    public required string Embedder { get; set; }
    
    public IDocumentsSource Parse(string key, YamlParseContext context)
    {
        var httpClientFactory = context.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient(HttpClientKeys.Documents);
        var client = new FirecrawlClient(httpClient);
        
        return new FirecrawlDocumentSource(client)
        {
            Name = key,
            Embedder = context.GetModel(Embedder),
            Urls = Urls,
            Format = Format,
            OnlyMainContent = OnlyMainContent,
        };
    }
}
