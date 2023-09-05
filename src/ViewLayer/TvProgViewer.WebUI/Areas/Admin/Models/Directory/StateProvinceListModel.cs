using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a state and province list model
    /// </summary>
    public partial record StateProvinceListModel : BasePagedListModel<StateProvinceModel>
    {
    }
}