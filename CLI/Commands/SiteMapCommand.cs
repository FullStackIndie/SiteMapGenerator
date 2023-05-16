using Serilog;
using SiteMapGenerator.CLI.Settings;
using SiteMapGenerator.Services;
using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;

namespace SiteMapGenerator.CLI.Commands
{
    internal class SiteMapCommand : AsyncCommand<SiteMapSettings>
    {
        private readonly ILogger _logger;
        private readonly SiteMapService _siteMapService;
        private readonly WebCrawlerService _webCrawlerService;

        public SiteMapCommand(SiteMapService siteMapService,
            WebCrawlerService webCrawlerService,
            ILogger logger)
        {
            _siteMapService = siteMapService;
            _webCrawlerService = webCrawlerService;
            _logger = logger;
            _logger = logger;
        }

        public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] SiteMapSettings settings)
        {
            _logger.Debug($"--- Starting sitemap generator. DateTime UTC = {DateTime.UtcNow} ---");
            if (string.IsNullOrEmpty(settings.Url))
            {
                AnsiConsole.WriteLine($"Url was empty");
                return 0;
            }

            Uri uri = new(settings.Url.TrimEnd('/').Trim());
            _logger.Information("Crawling {uri}", uri);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            await _webCrawlerService.Crawl(uri, uri, cancellationToken);
            _logger.Information("Crawling complete.");
            _logger.Debug("Attempting to generate sitemap.");
            var savedSiteMap = await _siteMapService.GenerateSitemapAsync(settings.SiteMapPath, _webCrawlerService.SitemapEntries, cancellationToken);
            if (savedSiteMap)
            {
                return 0;
            }

            return 1;
        }
    }
}
