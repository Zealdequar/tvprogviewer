using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
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

        #endregion

        #region Nested recordes

        public record SitemapItemModel
        {
            public string GroupTitle { get; set; }
            public string Url { get; set; }
            public string Name { get; set; }
        }

        #endregion
    }
}