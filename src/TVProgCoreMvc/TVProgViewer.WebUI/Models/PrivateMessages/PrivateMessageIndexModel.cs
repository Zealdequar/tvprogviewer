using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.PrivateMessages
{
    public partial record PrivateMessageIndexModel : BaseTvProgModel
    {
        public int InboxPage { get; set; }
        public int SentItemsPage { get; set; }
        public bool SentItemsTabSelected { get; set; }
    }
}