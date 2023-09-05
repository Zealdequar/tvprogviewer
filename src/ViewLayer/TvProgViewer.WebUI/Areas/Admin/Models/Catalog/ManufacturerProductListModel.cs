using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer product list model
    /// </summary>
    public partial record ManufacturerProductListModel : BasePagedListModel<ManufacturerProductModel>
    {
    }
}