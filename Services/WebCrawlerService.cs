using HtmlAgilityPack;
using Serilog;
using SiteMapGenerator.Data.Enums;
using SiteMapGenerator.Models;

namespace SiteMapGenerator.Services
{
    public class WebCrawlerService
    {
        public List<SitemapEntry> SitemapEntries { get; set; } = new();
        private List<string> invalidUrls { get; set; } = new();
        private HashSet<string> invalidPaths = new HashSet<string>()
        {
            "/", "mailto:", "//"
        };

        private readonly ILogger _logger;

        public WebCrawlerService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Crawl(Uri url, Uri parentUrl, CancellationToken cancellationToken)
        {
            try
            {
                var htmlWeb = new HtmlWeb();
                var htmlDocument = await htmlWeb.LoadFromWebAsync(url.ToString(), cancellationToken);

                // Parse the HTML document and extract relevant information
                // For example, extract links and metadata for the sitemap
                var extractedLinks = ExtractLinks(htmlDocument).Distinct();

                SitemapEntries.Add(new SitemapEntry
                {
                    Url = url.ToString(),
                    LastModified = DateTime.Now,
                    ChangeFrequency = ChangeFrequency.Daily
                });

                // Recursively crawl the extracted links
                foreach (var link in extractedLinks)
                {
                    if (invalidUrls.Contains(link))
                    {
                        return;
                    }
                    if (!UrlIsValidHtmlFile(link, parentUrl))
                    {
                        invalidUrls.Add(link);
                        _logger.Information($"Skipping link '{link}' because it isnt a valid html/php file Url");
                        continue;
                    }
                    if (invalidPaths.Any(i => link.StartsWith(i)) && !link.StartsWith("//"))
                    {
                        invalidUrls.Add(link);
                        _logger.Information($"Skipping link '{link}' because it isnt a valid Url");
                        continue;
                    }
                    if (link.StartsWith("#"))
                    {
                        invalidUrls.Add(link);
                        _logger.Information($"Skipping link '{link}' because it is a fragment Url");
                        continue;
                    }
                    if (!HostIsTheSame(link, parentUrl))
                    {
                        string[] uri = link.Split('/');
                        if (!uri[2].Contains(parentUrl.Host.Split('.')[1]))
                        {
                            invalidUrls.Add(link);
                            _logger.Information($"Skipping link '{link}' because it is a link for a different domain");
                            continue;
                        }

                        invalidUrls.Add(link);
                        _logger.Information($"Skipping link '{link}' because it is a link for a different sub-domain");
                        continue;
                    }
                    if (!link.Contains(parentUrl.Host))
                    {
                        invalidUrls.Add(link);
                        _logger.Information($"Skipping link '{link}' because it is a link for a different domain");
                        continue;
                    }
                    if (link.StartsWith("//"))
                    {
                        var newLink = $"{parentUrl.Scheme}:{link}".TrimEnd('/');

                        _logger.Information($"Crawling {newLink}");
                        await Crawl(new Uri(newLink), parentUrl, cancellationToken);
                        continue;
                    }

                    _logger.Information($"Crawling {link.TrimEnd('/')}");
                    await Crawl(new Uri(link.TrimEnd('/')), parentUrl, cancellationToken);
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.Error($"Error crawling {url} - {httpEx.Message}. \n Make sure host name (trimmedUrl) is correct");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error crawling {url} - {ex.Message} \n ---- [ StackTrace ] ----- \n {ex.StackTrace}");
            }

        }

        private List<string> ExtractLinks(HtmlDocument htmlDocument)
        {
            var links = new List<string>();

            // Use HtmlAgilityPack to select <a> elements and extract the "href" attribute
            var anchorNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            var dataRefNodes = htmlDocument.DocumentNode.SelectNodes("//a[@data-href]");

            if (anchorNodes != null)
            {
                foreach (var attribute in anchorNodes.Select(a => a.Attributes))
                {
                    var hrefAttribute = attribute["href"];
                    if (hrefAttribute != null)
                    {
                        var link = hrefAttribute.Value;
                        links.Add(link);
                    }
                }
            }

            if (dataRefNodes != null)
            {
                foreach (var attribute in dataRefNodes.Select(a => a.Attributes))
                {
                    var dataHrefAttribute = attribute["data-href"];
                    if (dataHrefAttribute != null)
                    {
                        var link = dataHrefAttribute.Value;
                        links.Add(link);
                    }
                }
            }

            return links;
        }

        private bool HostIsTheSame(string urlToParse, Uri parentUrl)
        {
            try
            {
                if (!urlToParse.Contains("http") || !urlToParse.Contains(parentUrl.Host) &&
                    !urlToParse.Contains("http"))
                {
                    _logger.Information($"Detetcted Invalid Url {urlToParse} ... Skipping...");
                    invalidUrls.Add(urlToParse);
                    return false;
                }
                Uri uri = new Uri(urlToParse.TrimEnd('/'));
                if (uri.Host == parentUrl.Host)
                {
                    return true;
                }
            }
            catch (UriFormatException ex)
            {
                _logger.Information($"Error Filtering Url {urlToParse}");
            }

            return false;
        }


        private bool UrlIsValidHtmlFile(string urlToParse, Uri parentUrl)
        {
            try
            {
                string[] uri = urlToParse.Split('.');
                if (!uri[0].Contains("http") && urlToParse.Contains(".htm") ||
                    !uri[0].Contains("http") && urlToParse.Contains(".php"))
                {
                    return false;
                }
            }
            catch (UriFormatException ex)
            {
                _logger.Information($"Error Filtering Url {urlToParse}");
            }

            return true;
        }



    }
}
