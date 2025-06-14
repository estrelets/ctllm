using Common.StepResults;
using Common.Steps;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Common.Runner;

// ReSharper disable once ClassNeverInstantiated.Global
public class PrintStepStepRunner(IUserInterface ui) : IStepRunner<PrintStep>
{
    public async Task<IStepResult> Run(WorkflowContext context, PrintStep step, CancellationToken ct)
    {
        await ui.ShowMessage(AuthorRole.Tool, context.Last.Main, ct);
        return new VoidStepResult();
    }
}