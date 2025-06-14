using Spectre.Console.Cli;

namespace Cli.Commands;

public class RagSettings : CommandSettings
{
    [CommandOption("-a|--agent")]
    public string? Agent { get; init; }
    
    public class Load : RagSettings
    {
        [CommandOption("-f|--force")]
        public string? Force { get; init; }        
    }
}
