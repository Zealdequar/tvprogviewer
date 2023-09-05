using System;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Cms
{
    public partial record RenderWidgetModel : BaseTvProgModel
    {
        public Type WidgetViewComponent { get; set; }
        public object WidgetViewComponentArguments { get; set; }
    }
}