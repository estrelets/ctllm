using Microsoft.SemanticKernel.ChatCompletion;

namespace Lr.Agents;

public class ChatContext(string prompt)
{
    public ChatHistory History { get; } = new(prompt);

    
    public ChatContext ChangePrompt(string prompt)
    {
        var context = new ChatContext(prompt);
        foreach (var item in History.Skip(1))
        {
            context.History.Add(item);
        }

        return context;
    }
}
