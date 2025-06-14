namespace Common.Configuration.Yaml;

public interface IModelConfiguration
{
    IModel Parse(string key, YamlParseContext context);
    void BindDocuments(IModel model, YamlParseContext context);
}