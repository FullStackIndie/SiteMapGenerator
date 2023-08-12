using Microsoft.Extensions.DependencyInjection;
using SiteMapGenerator.CLI.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SiteMapGenerator
{
    static class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                AnsiConsole.WriteLine("Setting up Services and Configuration");
                var services = new ServiceCollection();
                var app = new CommandApp<SiteMapCommand>(Startup.ConfigureServices(services));

                AnsiConsole.WriteLine("Setting up CLI Commands and Configuration");
                app.Configure(Startup.ConfigureApplication);

                AnsiConsole.WriteLine("Starting SiteMap Generator...");
                return await app.RunAsync(args);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine("An unhandled exception occurred. Exiting...");
                AnsiConsole.WriteException(ex);
                return 1;
            }
        }


    }
}