using System;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.News
{
    public partial record NewsCommentModel : BaseTvProgEntityModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserAvatarUrl { get; set; }

        public string CommentTitle { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AllowViewingProfiles { get; set; }
    }
}