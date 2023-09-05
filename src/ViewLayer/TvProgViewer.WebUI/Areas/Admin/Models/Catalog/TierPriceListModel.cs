using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tier price list model
    /// </summary>
    public partial record TierPriceListModel : BasePagedListModel<TierPriceModel>
    {
    }
}