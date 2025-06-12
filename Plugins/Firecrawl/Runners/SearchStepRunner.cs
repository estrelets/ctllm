using Common;
using Common.Runner;
using Common.StepResults;
using Plugins.Firecrawl.StepResults;
using Plugins.Firecrawl.Steps;

namespace Plugins.Firecrawl.Runners;

// ReSharper disable once ClassNeverInstantiated.Global
public class SearchStepRunner(HttpClient httpClient) : IStepRunner<SearchStep>
{
    public async Task<IStepResult> Run(StepContext context, SearchStep step, CancellationToken ct)
    {
        var client = new FireсrawlClient(httpClient);
        
        var foundResult = await client.Search(context.Last.Main, step.Format, step.Limit, step.Language, step.Country, ct);
        if (!foundResult.Success || (foundResult.Data?.Length ?? 0) == 0)
        {
            return new SearchStepFailedResult();
        }

        return new SearchStepResult(foundResult.Data!);
    }
}