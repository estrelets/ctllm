using System.Text.Json.Serialization;

namespace Plugins.Firecrawl.Dtos;

public class ScrapeData
{
    [JsonPropertyName("markdown")]
    public string? Markdown { get; set; }
}