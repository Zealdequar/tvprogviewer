using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a manufacturer list model
    /// </summary>
    public partial record ManufacturerListModel : BasePagedListModel<ManufacturerModel>
    {
    }
}