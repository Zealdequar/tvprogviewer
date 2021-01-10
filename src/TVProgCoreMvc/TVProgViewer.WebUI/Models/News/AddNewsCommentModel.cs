using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Models.News
{
    public partial record AddNewsCommentModel : BaseTvProgModel
    {
        [TvProgResourceDisplayName("News.Comments.CommentTitle")]
        public string CommentTitle { get; set; }

        [TvProgResourceDisplayName("News.Comments.CommentText")]
        public string CommentText { get; set; }

        public bool DisplayCaptcha { get; set; }
    }
}