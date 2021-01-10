using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents a popular search term list model
    /// </summary>
    public partial record PopularSearchTermListModel : BasePagedListModel<PopularSearchTermModel>
    {
    }
}