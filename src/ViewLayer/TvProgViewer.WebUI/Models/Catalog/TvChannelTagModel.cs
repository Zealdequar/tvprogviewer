using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    public partial record TvChannelTagModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public int TvChannelCount { get; set; }
    }
}