using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents a log list model
    /// </summary>
    public partial record LogListModel : BasePagedListModel<LogModel>
    {
    }
}