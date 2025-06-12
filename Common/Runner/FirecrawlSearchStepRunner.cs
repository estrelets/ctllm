using Common.Services.FireCrawl;
using Common.StepResults;
using Common.Steps;

namespace Common.Runner;

// ReSharper disable once ClassNeverInstantiated.Global
public class FirecrawlSearchStepRunner(FireCrawlClient client) : IStepRunner<FirecrawlSearchStep>
{
    public async Task<IStepResult> Run(StepContext context, FirecrawlSearchStep step, CancellationToken ct)
    {
        var foundResult = await client.Search(context.Last.Main, step.Format, step.Limit, step.Language, step.Country, ct);
        if (!foundResult.Success || (foundResult.Data?.Length ?? 0) == 0)
        {
            return new FirecrawlSearchStepFailedResult();
        }

        return new FirecrawlSearchStepResult(foundResult.Data!);
    }
}