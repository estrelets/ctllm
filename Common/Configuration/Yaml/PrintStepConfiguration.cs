using Common.Steps;

namespace Common.Configuration.Yaml;

// ReSharper disable once UnusedType.Global
public class PrintStepConfiguration : IStepConfiguration
{
    public string Name => "Print";
    
    public IStep Parse(YamlParseContext context)
    {
        return new PrintStep();
    }
}