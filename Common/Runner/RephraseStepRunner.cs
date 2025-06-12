using Common.ModelClient;
using Common.StepResults;
using Common.Steps;

namespace Common.Runner;

// ReSharper disable once ClassNeverInstantiated.Global
public class RephraseStepRunner(ModelAccessor accessor) : IStepRunner<RephraseStep>
{
    public async Task<IStepResult> Run(StepContext context, RephraseStep step, CancellationToken ct)
    {
        var model = accessor.Get(step.Model);
        var result = await model.Ask(context.Last.Main, step.CustomSystemPrompt, ct);
        return new RephraseStepResult(result);
    }
}