using Microsoft.SemanticKernel.Data;

namespace Common.RAG.SqliteVectorizer;

public class SqliteDocumentToTextMapper : ITextSearchStringMapper, ITextSearchResultMapper
{
    public string MapFromResultToString(object result)
    {
        if (result is SqliteDocument dbModel)
        {
            return dbModel.Content;
        }

        throw new ArgumentException("Invalid result type.");
    }

    public TextSearchResult MapFromResultToTextSearchResult(object result)
    {
        if (result is SqliteDocument dbModel)
        {
            return new TextSearchResult(dbModel.Content)
            {
                Name = dbModel.Name,
                Link = dbModel.Id
            };
        }

        throw new ArgumentException("Invalid result type.");
    }
}
