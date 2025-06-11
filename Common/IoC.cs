using Common.ModelClient;
using Common.Runner;
using Common.Services.FireCrawl;
using Microsoft.Extensions.DependencyInjection;

namespace Common;

public static class IoC
{
    public static void AddCommon(this IServiceCollection services)
    {
        services.AddScoped<AgentRunner>();
        services.AddScoped<RephraseRunner>();
        services.AddScoped<FirecrawlSearchRunner>();
        services.AddScoped<PrintStepRunner>();
        services.AddScoped<ModelAccessor>();
        services.AddScoped<ChatRunner>();
    }

    public static void AddFireCrawlClient(this IServiceCollection services, string url)
    {
        services.AddSingleton<FireCrawlClient>(x => new FireCrawlClient(url));
    }
}
