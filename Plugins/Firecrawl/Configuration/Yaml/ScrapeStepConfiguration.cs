using Common.Configuration.Yaml;
using Common.Steps;
using Plugins.Firecrawl.Steps;

namespace Plugins.Firecrawl.Configuration.Yaml;

// ReSharper disable once UnusedType.Global
public class ScrapeStepConfiguration : IStepConfiguration
{
    public required string Name { get; set; }
    public required string Format { get; set; }
    public bool OnlyMainContent { get; set; }

    public IStep Parse(YamlParseContext context)
    {
        return new ScrapeStep()
        {
            Format = Format,
            Name = Name,
            OnlyMainContent = OnlyMainContent
        };
    }
}
