using Common.RAG;

namespace Common.Configuration.Yaml;

public record YamlParseContext(
    IServiceProvider ServiceProvider,
    IReadOnlyDictionary<string, string> Prompts,
    IReadOnlyDictionary<string, IModel> Models,
    IReadOnlyDictionary<string, IDocumentsSource> Documents)
{
    public string? GetPrompt(string? prompt = null, string? promptName = null)
    {
        if (prompt != null)
        {
            return prompt;
        }

        if (promptName != null)
        {
            return Prompts.GetValueOrDefault(promptName);
        }

        return null;
    }

    public IModel GetModel(string model)
    {
        return Models[model];
    }
}
