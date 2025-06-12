namespace Common.Configuration.Yaml;

public class AgentConfiguration
{
    public required string Name { get; set; }
    public required WorkflowConfiguration Workflow { get; set; }

    public required Dictionary<string, string> Prompts { get; set; }
    public required Dictionary<string, IModelConfiguration> Models { get; set; }
}