using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a return request reason list model
    /// </summary>
    public partial record ReturnRequestReasonListModel : BasePagedListModel<ReturnRequestReasonModel>
    {
    }
}