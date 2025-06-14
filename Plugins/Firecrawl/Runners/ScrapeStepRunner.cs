using Common;
using Common.Runner;
using Common.StepResults;
using Plugins.Firecrawl.StepResults;
using Plugins.Firecrawl.Steps;

namespace Plugins.Firecrawl.Runners;

public class ScrapeStepRunner(IHttpClientFactory httpClientFactory) : IStepRunner<ScrapeStep>
{
    public async Task<IStepResult> Run(WorkflowContext context, ScrapeStep step, CancellationToken ct)
    {
        var httpClient = httpClientFactory.CreateClient(HttpClientKeys.Scrape);
        var client = new FirecrawlClient(httpClient);
        
        var foundResult = await client.Scrape(context.Last.Main, step.Format, step.OnlyMainContent, ct);
        if (!foundResult.Success || String.IsNullOrEmpty(foundResult.Data?.Markdown))
        {
            return new SearchStepFailedResult();
        }

        return new ScrapeStepResult(foundResult.Data.Markdown);
    }
}
