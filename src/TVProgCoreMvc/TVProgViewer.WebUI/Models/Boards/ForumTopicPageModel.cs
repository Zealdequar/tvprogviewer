using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Boards
{
    public partial record ForumTopicPageModel : BaseTvProgModel
    {
        public ForumTopicPageModel()
        {
            ForumPostModels = new List<ForumPostModel>();
        }

        public int Id { get; set; }
        public string Subject { get; set; }
        public string SeName { get; set; }

        public string WatchTopicText { get; set; }

        public bool IsUserAllowedToEditTopic { get; set; }
        public bool IsUserAllowedToDeleteTopic { get; set; }
        public bool IsUserAllowedToMoveTopic { get; set; }
        public bool IsUserAllowedToSubscribe { get; set; }

        public IList<ForumPostModel> ForumPostModels { get; set; }
        public int PostsPageIndex { get; set; }
        public int PostsPageSize { get; set; }
        public int PostsTotalRecords { get; set; }
    }
}