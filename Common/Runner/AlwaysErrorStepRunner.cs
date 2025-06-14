using Common.StepResults;
using Common.Steps;

namespace Common.Runner;

internal class AlwaysErrorStepRunner<T>(string error) : IStepRunner<T> where T : IStep
{
    public Task<IStepResult> Run(WorkflowContext context, T step, CancellationToken ct)
    {
        return Task.FromResult<IStepResult>(new ErrorStepResult(error));
    }
}
