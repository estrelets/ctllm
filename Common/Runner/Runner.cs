using System.Diagnostics;
using Common.ModelClient;

namespace Common.Runner;

public class AgentRunner(
    ChatRunner chatRunner, 
    RephraseRunner rephraseRunner, 
    FirecrawlSearchRunner firecrawlSearchRunner,
    PrintStepRunner printStepRunner)
{
    public async Task Run(Agent agent, StepContext stepContext, CancellationToken ct)
    {
        Logger.Debug("Start agent {Agent}", agent);
        Logger.Debug("Start workflow {Workflow}", agent.Workflow);

        foreach (var step in agent.Workflow.Steps)
        {
            Logger.Debug("Start execute step {@Step}", step);
            var sw = Stopwatch.StartNew();
            try
            {
                var result = await ExecuteStep(stepContext, step, ct);
                if (result is NoOpStepResult)
                {
                    continue;
                }
                
                stepContext.Push(result);
                Logger.Debug("Finished execute step {Step} with result {@Result}", step.Name, result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while executing step {Step}", step.Name);
                throw;
            }
            finally
            {
                sw.Stop();
                Logger.Debug("Finished execute step {Step} {Elapsed}", step.Name, sw.Elapsed);
            }
        }

        Logger.Debug("Finished workflow {Workflow}", agent.Workflow);
    }

    private async Task<IStepResult> ExecuteStep(StepContext stepContext, IStep step, CancellationToken ct)
    {
        switch (step)
        {
            case RephraseStep rephraseStep:
                return await rephraseRunner.Run(stepContext, rephraseStep, ct);

            case ChatStep chatStep:
                return await chatRunner.Run(stepContext, chatStep, ct);
            
            case FirecrawlSearchStep firecrawlSearchStep:
                return await firecrawlSearchRunner.Run(stepContext, firecrawlSearchStep, ct);
            
            case PrintStep printStep:
                return await printStepRunner.Run(stepContext, printStep, ct);
            
            default:
                throw new NotImplementedException();
        }
    }
}
