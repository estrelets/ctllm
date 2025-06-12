using Common.Steps;

namespace Plugins.Firecrawl.Steps;

/// <summary>
/// Step for searching information using Firecrawl
/// </summary>
public class SearchStep : IStep
{
    /// <inheritdoc />
    public required string Name { get; init; }
    
    /// <summary>
    /// Format of the search results output
    /// </summary>
    public required string Format { get; set; }
    
    /// <summary>
    /// Maximum number of results
    /// </summary>
    public int Limit { get; set; }
    
    /// <summary>
    /// Language of the search results content
    /// </summary>
    public string? Language { get; set; }
    
    /// <summary>
    /// Country associated with the results
    /// </summary>
    public string? Country { get; set; }
}