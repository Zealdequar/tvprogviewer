using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Stores
{
    /// <summary>
    /// Represents a store list model
    /// </summary>
    public partial record StoreListModel : BasePagedListModel<StoreModel>
    {
    }
}