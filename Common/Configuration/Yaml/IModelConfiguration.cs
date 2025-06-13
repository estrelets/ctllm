namespace Common.Configuration.Yaml;

public interface IModelConfiguration
{
    IModel Parse(YamlParseContext context);
}