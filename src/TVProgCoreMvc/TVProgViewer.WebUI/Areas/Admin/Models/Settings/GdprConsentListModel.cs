using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a GDPR consent list model
    /// </summary>
    public partial record GdprConsentListModel : BasePagedListModel<GdprConsentModel>
    {
    }
}