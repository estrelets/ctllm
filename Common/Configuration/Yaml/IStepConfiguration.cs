using Common.Steps;

namespace Common.Configuration.Yaml;

public interface IStepConfiguration
{
    string Name { get; }

    IStep Parse(YamlParseContext context);
}