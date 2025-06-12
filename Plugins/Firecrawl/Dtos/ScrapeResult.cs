using System.Text.Json.Serialization;

namespace Plugins.Firecrawl.Dtos;

public class ScrapeResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public ScrapeData? Data { get; set; }
}