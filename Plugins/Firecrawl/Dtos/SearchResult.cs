using System.Text.Json.Serialization;

namespace Plugins.Firecrawl.Dtos;

public class SearchResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public FoundItem[]? Data { get; set; }
}