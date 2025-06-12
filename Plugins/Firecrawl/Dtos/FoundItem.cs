using System.Text.Json.Serialization;

namespace Plugins.Firecrawl.Dtos;

public class FoundItem
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("markdown")]
    public string? Markdown { get; set; }
}