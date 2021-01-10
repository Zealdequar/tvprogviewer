using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a campaign list model
    /// </summary>
    public partial record CampaignListModel : BasePagedListModel<CampaignModel>
    {
    }
}