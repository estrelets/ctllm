using Common.RAG;
using Common.RAG.FileSystem;

namespace Common.Configuration.Yaml;

public class FileDocumentSourceConfiguration : IDocumentSourceConfiguration
{
    public required string Path { get; set; }
    public required string Embedder { get; set; }
    
    public IDocumentsSource Parse(string key, YamlParseContext context)
    {
        return new FileDocumentSource()
        {
            Name = key,
            Path = Path,
            Embedder = context.GetModel(Embedder)
        };
    }
}
