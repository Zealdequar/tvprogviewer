using System;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.News
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