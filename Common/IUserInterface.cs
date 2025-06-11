using Microsoft.SemanticKernel.ChatCompletion;

namespace Common;

public interface IUserInterface
{
    string? PromptString(string? title = null, string? defaultValue = null, bool allowEmpty = true);
    Task<string> GetMessage(CancellationToken ct);
    Task ShowMessage(AuthorRole role, string message, CancellationToken ct);
}
