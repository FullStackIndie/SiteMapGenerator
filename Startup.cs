using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SiteMapGenerator.CLI.Commands;
using SiteMapGenerator.DependencyInjection;
using SiteMapGenerator.Interceptors;
using SiteMapGenerator.Services;
using Spectre.Console.Cli;

namespace SiteMapGenerator
{
    public static class Startup
    {

        public static ServiceBuilder ConfigureServices(IServiceCollection services)
        {
            var log = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(LogInterceptor.LogLevel)
                .WriteTo.Console()
                .WriteTo.File($"{Directory.GetCurrentDirectory()}/sitemap_generator_logs.txt")
                .CreateLogger();
            
            services.AddSingleton<ILogger>(log);
            services.AddScoped<SiteMapService>();
            services.AddScoped<WebCrawlerService>();

            return new ServiceBuilder(services);
        }

        public static void ConfigureApplication(IConfigurator config)
        {
            config.CaseSensitivity(CaseSensitivity.None);
            config.SetApplicationName("sitemap");
            config.ValidateExamples();
            config.SetInterceptor(new LogInterceptor());
            config.AddCommand<SiteMapCommand>("url")
                .WithExample(new[] { "url", "https://example.com", "-f", "Daily", "-p", ".", "-L", "Information"})
                .WithExample(new[] { "url", "https://example.com", "-f", "Daily", "-p", "/home/user1 | C:/Users/user1/Documents", "-L", "Information" });
        }

    }
}
