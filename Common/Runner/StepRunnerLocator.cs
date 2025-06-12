using Microsoft.Extensions.DependencyInjection;

namespace Common.Runner;

public class StepRunnerLocator(Dictionary<Type, Type> stepToRunnerMap)
{
    public IStepRunner Create(IServiceProvider sp, Type stepType)
    {
        var runnerType = stepToRunnerMap[stepType];
        return (IStepRunner)sp.GetRequiredService(runnerType);
    }
}
