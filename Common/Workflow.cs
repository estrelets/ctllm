namespace Common;

public class Workflow
{
    public required string Name { get; set; }
    public required IStep[] Steps { get; set; }
}