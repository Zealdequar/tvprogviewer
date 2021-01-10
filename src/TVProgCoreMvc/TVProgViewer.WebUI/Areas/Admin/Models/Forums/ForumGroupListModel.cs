using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Forums
{
    /// <summary>
    /// Represents a forum group list model
    /// </summary>
    public partial record ForumGroupListModel : BasePagedListModel<ForumGroupModel>
    {
    }
}