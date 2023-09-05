using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Cms
{
    /// <summary>
    /// Represents a widget list model
    /// </summary>
    public partial record WidgetListModel : BasePagedListModel<WidgetModel>
    {
    }
}