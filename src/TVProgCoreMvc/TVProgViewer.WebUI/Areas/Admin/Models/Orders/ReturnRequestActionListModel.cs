using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request action list model
    /// </summary>
    public partial record ReturnRequestActionListModel : BasePagedListModel<ReturnRequestActionModel>
    {
    }
}