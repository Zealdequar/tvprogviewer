using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a news item list model
    /// </summary>
    public partial record NewsItemListModel : BasePagedListModel<NewsItemModel>
    {
    }
}