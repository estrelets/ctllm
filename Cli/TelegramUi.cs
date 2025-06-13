using System.Collections.Concurrent;
using Common;
using Microsoft.SemanticKernel.ChatCompletion;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Cli;

public class TelegramUi(ChatId chatId, string token) : IUserInterface
{
    private readonly TelegramBotClient _bot = new(token);
    private readonly ConcurrentQueue<string?> _input = new();

    public async Task Init()
    {
        var me = await _bot.GetMe();
        _bot.OnMessage += OnMessageHandler;
        Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
    }
    
    private Task OnMessageHandler(Message message, UpdateType type)
    {
        if (message.Type != MessageType.Text)
        {
            return Task.CompletedTask;
        }
    
        var text = message.Text?.ToLower();
        _input.Enqueue(text);
        
        return Task.CompletedTask;
    }

    public async Task<string?> PromptString(string? title = null, string? defaultValue = null, bool allowEmpty = true)
    {
        if (title != null)
        {
            await _bot.SendMessage(chatId, title);
        }

        var message = await ReadMessage();
        if (!allowEmpty && String.IsNullOrEmpty(message))
        {
            return await PromptString(title, defaultValue, allowEmpty);
        }

        return message;
    }

    public async Task<string> GetMessage(CancellationToken ct)
    {
        return await ReadMessage() ?? String.Empty;
    }

    public async Task ShowMessage(AuthorRole role, string messageLong, CancellationToken ct)
    {
        var messages = messageLong.Chunk(4096).Select(chars => new string(chars));
        foreach (var message in messages)
        {
            await _bot.SendMessage(chatId, message, cancellationToken: ct);    
        }
    }

    private async Task<string?> ReadMessage()
    {
        string? message;
        while (!_input.TryDequeue(out message))
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        return message;
    }
}
