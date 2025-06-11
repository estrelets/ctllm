namespace Common.Configuration;

public class WorkflowConfiguration
{
    public required string Name { get; init; }
    public required IStepConfiguration[] Steps { get; init; }
}
