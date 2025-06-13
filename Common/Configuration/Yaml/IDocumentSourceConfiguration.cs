using Common.RAG;

namespace Common.Configuration.Yaml;

public interface IDocumentSourceConfiguration
{
    IDocumentsSource Parse(string key, YamlParseContext context);
}
