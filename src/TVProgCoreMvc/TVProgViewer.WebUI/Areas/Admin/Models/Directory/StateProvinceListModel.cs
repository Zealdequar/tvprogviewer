using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a state and province list model
    /// </summary>
    public record StateProvinceListModel : BasePagedListModel<StateProvinceModel>
    {
    }
}