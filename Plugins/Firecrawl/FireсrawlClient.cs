using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Plugins.Firecrawl.Dtos;

namespace Plugins.Firecrawl;

public class FirecrawlClient(HttpClient client)
{
    public async Task<SearchResult> Search(
        string query,
        string format,
        int limit,
        string? language,
        string? country,
        CancellationToken ct)
    {
        try
        {
            var req = new SearchRequest()
            {
                Lang = language,
                Country = country,
                Query = query,
                Limit = limit,
                ScrapeOptions = new ScrapeOptions()
                {
                    Formats = [format]
                }
            };

            var result = await Execute<SearchResult>("search", req, ct);
            return result ?? new SearchResult() { Success = false };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error while firecrawl request");
            return new SearchResult() { Success = false };
        }
    }

    public async Task<ScrapeResult> Scrape(
        string url,
        string format,
        bool onlyMainContent,
        CancellationToken ct)
    {
        try
        {
            var req = new ScrapeRequest()
            {
                Url = url,
                OnlyMainContent = onlyMainContent,
                Formats = [format]
            };

            var result = await Execute<ScrapeResult>("scrape", req, ct);
            if (result == null || result.Data == null || String.IsNullOrEmpty(result.Data.Markdown))
            {
                return new ScrapeResult() { Success = false };
            }

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error while firecrawl request");
            return new ScrapeResult() { Success = false };
        }
    }
    
    private async Task<T?> Execute<T>(string url, object payload, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(payload, JsonSerializerOptions.Web);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        Logger.Debug("Send request to Firecrawl: {Payload}", json);
        var response = await client.PostAsync(url, content, ct);
        var responseContent = await response.Content.ReadAsStringAsync(ct);
        Logger.Debug("Received from Firecrawl: {Payload}", responseContent);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }
}

