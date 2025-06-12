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
        YamlTypeLocator.AddStep<ScrapeStepConfiguration>("Firecrawl:Scraper");
        YamlTypeLocator.AddStep<SearchStepConfiguration>("Firecrawl:Search");

        return new FirecrawlServiceCollection(
            services
                .AddAlwaysErrorRunner<SearchStep>("Firecrawl is not configured")
                .AddAlwaysErrorRunner<ScrapeStep>("Firecrawl is not configured"));
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
        return AddFirecrawlSearch(url).AddFirecrawlScrape(url);
    }

    public FirecrawlServiceCollection AddFirecrawlSearch(string url)
    {
        _services
            .AddRunner<SearchStep, SearchStepRunner>(ServiceLifetime.Scoped)
            .AddHttpClient<SearchStepRunner>(o =>
            {
                o.BaseAddress = new Uri(url);
                o.Timeout = TimeSpan.FromMinutes(5);
            });
        
        return this;
    }

    public FirecrawlServiceCollection AddFirecrawlScrape(string url)
    {
        _services
            .AddRunner<ScrapeStep, ScrapeStepRunner>(ServiceLifetime.Scoped)
            .AddHttpClient<ScrapeStepRunner>(o =>
            {
                o.BaseAddress = new Uri(url);
                o.Timeout = TimeSpan.FromMinutes(5);
            });

        return this;
    }
}
