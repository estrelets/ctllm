namespace Common.RAG.FileSystem;

public class FileDocumentEqualityComparer : IEqualityComparer<FileDocument>
{
    public static FileDocumentEqualityComparer Instance => new();
    
    public bool Equals(FileDocument? x, FileDocument? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null)
        {
            return false;
        }

        if (y is null)
        {
            return false;
        }

        if (x.GetType() != y.GetType())
        {
            return false;
        }

        var xHash = x.Attributes.GetValueOrDefault(FileDocument.HashAttribute);
        var yHash = y.Attributes.GetValueOrDefault(FileDocument.HashAttribute);

        return x.Id.Equals(y.Id) && xHash == yHash;
    }

    public int GetHashCode(FileDocument obj)
    {
        return HashCode.Combine(obj.Id, obj.Attributes.GetHashCode());
    }
}
