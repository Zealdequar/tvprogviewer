using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Cms
{
    public partial record RenderWidgetModel : BaseTvProgModel
    {
        public string WidgetViewComponentName { get; set; }
        public object WidgetViewComponentArguments { get; set; }
    }
}