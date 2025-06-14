using System.ComponentModel;
using Spectre.Console.Cli;

namespace Cli.Commands;

public class ChatSettings : CommandSettings
{
    [Description("Text for LLM")]
    [CommandArgument(0, "[text]")]
    public string? Text { get; init; }
    
    [CommandOption("-a|--agent")]
    public string? Agent { get; init; }
}
