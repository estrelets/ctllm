namespace Common.Configuration;

public class FirecrawlSearchStepConfiguration : IStepConfiguration
{
    public required string Name { get; set; }
    public required string Format { get; set; }
    public int Limit { get; set; }
    public string? Language { get; set; }
    public string? Country { get; set; }
}
