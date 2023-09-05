using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request action list model
    /// </summary>
    public partial record ReturnRequestActionListModel : BasePagedListModel<ReturnRequestActionModel>
    {
    }
}