using Common.Configuration.Yaml;
using Common.Steps;
using Plugins.Firecrawl.Steps;

namespace Plugins.Firecrawl.Configuration.Yaml;

// ReSharper disable once UnusedType.Global
public class SearchStepConfiguration : IStepConfiguration
{
    public required string Name { get; set; }
    public required string Format { get; set; }
    public int Limit { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }
    
    public IStep Parse(YamlParseContext context)
    {
        return new SearchStep()
        {
            Name = Name,
            Format = Format,
            Limit = Limit,
            Language = Language,
            Country = Country,
        };
    }
}