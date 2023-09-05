using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a GDPR request list model
    /// </summary>
    public partial record GdprLogListModel : BasePagedListModel<GdprLogModel>
    {
    }
}