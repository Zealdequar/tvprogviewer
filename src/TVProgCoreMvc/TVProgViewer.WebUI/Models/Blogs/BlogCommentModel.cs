using System;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.Blogs
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