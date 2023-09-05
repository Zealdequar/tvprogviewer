using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Boards
{
    public partial record ForumTopicRowModel : BaseTvProgModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string SeName { get; set; }
        public int LastPostId { get; set; }

        public int NumPosts { get; set; }
        public int Views { get; set; }
        public int Votes { get; set; }
        public int NumReplies { get; set; }
        public ForumTopicType ForumTopicType { get; set; }

        public int UserId { get; set; }
        public bool AllowViewingProfiles { get; set; }
        public string UserName { get; set; }

        //posts
        public int TotalPostPages { get; set; }
    }
}