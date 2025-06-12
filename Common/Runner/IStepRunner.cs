using Common.StepResults;
using Common.Steps;

namespace Common.Runner;

public interface IStepRunner<in TStep> : IStepRunner where TStep : IStep
{
    Task<IStepResult> Run(StepContext context, TStep step, CancellationToken ct);

    Task<IStepResult> IStepRunner.Run(StepContext context, IStep step, CancellationToken ct)
    {
        return Run(context,(TStep) step, ct);
    }
}

public interface IStepRunner
{
    Task<IStepResult> Run(StepContext context, IStep step, CancellationToken ct);
}
