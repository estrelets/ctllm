using Serilog;
using Serilog.Events;

namespace Cli;

public static class SerilogConfiguration
{
    public static void Init()
    {
        var logPath = Path.Combine(Path.GetTempPath(), "lr.log");

        var consoleTemplate = "[{Timestamp:HH:mm:ss} {Level:w2}] {Message:lj}{NewLine}{Exception}";
        var fileTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        Log.Logger = new LoggerConfiguration()
            //.WriteTo.Console(LogEventLevel.Information, outputTemplate: consoleTemplate)
            .WriteTo.File(logPath, LogEventLevel.Verbose, outputTemplate: fileTemplate)
            .MinimumLevel.Verbose()
            .CreateLogger();
    }
}
