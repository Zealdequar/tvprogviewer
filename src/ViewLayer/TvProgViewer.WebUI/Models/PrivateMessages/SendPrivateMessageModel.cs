using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.PrivateMessages
{
    public partial record SendPrivateMessageModel : BaseTvProgEntityModel
    {
        public int ToUserId { get; set; }
        public string UserToName { get; set; }
        public bool AllowViewingToProfile { get; set; }

        public int ReplyToMessageId { get; set; }
        
        public string Subject { get; set; }
        
        public string Message { get; set; }
    }
}