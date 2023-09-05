using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliate list model
    /// </summary>
    public partial record AffiliateListModel : BasePagedListModel<AffiliateModel>
    {
    }
}