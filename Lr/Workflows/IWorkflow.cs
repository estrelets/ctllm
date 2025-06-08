namespace Lr.Workflows;

public interface IWorkflow
{
    Task Run(CancellationToken ct);
}
