using Serilog.Events;
using SiteMapGenerator.Converters;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace SiteMapGenerator.CLI.Settings
{
    internal class LogLevelSettings : CommandSettings
    {
        [CommandOption("-L|--logLevel")]
        [Description("Minimum level for logging. Default is Information")]
        [TypeConverter(typeof(LogLevelConverter))]
        [DefaultValue(LogEventLevel.Information)]
        public LogEventLevel LogLevel { get; set; }
    }
}
