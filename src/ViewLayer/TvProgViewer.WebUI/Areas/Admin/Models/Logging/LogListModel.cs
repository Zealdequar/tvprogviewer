using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents a log list model
    /// </summary>
    public partial record LogListModel : BasePagedListModel<LogModel>
    {
    }
}