using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.PrivateMessages
{
    public partial record PrivateMessageIndexModel : BaseTvProgModel
    {
        public int InboxPage { get; set; }
        public int SentItemsPage { get; set; }
        public bool SentItemsTabSelected { get; set; }
    }
}