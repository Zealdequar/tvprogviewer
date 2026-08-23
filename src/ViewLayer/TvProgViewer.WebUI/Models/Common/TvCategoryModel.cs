using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public record TvCategoryModel : BaseTvProgEntityModel
    {
        public string Name { get; set; }
    }
}
