using Common.RAG;

namespace Common;

public class Agent
{
    public required string Name { get; set; }
    public required Workflow Workflow { get; set; }
    public required IDocumentsSource[] DocumentsSources { get; set; }
}
