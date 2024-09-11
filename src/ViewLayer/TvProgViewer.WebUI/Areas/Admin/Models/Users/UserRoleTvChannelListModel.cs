using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user role tvChannel list model
    /// </summary>
    public partial record UserRoleTvChannelListModel : BasePagedListModel<TvChannelModel>
    {
    }
}