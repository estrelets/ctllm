namespace Common.Configuration.Yaml;

public record YamlParseContext(IReadOnlyDictionary<string, string> Prompts, IReadOnlyDictionary<string, IModel> Models)
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
