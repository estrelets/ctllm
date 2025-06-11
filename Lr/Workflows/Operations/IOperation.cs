using Lr.Agents;
using Lr.UI;

namespace Lr.Workflows.Operations;

public interface IOperation
{
    Task Execute(
        Agent agent,
        IWorkflow workflow,
        IUserInterface ui,
        ChatContext chat,
        string? query,
        CancellationToken ct);
}