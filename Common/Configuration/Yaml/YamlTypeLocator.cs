namespace Common.Configuration.Yaml;

public static class YamlTypeLocator
{
    internal static IDictionary<string, Type> Steps { get; } = new Dictionary<string, Type>();
    internal static IDictionary<string, Type> Models { get; } = new Dictionary<string, Type>();

    public static void AddStep<TStep>(string name) where TStep : IStepConfiguration
    {
        Steps[name] = typeof(TStep);
    }
    
    public static void AddModel<TModel>(string name) where TModel : IModelConfiguration
    {
        Models[name] = typeof(TModel);
    }
}
