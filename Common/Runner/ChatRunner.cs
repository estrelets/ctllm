using Common.ModelClient;
using Common.Services.FireCrawl;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Common.Runner;

public class ChatRunner(ModelAccessor accessor, IUserInterface ui)
{
    public async Task<IStepResult> Run(StepContext context, ChatStep step, CancellationToken ct)
    {
        var model = accessor.Get(step.Model);
        var history = new ChatHistory();

        if (context.Last is not StubStepResult)
        {
            await SendMessage(context.Last.Main);
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

public class RephraseRunner(ModelAccessor accessor)
{
    public async Task<IStepResult> Run(StepContext context, RephraseStep step, CancellationToken ct)
    {
        var model = accessor.Get(step.Model);
        var result = await model.Ask(context.Last.Main, step.CustomSystemPrompt, ct);
        return new RephraseStepResult(result);
    }
}

public class FirecrawlSearchRunner(FireCrawlClient client)
{
    public async Task<IStepResult> Run(StepContext context, FirecrawlSearchStep step, CancellationToken ct)
    {
        var foundResult = await client.Search(context.Last.Main, step.Format, step.Limit, step.Language, step.Country, ct);
        if (!foundResult.Success || (foundResult.Data?.Length ?? 0) == 0)
        {
            return new FirecrawlSearchStepFailedResult();
        }

        return new FirecrawlSearchStepResult(foundResult.Data!);
    }
}

public class PrintStepRunner(IUserInterface ui)
{
    public async Task<IStepResult> Run(StepContext context, PrintStep step, CancellationToken ct)
    {
        await ui.ShowMessage(AuthorRole.Tool, context.Last.Main, ct);
        return new NoOpStepResult();
    }
}