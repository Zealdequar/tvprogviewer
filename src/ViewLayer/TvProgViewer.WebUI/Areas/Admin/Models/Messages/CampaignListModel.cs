using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a campaign list model
    /// </summary>
    public partial record CampaignListModel : BasePagedListModel<CampaignModel>
    {
    }
}