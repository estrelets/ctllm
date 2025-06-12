using System.Diagnostics;
using Common.StepResults;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Runner;

public class AgentRunner(StepRunnerLocator runnerLocator, IServiceScopeFactory scopeFactory)
{
    public async Task Run(Agent agent, StepContext stepContext, CancellationToken ct)
    {
        Logger.Debug("Start agent {Agent}", agent);
        Logger.Debug("Start workflow {Workflow}", agent.Workflow);

        foreach (var step in agent.Workflow.Steps)
        {
            Logger.Debug("Start execute step {@Step}", step);
            var sw = Stopwatch.StartNew();
            using var stepScope = scopeFactory.CreateScope();
            
            try
            {
                var runner = runnerLocator.Create(stepScope.ServiceProvider, step.GetType());
                var result = await runner.Run(stepContext, step, ct);
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
}
