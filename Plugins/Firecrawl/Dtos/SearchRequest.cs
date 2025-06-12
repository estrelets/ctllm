using System.Text.Json.Serialization;

namespace Plugins.Firecrawl.Dtos;

public class SearchRequest
{
    [JsonPropertyName("query")]
    public string Query { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("lang")]
    public string? Lang { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("scrapeOptions")]
    public ScrapeOptions ScrapeOptions { get; set; }
}