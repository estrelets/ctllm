using YamlDotNet.Serialization;

namespace Common.Configuration.Yaml;

public class YamlConfigParser(string configDirectory) : IAgentFactory
{
    private readonly IDeserializer _deserializer = BuildDeserializer();

    static YamlConfigParser()
    {
        YamlTypeLocator.AddModel<OllamaModelConfiguration>("Ollama");
        YamlTypeLocator.AddStep<ChatStepConfiguration>("Chat");
        YamlTypeLocator.AddStep<PrintStepConfiguration>("Print");
        YamlTypeLocator.AddStep<RephraseStepConfiguration>("Rephrase");
    }

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
                o.AddKeyValueTypeDiscriminator<IStepConfiguration>("Type", YamlTypeLocator.Steps);
                o.AddKeyValueTypeDiscriminator<IModelConfiguration>("Type", YamlTypeLocator.Models);
            })
            .Build();
    }
}
