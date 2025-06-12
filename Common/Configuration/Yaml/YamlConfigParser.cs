using System.Reflection;
using YamlDotNet.Serialization;

namespace Common.Configuration.Yaml;

public class YamlConfigParser(string configDirectory) : IAgentFactory
{
    private readonly IDeserializer _deserializer = BuildDeserializer();

    public async Task<Agent[]> Init(CancellationToken ct)
    {
        return await ParseDirectory(ct);
    }

    private async Task<Agent[]> ParseDirectory(CancellationToken ct)
    {
        var directoryInfo = new DirectoryInfo(configDirectory);
        var agents = new List<Agent>();

        foreach (var fileInfo in directoryInfo.GetFiles("*.yaml"))
        {
            try
            {
                Logger.Debug("Loading {Path} agent config", fileInfo.FullName);
                var fileContent = await File.ReadAllTextAsync(fileInfo.FullName, ct);
                var agent = ParseConfig(fileContent);
                agents.Add(agent);
                Logger.Debug("Loaded {Path} agent config", fileInfo.FullName);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Bad {Path} agent config", fileInfo.FullName);
            }
        }

        return agents.ToArray();
    }

    private Agent ParseConfig(string yaml)
    {
        var config = _deserializer.Deserialize<AgentConfiguration>(yaml);
        var prompts = config.Prompts;
        
        var context = new YamlParseContext(prompts, new Dictionary<string, IModel>());
        var models = config.Models.ToDictionary(x => x.Key, v => v.Value.Parse(context));
        context = context with { Models = models };

        return new Agent()
        {
            Name = config.Name,
            Workflow = new Workflow()
            {
                Name = config.Workflow.Name,
                Steps = config.Workflow.Steps
                    .Select(step => step.Parse(context))
                    .ToArray()
            }
        };
    }

    private static IDeserializer BuildDeserializer()
    {
        return new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithTypeDiscriminatingNodeDeserializer(o =>
            {
                o.AddKeyValueTypeDiscriminator<IStepConfiguration>(
                    "Type",
                    FindAllImplementations<IStepConfiguration>(nameof(IStepConfiguration.Discriminator))
                );
                o.AddKeyValueTypeDiscriminator<IModelConfiguration>(
                    "Type",
                    FindAllImplementations<IModelConfiguration>(nameof(IModelConfiguration.Discriminator)));
            })
            .Build();
    }

    private static Dictionary<string, Type> FindAllImplementations<TInterface>(string discriminatorPropertyName)
    {
        var interfaceType = typeof(TInterface);
        var configTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t) && !t.IsAbstract)
            .ToList();

        var configMap = new Dictionary<string, Type>();
        foreach (var type in configTypes)
        {
            var instance = Activator.CreateInstance(type);
            var discriminatorProperty = type.GetProperty(discriminatorPropertyName);
            if (discriminatorProperty != null)
            {
                var discriminatorValue = discriminatorProperty.GetValue(instance) as string;
                if (!string.IsNullOrEmpty(discriminatorValue))
                {
                    configMap[discriminatorValue] = type;
                }
            }
        }

        return configMap;
    }
}
