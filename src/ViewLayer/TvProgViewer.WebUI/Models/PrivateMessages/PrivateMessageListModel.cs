using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Models.Common;

namespace TvProgViewer.WebUI.Models.PrivateMessages
{
    public partial record PrivateMessageListModel : BaseTvProgModel
    {
        public IList<PrivateMessageModel> Messages { get; set; }
        public PagerModel PagerModel { get; set; }
    }
}