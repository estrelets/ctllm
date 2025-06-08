using Lr.Agents;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lr.UI;

public interface IUserInterface
{
    Task<UserCommand> GetNextCommand(CancellationToken ct);
    void PrintMessage(AuthorRole role, string? message);
    Task<Agent> PickAgent(ApplicationContext context, string? query, CancellationToken ct);

    string? PromptString(string? title = null, string? defaultValue = null, bool allowEmpty = true);
}