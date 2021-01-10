using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Common
{
    public partial record StoreThemeModel : BaseTvProgModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}