using Common.ModelClient;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.SqliteVec;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Text;

#pragma warning disable SKEXP0001

namespace Common.RAG.SqliteVectorizer;

public class DocumentVectorizer(string dbPath, IUserInterface ui) : IDocumentVectorizer
{
    public async Task Load(IModelClient model, IDocumentsSource source, CancellationToken ct)
    {
        var collection = await GetCollection(model, source.Name, ct);
        await collection.EnsureCollectionExistsAsync(ct);

        var documents = await source.Refresh(ct);

        foreach (var (document, i) in documents.Select((x, i) => (x, i)))
        {
            await ui.ShowMessage(AuthorRole.Tool, $"Start load [{i + 1}/{documents.Length}] {document.Name}:{document.Id}", ct);
            await ImportDocument(model, collection, document, ct);
        }
    }
    
    public async Task<KernelPlugin?> CreatePlugin(IModelClient model, IDocumentsSource source, CancellationToken ct)
    {
        var collection = await GetCollection(model, source.Name, ct);
        
        if (await collection.CollectionExistsAsync(ct) == false)
        {
            return null;
        }

        var formatter = new SqliteDocumentToTextMapper();
        var search = new VectorStoreTextSearch<SqliteDocument>(collection, model.EmbeddingGenerator, 
            formatter, formatter);
        
        return search.CreateWithGetTextSearchResults(
            $"SearchPlugin{source.Name}",
            $"Search for information in {source.Name} knowledge base");
    }

    private async Task ImportDocument(
        IModelClient model,
        SqliteCollection<string, SqliteDocument> collection,
        IDocument document,
        CancellationToken ct)
    {
        if (await IsLoadRequired(collection, document, ct) == false)
        {
            return;
        }
        
        var content = await document.GetContent(ct);
        var hash = await document.GetHash(ct);
        
        var chunks = ChunkString(content, 512);

        foreach (var (chunk, index) in chunks.Select(((s, i) => (s, i))))
        {
            var embedResult = await model.EmbedAsync(chunk, ct);
            var dbDocument = new SqliteDocument
            {
                Id = GenerateChunkId(document.Id, index),
                Name = document.Name,
                ChunkNumber = index,
                Hash = hash,
                Content = chunk,
                ContentEmbedding = embedResult,
                AttributesJson = string.Empty
            };
            
            await collection.UpsertAsync(dbDocument, ct);
        }
    }

    private async Task<bool> IsLoadRequired(
        SqliteCollection<string, SqliteDocument> collection,
        IDocument document,
        CancellationToken ct)
    {
        var firstChunkId = GenerateChunkId(document.Id, 0);
        var existingDocument = await collection.GetAsync(firstChunkId, cancellationToken: ct);

        if (existingDocument == null)
        {
            Logger.Debug("Document is new");
            return true;
        }
        
        var existingHash = existingDocument.Hash;
        var hash = await document.GetHash(ct);
        if (existingHash == hash)
        {
            Logger.Debug("Same document already exists");
            return false;
        }

        Logger.Debug("Document changed");
        return true;
    }

    private async Task<SqliteCollection<string, SqliteDocument>> GetCollection(
        IModelClient model,
        string sourceName,
        CancellationToken ct)
    {
        var dbFilePath = Path.Combine(dbPath, Path.ChangeExtension(sourceName, "sqlite"));
        var connectionString = $"Data Source={dbFilePath}";
        var vectorStore = new SqliteVectorStore(connectionString);

        var dimesions = await model.GetEmbeddingLength(ct);
        var collectionDefinition = SqliteDocument.CreateVectorStoreCollectionDefinition(dimesions);

        return vectorStore.GetCollection<string, SqliteDocument>(sourceName, collectionDefinition);
        //return vectorStore.GetDynamicCollection(sourceName, SqliteDocument.CreateVectorStoreCollectionDefinition(dimesions));
    }

    private static IEnumerable<string> ChunkString(string input, int maxTokens)
    {
#pragma warning disable SKEXP0050
        return TextChunker.SplitPlainTextLines(input, maxTokens);
#pragma warning restore SKEXP0050
    }

    private static string GenerateChunkId(string documentId, int index)
    {
        return $"{documentId}#{index:D6}";
    }
}


// public async Task Search(string agent, IDocumentsSource ds, CancellationToken ct)
// {
//     var model = "asd";
//     var name = "test";
//     var path = "/media/static/LLM/ollama/custom_models/db asd das.sqlite";
//     var cs = $"Data Source={path}";
//     var ollama = new OllamaApiClient("http://localhost:11434", model);
//
//
//         
//     var vectorStoreCollection = new SqliteCollection<string, SqliteDocument>(cs, name,
//         new SqliteCollectionOptions()
//         {
//             EmbeddingGenerator = ollama
//         });
//
//     var dbModelTextMapper = new SqliteDocumentToTextMapper();
//     var search =
//         new VectorStoreTextSearch<SqliteDocument>(vectorStoreCollection, ollama, dbModelTextMapper,
//             dbModelTextMapper);
//
//     search.CreateWithGetTextSearchResults("SearchPlugin");
//     var searchResultCollection = await search.SearchAsync("Access modifier guidelines", cancellationToken: ct, searchOptions: new TextSearchOptions(){});
//     var searchResults = await searchResultCollection.Results.ToArrayAsync(ct);
// }
