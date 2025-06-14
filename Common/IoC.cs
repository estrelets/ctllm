using Common.ModelClient;
using Common.RAG;
using Common.RAG.SqliteVectorizer;
using Common.Runner;
using Common.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class IoC
{
    private readonly static Dictionary<Type, Type> StepToRunnerMap = new();

    public static IServiceCollection AddCommon(this IServiceCollection services)
    {
        return services
            .AddScoped<AgentRunner>()
            .AddRunner<ChatStep, ChatStepStepRunner>(ServiceLifetime.Scoped)
            .AddRunner<RephraseStep, RephraseStepRunner>(ServiceLifetime.Scoped)
            .AddRunner<PrintStep, PrintStepStepRunner>(ServiceLifetime.Scoped)
            .AddSingleton<IDocumentVectorizer, DocumentVectorizer>(sp => ActivatorUtilities.CreateInstance<DocumentVectorizer>(sp, "/home/estr/Documents/llm/vectors"))
            ;
    }
    
    public static IServiceCollection AddAlwaysErrorRunner<TStep>(this IServiceCollection services, string message)
        where TStep : IStep
    {
        var runner = new AlwaysErrorStepRunner<TStep>(message);
        var descriptor = new ServiceDescriptor(runner.GetType(), runner);
        return services.AddRunner<TStep, AlwaysErrorStepRunner<TStep>>(descriptor);
    }

    public static IServiceCollection AddRunner<TStep, TRunner>(this IServiceCollection services,
        ServiceLifetime lifetime)
        where TStep : IStep
        where TRunner : IStepRunner<TStep>
    {
        var descriptor = new ServiceDescriptor(typeof(TRunner), typeof(TRunner), lifetime);
        return services.AddRunner<TStep, TRunner>(descriptor);
    }

    public static IServiceCollection AddRunner<TStep, TRunner>(this IServiceCollection services, TRunner instance)
        where TStep : IStep
        where TRunner : IStepRunner<TStep>
    {
        var descriptor = new ServiceDescriptor(typeof(TRunner), typeof(TRunner), instance);
        return services.AddRunner<TStep, TRunner>(descriptor);
    }

    public static IServiceCollection AddRunner<TStep, TRunner>(this IServiceCollection services,
        ServiceDescriptor descriptor)
        where TStep : IStep
        where TRunner : IStepRunner<TStep>
    {
        if (!services.Any(x => x.ServiceType == typeof(StepRunnerLocator)))
        {
            services.AddSingleton<StepRunnerLocator>(_ => new StepRunnerLocator(StepToRunnerMap));
        }

        if (StepToRunnerMap.TryGetValue(typeof(TStep), out var existingRunnerType))
        {
            var existingRunnerDescriptor = services.FirstOrDefault(x => x.ServiceType == existingRunnerType);
            if (existingRunnerDescriptor != null)
            {
                services.Remove(existingRunnerDescriptor);
            }
        }

        StepToRunnerMap[typeof(TStep)] = typeof(TRunner);
        services.Add(descriptor);
        return services;
    }
}
