using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliated order list model
    /// </summary>
    public partial record AffiliatedOrderListModel : BasePagedListModel<AffiliatedOrderModel>
    {
    }
}