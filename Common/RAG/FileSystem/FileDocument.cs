using System.Security.Cryptography;
using System.Text;

namespace Common.RAG.FileSystem;

public class FileDocument(FileInfo fileInfo, string content) : IDocument, IDocumentLite
{
    public const string HashAttribute = "Hash";
    public const string CreationTimeAttribute = "CreationTime";
    public const string LastWriteTimeAttribute = "LastWriteTime";
    public const string LastAccessTimeAttribute = "LastAccessTime";

    public string Id => fileInfo.FullName;
    public string Name => fileInfo.Name;
    public string Content => content;

    public IReadOnlyDictionary<string, string> Attributes { get; } = new Dictionary<string, string>
    {
        [HashAttribute] = CalculateFileHash(content),
        [CreationTimeAttribute] = fileInfo.CreationTimeUtc.ToString("s"),
        [LastWriteTimeAttribute] = fileInfo.LastWriteTimeUtc.ToString("s"),
        [LastAccessTimeAttribute] = fileInfo.LastAccessTimeUtc.ToString("s"),
    };


    private static string CalculateFileHash(string input)
    {
        var sha256 = SHA256.Create();
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(inputBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }
}
