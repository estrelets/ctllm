using Common.ModelClient;
using Common.RAG;

namespace Common;

public class Agent(IEnumerable<IModel> models)
{
    public required string Name { get; set; }
    public required Workflow Workflow { get; set; }
    public required IDocumentsSource[] DocumentsSources { get; set; }
    public ModelAccessor Models { get; } = new(models);
}
