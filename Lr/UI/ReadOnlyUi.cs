using Lr.Agents;
using Markdig;
using Markdig.Renderers.Normalize;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Lr.UI;

public class ReadOnlyUi : IUserInterface
{
    private readonly Queue<UserCommand> _commandQueue = new();

    public void PushCommand(UserCommand command)
    {
        _commandQueue.Enqueue(command);
    }

    public Task<UserCommand> GetNextCommand(CancellationToken ct)
    {
        if (_commandQueue.TryDequeue(out var queued))
        {
            return Task.FromResult(queued);
        }

        throw NotSupportedInput();
    }

    public void PrintMessage(AuthorRole role, string? message)
    {
        try
        {
            using var sw = new StringWriter();
            var normRender = new NormalizeRenderer(sw);
            normRender.Render(Markdown.Parse(message));
            var plain = sw.ToString();
            Console.WriteLine(plain);
        }
        catch
        {
            Console.WriteLine(message);
        }
    }

    public Task<Agent> PickAgent(ApplicationContext context, string? query, CancellationToken ct)
    {
        throw NotSupportedInput();
    }

    public string? PromptString(string? title = null, string? defaultValue = null, bool allowEmpty = true)
    {
        throw NotSupportedInput();
    }
    
    private static NotSupportedException NotSupportedInput()
    {
        return new NotSupportedException("Only for Console.IsInputRedirected = true");
    }
}
