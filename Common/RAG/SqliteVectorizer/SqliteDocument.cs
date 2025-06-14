using Microsoft.Extensions.VectorData;

namespace Common.RAG.SqliteVectorizer;

public class SqliteDocument
{
    public required string Id { get; set; }
    public int ChunkNumber { get; set; }
    public required string Hash { get; set; }
    public required string Name { get; set; }
    public required string Content { get; set; }
    public required string AttributesJson { get; set; }
    public ReadOnlyMemory<float>? ContentEmbedding { get; set; }
    
    public static VectorStoreCollectionDefinition CreateVectorStoreCollectionDefinition(int dimensions)
    {
        return new VectorStoreCollectionDefinition()
        {
            Properties = new List<VectorStoreProperty>
            {
                // Ключевое поле для идентификации документов
                new VectorStoreKeyProperty(nameof(Id), typeof(string)),

                // Дополнительные данные
                new VectorStoreDataProperty(nameof(ChunkNumber), typeof(int)),
                new VectorStoreDataProperty(nameof(Hash), typeof(string)),
                new VectorStoreDataProperty(nameof(Name), typeof(string)),
                new VectorStoreDataProperty(nameof(Content), typeof(string)),
                new VectorStoreDataProperty(nameof(AttributesJson), typeof(string)),

                // Векторное представление содержимого
                new VectorStoreVectorProperty(
                    nameof(ContentEmbedding),
                    typeof(ReadOnlyMemory<float>),
                    dimensions: dimensions)
            }
        };
    }
}
