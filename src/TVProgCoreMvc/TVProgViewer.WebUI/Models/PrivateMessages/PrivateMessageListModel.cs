using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.WebUI.Models.Common;

namespace TVProgViewer.WebUI.Models.PrivateMessages
{
    public partial record PrivateMessageListModel : BaseTvProgModel
    {
        public IList<PrivateMessageModel> Messages { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}