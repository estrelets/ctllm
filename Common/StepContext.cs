namespace Common;

public class StepContext
{
    private readonly List<IStepResult> _results = new();
    public IReadOnlyList<IStepResult> Results => _results;

    public IStepResult Last => _results.LastOrDefault() ?? new StubStepResult();

    public void Push(IStepResult step)
    {
        _results.Add(step);
    }
}

