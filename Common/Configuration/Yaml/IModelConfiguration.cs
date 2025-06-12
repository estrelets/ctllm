namespace Common.Configuration.Yaml;

public interface IModelConfiguration
{
    string Discriminator { get; }
    IModel Parse(YamlParseContext context);
}