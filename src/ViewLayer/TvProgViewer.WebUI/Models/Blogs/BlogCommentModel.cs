using System;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Blogs
{
    public partial record BlogCommentModel : BaseTvProgEntityModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserAvatarUrl { get; set; }

        public string CommentText { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AllowViewingProfiles { get; set; }
    }
}