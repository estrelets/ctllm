using Common.RAG;
using Common.RAG.FileSystem;

namespace Common.Configuration.Yaml;

public class DirectoryDocumentSourceConfiguration : IDocumentSourceConfiguration
{
    public required string Path { get; set; }
    public required string Pattern { get; set; }
    public required bool Recursive { get; set; }
    
    public IDocumentsSource Parse(string key, YamlParseContext context)
    {
        return new DirectoryDocumentSource()
        {
            Name = key,
            Path = Path,
            Pattern = Pattern,
            Recursive = Recursive
        };
    }
}
