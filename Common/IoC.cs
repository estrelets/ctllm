using Common.ModelClient;
using Common.Runner;
using Common.Services.FireCrawl;
using Common.Steps;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class IoC
{
    private readonly static Dictionary<Type, Type> StepToRunnerMap = new();
    
    public static void AddCommon(this IServiceCollection services)
    {
        services.AddScoped<AgentRunner>();
        
        services.AddScoped<ModelAccessor>();
        services.AddScoped<ChatStepStepRunner>();
        
        services.AddRunner<ChatStep, ChatStepStepRunner>(ServiceLifetime.Scoped);
        services.AddRunner<RephraseStep, RephraseStepRunner>(ServiceLifetime.Scoped);
        services.AddRunner<FirecrawlSearchStep, FirecrawlSearchStepRunner>(ServiceLifetime.Scoped);
        services.AddRunner<PrintStep, PrintStepStepRunner>(ServiceLifetime.Scoped);
    }

    public static void AddRunner<TStep, TRunner>(this IServiceCollection services, ServiceLifetime lifetime)
        where TStep : IStep
        where TRunner : IStepRunner<TStep>
    {
        if (!services.Any(x => x.ServiceType == typeof(StepRunnerLocator)))
        {
            services.AddSingleton<StepRunnerLocator>(_ => new StepRunnerLocator(StepToRunnerMap));
        }
        
        services.Add(new ServiceDescriptor(typeof(TRunner), typeof(TRunner), lifetime));
        StepToRunnerMap[typeof(TStep)] = typeof(TRunner);
    }

    public static void AddFireCrawlClient(this IServiceCollection services, string url)
    {
        services.AddSingleton<FireCrawlClient>(x => new FireCrawlClient(url));
    }
}
