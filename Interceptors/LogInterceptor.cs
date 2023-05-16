using Serilog.Core;
using SiteMapGenerator.CLI.Settings;
using Spectre.Console.Cli;

namespace SiteMapGenerator.Interceptors
{
    public class LogInterceptor : ICommandInterceptor
    {
        public static LoggingLevelSwitch LogLevel { get; set; } = new LoggingLevelSwitch();

        public void Intercept(CommandContext context, CommandSettings settings)
        {
            if (settings is LogLevelSettings logLevelSettings)
            {
                LogLevel.MinimumLevel = logLevelSettings.LogLevel;
            }
        }
    }
}
