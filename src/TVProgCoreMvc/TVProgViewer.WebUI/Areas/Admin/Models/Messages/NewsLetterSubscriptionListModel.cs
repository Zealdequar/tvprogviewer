using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a newsletter subscription list model
    /// </summary>
    public partial record NewsletterSubscriptionListModel : BasePagedListModel<NewsletterSubscriptionModel>
    {
    }
}