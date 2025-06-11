using YamlDotNet.Serialization;

namespace Common.Configuration;

public class ConfigParser
{
    private readonly IDeserializer Deserializer;
    
    public ConfigParser()
    {
        Deserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithTypeDiscriminatingNodeDeserializer(o =>
            {
                o.AddKeyValueTypeDiscriminator<IStepConfiguration>(
                    "Type",
                    new Dictionary<string, Type>()
                    {
                        ["Chat"] = typeof(ChatStepConfiguration),
                        ["Rephrase"] = typeof(RephraseStepConfiguration),
                        ["FirecrawlSearch"] = typeof(FirecrawlSearchStepConfiguration),
                        ["Print"] = typeof(PrintStepConfiguration),
                    });

                o.AddKeyValueTypeDiscriminator<IModelConfiguration>(
                    "Type",
                    new Dictionary<string, Type>()
                    {
                        ["Ollama"] = typeof(OllamaModelConfiguration)
                    });
            })
            .Build();
    }

    public Agent Parse(string yaml)
    {
        var config = Deserializer.Deserialize<AgentConfiguration>(yaml);
        var prompts = config.Prompts;
        var models = config.Models.ToDictionary(x => x.Key, v => ParseModel(prompts, v.Value));

        return new Agent()
        {
            Name = config.Name,
            Workflow = new Workflow()
            {
                Name = config.Workflow.Name,
                Steps = config.Workflow.Steps
                    .Select(step => ParseStep(prompts, models, step))
                    .ToArray()
            }
        };
    }

    public async Task<Agent[]> ParseDirectory(string directory, CancellationToken ct)
    {
        var directoryInfo = new DirectoryInfo(directory);
        var agents = new List<Agent>();
        
        foreach (var fileInfo in directoryInfo.GetFiles("*.yaml"))
        {
            try
            {
                Logger.Debug("Loading {Path} agent config", fileInfo.FullName);
                var fileContent = await File.ReadAllTextAsync(fileInfo.FullName, ct);
                var agent = Parse(fileContent);
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
    
    private IModel ParseModel(
        IReadOnlyDictionary<string, string> prompts,
        IModelConfiguration configuration)
    {
        return configuration switch
        {
            OllamaModelConfiguration ollama => new OllamaModel()
            {
                Name = ollama.Name,
                SystemPrompt = GetPrompt(prompts, ollama.Prompt, ollama.PromptName)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(configuration), configuration, null)
        };
    }

    private IStep ParseStep(
        IReadOnlyDictionary<string, string> prompts,
        IReadOnlyDictionary<string, IModel> models,
        IStepConfiguration configuration)
    {
        return configuration switch
        {
            ChatStepConfiguration chat => new ChatStep()
            {
                Name = chat.Name,
                Model = models[chat.Model],
                CustomSystemPrompt = GetPrompt(prompts, chat.Prompt, chat.PromptName)
            },
            RephraseStepConfiguration rephrase => new RephraseStep()
            {
                Name = rephrase.Name,
                Model = models[rephrase.Model],
                CustomSystemPrompt = GetPrompt(prompts, rephrase.Prompt, rephrase.PromptName)
            },
            FirecrawlSearchStepConfiguration firecrawlSearch => new FirecrawlSearchStep
            {
                Name = firecrawlSearch.Name,
                Format = firecrawlSearch.Format,
                Limit = firecrawlSearch.Limit,
                Language = firecrawlSearch.Language,
                Country = firecrawlSearch.Country,
            },
            PrintStepConfiguration => new PrintStep(),
            _ => throw new ArgumentOutOfRangeException(nameof(configuration), configuration, null)
        };
    }

    private string? GetPrompt(
        IReadOnlyDictionary<string, string> prompts,
        string? prompt = null,
        string? promptName = null)
    {
        if (prompt != null)
        {
            return prompt;
        }

        if (promptName != null)
        {
            return prompts.GetValueOrDefault(promptName);
        }

        return null;
    }
}
