using Common;
using Common.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Plugins.Firecrawl.Configuration.Yaml;
using Plugins.Firecrawl.Runners;
using Plugins.Firecrawl.Steps;

namespace Plugins.Firecrawl;

public static class IoC
{
    public static FirecrawlServiceCollection AddFirecrawlPlugin(this IServiceCollection services)
    {
        return new FirecrawlServiceCollection(services);
    }
}

public class FirecrawlServiceCollection
{
    private readonly IServiceCollection _services;

    internal FirecrawlServiceCollection(IServiceCollection services)
    {
        _services = services;
    }

    public FirecrawlServiceCollection AddFirecrawl(string url)
    {
        return AddFirecrawlSearch(url)
            .AddFirecrawlScrape(url)
            .AddFirecrawlDocumentsSource(url);
    }

    public FirecrawlServiceCollection AddFirecrawlSearch(string url)
    {
        YamlTypeLocator.AddStep<SearchStepConfiguration>("Firecrawl:Search");

        _services
            .AddRunner<SearchStep, SearchStepRunner>(ServiceLifetime.Scoped)
            .AddHttpClient(HttpClientKeys.Search, client =>
            {
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromMinutes(5);
            });

        return this;
    }

    public FirecrawlServiceCollection AddFirecrawlScrape(string url)
    {
        YamlTypeLocator.AddStep<ScrapeStepConfiguration>("Firecrawl:Scraper");

        _services
            .AddRunner<ScrapeStep, ScrapeStepRunner>(ServiceLifetime.Scoped)
            .AddHttpClient(HttpClientKeys.Scrape, client =>
            {
                client.BaseAddress = new Uri(url);
                client.Timeout = TimeSpan.FromMinutes(5);
            });

        return this;
    }

    public FirecrawlServiceCollection AddFirecrawlDocumentsSource(string url)
    {
        YamlTypeLocator.AddDocumentSource<FirecrawlDocumentSourceConfiguration>("Firecrawl");

        _services.AddHttpClient(HttpClientKeys.Documents, client =>
        {
            client.BaseAddress = new Uri(url);
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        return this;
    }
}
