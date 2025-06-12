namespace Common.Configuration;

public interface IAgentFactory
{
    Task<Agent[]> Init(CancellationToken ct);
}
