using System.Diagnostics;
using Common.RAG;
using Common.StepResults;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Runner;

public class AgentRunner(StepRunnerLocator runnerLocator, IDocumentVectorizer vectorizer, IServiceScopeFactory scopeFactory)
{
    public async Task Run(Agent agent, WorkflowContext workflowContext, CancellationToken ct)
    {
        Logger.Debug("Start agent {Agent}", agent);
        Logger.Debug("Start workflow {Workflow}", agent.Workflow);

        await agent.Models.Init(vectorizer, ct);

        foreach (var step in agent.Workflow.Steps)
        {
            Logger.Debug("Start execute step {@Step}", step);
            var sw = Stopwatch.StartNew();
            using var stepScope = scopeFactory.CreateScope();
            
            try
            {
                var runner = runnerLocator.Create(stepScope.ServiceProvider, agent, step.GetType());
                var result = await runner.Run(workflowContext, step, ct);
                if (result is VoidStepResult)
                {
                    continue;
                }
                
                workflowContext.Push(result);
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
