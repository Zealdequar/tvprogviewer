using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Sitemap
{
    public partial record SitemapXmlModel : BaseTvProgModel
    {
        public string SitemapXmlPath { get; set; }
    }
}