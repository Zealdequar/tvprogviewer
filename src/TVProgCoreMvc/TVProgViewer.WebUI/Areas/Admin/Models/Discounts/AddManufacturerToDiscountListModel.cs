using TVProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a manufacturer list model to add to the discount
    /// </summary>
    public partial record AddManufacturerToDiscountListModel : BasePagedListModel<ManufacturerModel>
    {
    }
}