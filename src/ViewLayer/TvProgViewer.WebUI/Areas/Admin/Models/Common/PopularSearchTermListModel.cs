using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents a popular search term list model
    /// </summary>
    public partial record PopularSearchTermListModel : BasePagedListModel<PopularSearchTermModel>
    {
    }
}