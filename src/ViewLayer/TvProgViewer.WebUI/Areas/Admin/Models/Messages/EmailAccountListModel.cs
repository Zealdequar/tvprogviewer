using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents an email account list model
    /// </summary>
    public partial record EmailAccountListModel : BasePagedListModel<EmailAccountModel>
    {
    }
}