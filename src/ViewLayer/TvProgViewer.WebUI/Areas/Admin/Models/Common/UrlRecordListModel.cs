using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents an URL record list model
    /// </summary>
    public partial record UrlRecordListModel : BasePagedListModel<UrlRecordModel>
    {
    }
}