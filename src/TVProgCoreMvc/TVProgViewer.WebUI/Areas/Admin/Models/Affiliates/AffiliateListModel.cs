using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliate list model
    /// </summary>
    public partial record AffiliateListModel : BasePagedListModel<AffiliateModel>
    {
    }
}