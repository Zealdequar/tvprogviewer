using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer list model
    /// </summary>
    public partial record ManufacturerListModel : BasePagedListModel<ManufacturerModel>
    {
    }
}