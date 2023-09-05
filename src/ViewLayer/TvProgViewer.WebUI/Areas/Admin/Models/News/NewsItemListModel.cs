using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a news item list model
    /// </summary>
    public partial record NewsItemListModel : BasePagedListModel<NewsItemModel>
    {
    }
}