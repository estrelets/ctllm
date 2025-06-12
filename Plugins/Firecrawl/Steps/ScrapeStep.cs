using Common.Steps;

namespace Plugins.Firecrawl.Steps;

public class ScrapeStep : IStep
{
    public required string Name { get; set; }
    public required string Format { get; set; }
    public bool OnlyMainContent { get; set; }
}
