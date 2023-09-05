using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliated order list model
    /// </summary>
    public partial record AffiliatedOrderListModel : BasePagedListModel<AffiliatedOrderModel>
    {
    }
}