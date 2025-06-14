using Common.ModelClient;
using Common.StepResults;
using Common.Steps;

namespace Common.Runner;

// ReSharper disable once ClassNeverInstantiated.Global
public class RephraseStepRunner(Agent agent) : IStepRunner<RephraseStep>
{
    public async Task<IStepResult> Run(WorkflowContext context, RephraseStep step, CancellationToken ct)
    {
        var model = agent.Models.Get(step.Model);
        var result = await model.Ask(context.Last.Main, step.CustomSystemPrompt, ct);
        return new RephraseStepResult(result);
    }
}