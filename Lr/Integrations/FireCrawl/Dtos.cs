using System.Text.Json.Serialization;

namespace Lr.Integrations.FireCrawl;

public class FireCrawlResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("data")]
    public List<Datum> Data { get; set; }
}

public class FireCrawlSearchRequest
{
    [JsonPropertyName("query")]
    public string Query { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("lang")]
    public string Lang { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("tbs")]
    public string Tbs { get; set; }

    [JsonPropertyName("scrapeOptions")]
    public ScrapeOptions ScrapeOptions { get; set; }
}

public class ScrapeOptions
{
    [JsonPropertyName("formats")]
    public List<string> Formats { get; set; }
}


public class Datum
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("markdown")]
    public string Markdown { get; set; }
}