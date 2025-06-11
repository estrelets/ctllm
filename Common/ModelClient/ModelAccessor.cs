#pragma warning disable SKEXP0070

namespace Common.ModelClient;

public class ModelAccessor
{
    public IModelClient Get(IModel model)
    {
        return model switch
        {
            OllamaModel ollamaModel => new OllamaModelClient(ollamaModel),
            _ => throw new ArgumentOutOfRangeException(nameof(model), model, null)
        };
    }
}