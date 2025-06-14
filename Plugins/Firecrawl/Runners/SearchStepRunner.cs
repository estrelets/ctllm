using Common;
using Common.Runner;
using Common.StepResults;
using Plugins.Firecrawl.StepResults;
using Plugins.Firecrawl.Steps;

namespace Plugins.Firecrawl.Runners;

// ReSharper disable once ClassNeverInstantiated.Global
public class SearchStepRunner(IHttpClientFactory httpClientFactory) : IStepRunner<SearchStep>
{
    public async Task<IStepResult> Run(WorkflowContext context, SearchStep step, CancellationToken ct)
    {
        var httpClient = httpClientFactory.CreateClient(HttpClientKeys.Search);
        var client = new FirecrawlClient(httpClient);
        
        var foundResult = await client.Search(context.Last.Main, step.Format, step.Limit, step.Language, step.Country, ct);
        if (!foundResult.Success || (foundResult.Data?.Length ?? 0) == 0)
        {
            return new SearchStepFailedResult();
        }

        return new SearchStepResult(foundResult.Data!);
    }
}