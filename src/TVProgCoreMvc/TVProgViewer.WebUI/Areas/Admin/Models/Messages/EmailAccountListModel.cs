using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents an email account list model
    /// </summary>
    public partial record EmailAccountListModel : BasePagedListModel<EmailAccountModel>
    {
    }
}