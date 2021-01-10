using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tier price list model
    /// </summary>
    public partial record TierPriceListModel : BasePagedListModel<TierPriceModel>
    {
    }
}