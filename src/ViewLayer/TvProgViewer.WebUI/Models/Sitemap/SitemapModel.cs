using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Sitemap
{
    public partial record SitemapModel : BaseTvProgModel
    {
        #region Ctor

        public SitemapModel()
        {
            Items = new List<SitemapItemModel>();
            PageModel = new SitemapPageModel();
        }

        #endregion

        #region Properties

        public List<SitemapItemModel> Items { get; set; }

        public SitemapPageModel PageModel { get; set; }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }

        #endregion

        #region Nested classes

        public partial record SitemapItemModel
        {
            public string GroupTitle { get; set; }
            public string Url { get; set; }
            public string Name { get; set; }
        }

        #endregion
    }
}