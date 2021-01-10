using System;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.PrivateMessages
{
    public partial record PrivateMessageModel : BaseTvProgEntityModel
    {
        public int FromUserId { get; set; }
        public string UserFromName { get; set; }
        public bool AllowViewingFromProfile { get; set; }

        public int ToUserId { get; set; }
        public string UserToName { get; set; }
        public bool AllowViewingToProfile { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public bool IsRead { get; set; }
    }
}