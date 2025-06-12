using System.Text.Json.Serialization;

namespace Plugins.Firecrawl.Dtos;

public class ScrapeOptions
{
    [JsonPropertyName("formats")]
    public string[] Formats { get; set; }
}