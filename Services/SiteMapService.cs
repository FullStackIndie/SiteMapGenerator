using Serilog;
using SiteMapGenerator.Models;
using System.Xml;
using System.Xml.Linq;

namespace SiteMapGenerator.Services
{
    public class SiteMapService
    {
        private readonly XNamespace _xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private readonly ILogger _logger;

        public SiteMapService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<bool> GenerateSitemapAsync(string? siteMapPath, List<SitemapEntry> sitemapEntries, CancellationToken cancellationToken)
        {
            sitemapEntries = RemoveDuplicateUrlEntries(sitemapEntries);

            // Create an XML document for the sitemap
            var xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null),
                new XElement(_xmlns + "urlset", sitemapEntries.Select(ToXElement)));
            string? path = siteMapPath;
            if (string.IsNullOrEmpty(path) || path == ".")
            {
                path = $"{Directory.GetCurrentDirectory()}/sitemap.xml";
            }
            using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 4096, useAsync: true);
            using XmlWriter xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Async = true, Indent = true });

            await xmlDoc.SaveAsync(xmlWriter, cancellationToken);
            _logger.Information($"Sitemap generated at {path}");
            return true;
        }

        private XElement ToXElement(SitemapEntry entry)
        {
            return new XElement(_xmlns + "url",
                new XElement(_xmlns + "loc", entry.Url),
                new XElement(_xmlns + "lastmod", entry.LastModified.ToString("yyyy-MM-dd")),
                new XElement(_xmlns + "changefreq", entry.ChangeFrequency.ToString().ToLowerInvariant()));
        }

        private List<SitemapEntry> RemoveDuplicateUrlEntries(List<SitemapEntry> entries)
        {
            var result = new List<SitemapEntry>();
            foreach (var entry in entries)
            {
                if (!result.Any(r => r.Url == entry.Url))
                {
                    result.Add(entry);
                }
            }
            return result;
        }


    }
}
