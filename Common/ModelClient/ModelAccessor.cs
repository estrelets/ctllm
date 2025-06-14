using Common.RAG;

#pragma warning disable SKEXP0070

namespace Common.ModelClient;

public class ModelAccessor(IEnumerable<IModel> models)
{
    private readonly Dictionary<IModel, IModelClient> _clients = new();
    
    public IModelClient Get(IModel model)
    {
        if (_clients.TryGetValue(model, out var client))
        {
            return client;
        }
        
        return _clients[model] = model switch
        {
            OllamaModel ollamaModel => new OllamaModelClient(ollamaModel),
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
        };
    }

    public async Task Init(IDocumentVectorizer vectorizer, CancellationToken ct)
    {
        foreach (var model in models)
        {
            var client = Get(model);
            foreach (var documentsSource in model.DocumentsSources)
            {
                var embedder = Get(documentsSource.Embedder);
                var plugin = await vectorizer.CreatePlugin(embedder, documentsSource, ct);
                if (plugin != null)
                {
                    client.AddPlugin(plugin);
                }
            }
        }
    }
}