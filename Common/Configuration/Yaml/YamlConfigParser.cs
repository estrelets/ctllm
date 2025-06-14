using Common.RAG;
using YamlDotNet.Serialization;

namespace Common.Configuration.Yaml;

public class YamlConfigParser(IServiceProvider sp, string configDirectory) : IAgentFactory
{
    private readonly IDeserializer _deserializer = BuildDeserializer();
    private Agent[]? _cached;

    static YamlConfigParser()
    {
        YamlTypeLocator.AddModel<OllamaModelConfiguration>("Ollama");
        YamlTypeLocator.AddStep<ChatStepConfiguration>("Chat");
        YamlTypeLocator.AddStep<PrintStepConfiguration>("Print");
        YamlTypeLocator.AddStep<RephraseStepConfiguration>("Rephrase");
        YamlTypeLocator.AddDocumentSource<DirectoryDocumentSourceConfiguration>("Directory");
        YamlTypeLocator.AddDocumentSource<FileDocumentSourceConfiguration>("File");
    }
    
    private static IDeserializer BuildDeserializer()
    {
        return new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithTypeDiscriminatingNodeDeserializer(o =>
            {
                o.AddKeyValueTypeDiscriminator<IStepConfiguration>("Type", YamlTypeLocator.Steps);
                o.AddKeyValueTypeDiscriminator<IModelConfiguration>("Type", YamlTypeLocator.Models);
                o.AddKeyValueTypeDiscriminator<IDocumentSourceConfiguration>("Type", YamlTypeLocator.DocumentSources);
            })
            .Build();
    }

    public async Task<Agent[]> Init(CancellationToken ct)
    {
        return _cached ??= await ParseDirectory(ct);
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

        var context = new YamlParseContext(
            sp,
            config.Prompts, 
            new Dictionary<string, IModel>(),
            new Dictionary<string, IDocumentsSource>());

        context = ParseModels(config, context);
        context = ParseSources(config, context);
        BindDocuments(config, context);

        return new Agent(context.Models.Values)
        {
            Name = config.Name,
            Workflow = ParseWorkflow(config, context),
            DocumentsSources = context.Documents.Values.ToArray()
        };
    }

    private static YamlParseContext ParseSources(AgentConfiguration config, YamlParseContext context)
    {
        if (config.Documents == null)
        {
            return context;
        }
        
        var documentsSources = config.Documents.ToDictionary(x => x.Key, kvp => kvp.Value.Parse(kvp.Key, context));
        return context with { Documents = documentsSources };
    }
    
    private static YamlParseContext ParseModels(AgentConfiguration config, YamlParseContext context)
    {
        var models = config.Models.ToDictionary(x => x.Key, kvp => kvp.Value.Parse(kvp.Key, context));
        return context with { Models = models };
    }

    private static void BindDocuments(AgentConfiguration config, YamlParseContext context)
    {
        var pairs = config.Models.Values.Zip(context.Models.Values);
        foreach (var modelConfig in pairs)
        {
            modelConfig.First.BindDocuments(modelConfig.Second, context);
        }
    }

    private static Workflow ParseWorkflow(AgentConfiguration config, YamlParseContext context)
    {
        return new Workflow()
        {
            Name = config.Workflow.Name,
            Steps = config.Workflow.Steps
                .Select(step => step.Parse(context))
                .ToArray()
        };
    }
}
