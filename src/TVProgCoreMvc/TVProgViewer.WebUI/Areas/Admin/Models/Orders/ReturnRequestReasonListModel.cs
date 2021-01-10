using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request reason list model
    /// </summary>
    public partial record ReturnRequestReasonListModel : BasePagedListModel<ReturnRequestReasonModel>
    {
    }
}