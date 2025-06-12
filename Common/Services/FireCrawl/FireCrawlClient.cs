using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Serilog;

namespace Common.Services.FireCrawl;

public class FireCrawlClient(string url)
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri(url),
        Timeout = TimeSpan.FromMinutes(5)
    };

    public async Task<FireCrawlResult> Search(
        string query,
        string format,
        int limit,
        string? language,
        string? country,
        CancellationToken ct)
    {
        try
        {
            var req = new FireCrawlSearchRequest()
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

            var json = JsonSerializer.Serialize(req, JsonSerializerOptions.Web);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Logger.Debug("Send request to Firecrawl: {Payload}", json);
            var response = await _client.PostAsync("search", content, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);
            Logger.Debug("Received from Firecrawl: {Payload}", responseContent);
            var result = await response.Content.ReadFromJsonAsync<FireCrawlResult>(cancellationToken: ct);
            return result ?? new FireCrawlResult() { Success = false };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error while firecrawl request");
            return new FireCrawlResult() { Success = false };
        }
    }
}
