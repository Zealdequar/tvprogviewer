using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.News
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