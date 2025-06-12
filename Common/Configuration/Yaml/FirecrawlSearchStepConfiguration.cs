using Common.Steps;

namespace Common.Configuration.Yaml;

// ReSharper disable once UnusedType.Global
public class FirecrawlSearchStepConfiguration : IStepConfiguration
{
    public string Discriminator => "FirecrawlSearch";
    public required string Name { get; set; }
    public required string Format { get; set; }
    public int Limit { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }
    
    public IStep Parse(YamlParseContext context)
    {
        return new FirecrawlSearchStep()
        {
            Name = Name,
            Format = Format,
            Limit = Limit,
            Language = Language,
            Country = Country,
        };
    }
}
