using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request list model
    /// </summary>
    public partial record ReturnRequestListModel : BasePagedListModel<ReturnRequestModel>
    {
    }
}