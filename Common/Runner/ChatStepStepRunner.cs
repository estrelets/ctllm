using Common.ModelClient;
using Common.StepResults;
using Common.Steps;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Common.Runner;

public class ChatStepStepRunner(ModelAccessor accessor, IUserInterface ui) : IStepRunner<ChatStep>
{
    public async Task<IStepResult> Run(StepContext context, ChatStep step, CancellationToken ct)
    {
        var model = accessor.Get(step.Model);
        var history = new ChatHistory();

        if (context.Last is not StubStepResult && !step.IgnoreFirstMessage)
        {
            await SendMessage(context.Last.Main);
        }

        if (step.IgnoreFirstMessage)
        {
            history.AddUserMessage(context.Last.Main);
        }

        while (!ct.IsCancellationRequested)
        {
            var message = await ui.GetMessage(ct);
            await SendMessage(message);
        }

        return new ChatStepResult(history);

        async Task SendMessage(string message)
        {
            history.AddMessage(AuthorRole.User, message);
            
            var answer = await model.GetChatAnswer(history, step.CustomSystemPrompt, ct);
            await ui.ShowMessage(AuthorRole.Assistant, answer, ct);
        }
    }
}