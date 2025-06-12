using Common;
using Common.Runner;
using Common.StepResults;
using Plugins.Firecrawl.StepResults;
using Plugins.Firecrawl.Steps;

namespace Plugins.Firecrawl.Runners;

public class ScrapeStepRunner(HttpClient httpClient) : IStepRunner<ScrapeStep>
{
    public async Task<IStepResult> Run(StepContext context, ScrapeStep step, CancellationToken ct)
    {
        var client = new FireсrawlClient(httpClient);
        
        var foundResult = await client.Scrape(context.Last.Main, step.Format, step.OnlyMainContent, ct);
        if (!foundResult.Success || String.IsNullOrEmpty(foundResult.Data?.Markdown))
        {
            return new SearchStepFailedResult();
        }

        return new ScrapeStepResult(foundResult.Data.Markdown);
    }
}
