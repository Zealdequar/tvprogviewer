using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Messages
{
    /// <summary>
    /// Represents a newsletter subscription list model
    /// </summary>
    public partial record NewsletterSubscriptionListModel : BasePagedListModel<NewsletterSubscriptionModel>
    {
    }
}