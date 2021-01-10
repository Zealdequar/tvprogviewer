using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.WebUI.Models.Common;

namespace TVProgViewer.WebUI.Models.Boards
{
    public partial record UserForumSubscriptionsModel : BaseTvProgModel
    {
        public UserForumSubscriptionsModel()
        {
            ForumSubscriptions = new List<ForumSubscriptionModel>();
        }

        public IList<ForumSubscriptionModel> ForumSubscriptions { get; set; }
        public PagerModel PagerModel { get; set; }

        #region Nested recordes

        public partial record ForumSubscriptionModel : BaseTvProgEntityModel
        {
            public int ForumId { get; set; }
            public int ForumTopicId { get; set; }
            public bool TopicSubscription { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
        }

        #endregion
    }
}