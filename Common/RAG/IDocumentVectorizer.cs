using Common.ModelClient;
using Microsoft.SemanticKernel;

namespace Common.RAG;

public interface IDocumentVectorizer
{
    Task Load(IModelClient model, IDocumentsSource source, CancellationToken ct);
    Task<KernelPlugin?> CreatePlugin(IModelClient model, IDocumentsSource source, CancellationToken ct);
}
