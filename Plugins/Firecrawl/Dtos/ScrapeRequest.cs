using System.Text.Json.Serialization;

namespace Plugins.Firecrawl.Dtos;

public class ScrapeRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("formats")]
    public string[] Formats { get; set; }

    [JsonPropertyName("onlyMainContent")]
    public bool OnlyMainContent { get; set; }
}