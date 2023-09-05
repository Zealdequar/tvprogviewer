using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request list model
    /// </summary>
    public partial record ReturnRequestListModel : BasePagedListModel<ReturnRequestModel>
    {
    }
}