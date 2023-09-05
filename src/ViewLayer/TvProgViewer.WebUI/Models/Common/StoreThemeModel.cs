using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Common
{
    public partial record StoreThemeModel : BaseTvProgModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}