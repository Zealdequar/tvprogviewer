using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a cross-sell product list model
    /// </summary>
    public partial record CrossSellProductListModel : BasePagedListModel<CrossSellProductModel>
    {
    }
}