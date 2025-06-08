namespace Lr.Agents.Configuration;

public abstract record InitResult
{
    public record Ok(AgentConfiguration Configuration) : InitResult;
    public record Failed(string Message) : InitResult;
}

public interface IAgentConfigurationFactory
{
    Task<InitResult[]> Init(CancellationToken ct);
}


