using System.Net.Http.Json;
using System.Text.Json;

namespace Lr.Integrations.FireCrawl;

public class FireCrawlClient(string url)
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri(url),
        Timeout = TimeSpan.FromMinutes(5)
    };
    
    public async Task<FireCrawlResult> Search(string query, CancellationToken ct)
    {
        try
        {
            var req = new FireCrawlSearchRequest()
            {
                Lang = "ru",
                Country = "ru",
                Query = query,
                Limit = 5,
                ScrapeOptions = new ScrapeOptions()
                {
                    Formats = ["markdown"]
                },
                Tbs = ""
            };

            var response = await _client.PostAsJsonAsync("search", req, JsonSerializerOptions.Web, ct);
            var result = await response.Content.ReadFromJsonAsync<FireCrawlResult>(cancellationToken: ct);
            return result ?? new FireCrawlResult() { Success = false };
        }
        catch(Exception ex)
        {
            return new FireCrawlResult() { Success = false };;
        }
    }
}
