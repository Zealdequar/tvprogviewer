using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents an URL record list model
    /// </summary>
    public partial record UrlRecordListModel : BasePagedListModel<UrlRecordModel>
    {
    }
}