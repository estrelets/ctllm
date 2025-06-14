using System.Text.Json.Serialization;

namespace Plugins.Firecrawl.Dtos;

public class ScrapeResultMetadata
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}
