using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a bestseller list model
    /// </summary>
    public partial record BestsellerListModel : BasePagedListModel<BestsellerModel>
    {
    }
}