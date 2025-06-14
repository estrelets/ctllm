using System.Security.Cryptography;
using System.Text;

namespace Common.RAG.FileSystem;

public class FileDocument(FileInfo fileInfo) : IDocument
{
    public const string CreationTimeAttribute = "CreationTime";
    public const string LastWriteTimeAttribute = "LastWriteTime";
    public const string LastAccessTimeAttribute = "LastAccessTime";

    public string Id => fileInfo.FullName;
    public string Name => fileInfo.Name;

    public IReadOnlyDictionary<string, string> Attributes { get; } = new Dictionary<string, string>
    {
        [CreationTimeAttribute] = fileInfo.CreationTimeUtc.ToString("s"),
        [LastWriteTimeAttribute] = fileInfo.LastWriteTimeUtc.ToString("s"),
        [LastAccessTimeAttribute] = fileInfo.LastAccessTimeUtc.ToString("s"),
    };

    public Task<string> GetContent(CancellationToken ct)
    {
        return File.ReadAllTextAsync(fileInfo.FullName, ct);
    }

    public async Task<string> GetHash(CancellationToken ct)
    {
        await using var fs = File.OpenRead(fileInfo.FullName);
        var sha256 = SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(fs);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }
}
