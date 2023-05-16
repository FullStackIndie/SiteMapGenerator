using SiteMapGenerator.Converters;
using SiteMapGenerator.Data.Enums;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace SiteMapGenerator.CLI.Settings
{
    internal class SiteMapSettings : LogLevelSettings
    {
        [CommandArgument(0, "<url>")]
        [Description("Url of website/domain to crawl.")]
        public string? Url { get; set; }


        [CommandOption("-p|--path")]
        [Description("SiteMapPath and file name for sitemap. The default is current directory ./sitemap.xml")]
        public string SiteMapPath { get; set; }

        [CommandOption("-f|--frequency")]
        [Description("Frequency of changes on your website. Added to sitemap.")]
        [TypeConverter(typeof(ChangeFrequencyTypeConverter))]
        [DefaultValue(ChangeFrequency.Daily)]
        public ChangeFrequency ChangeFrequency { get; set; }
    }
}
