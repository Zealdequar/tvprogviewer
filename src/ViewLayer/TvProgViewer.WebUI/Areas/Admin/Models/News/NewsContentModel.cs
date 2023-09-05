using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.News
{
    /// <summary>
    /// Represents a news content model
    /// </summary>
    public partial record NewsContentModel : BaseTvProgModel
    {
        #region Ctor

        public NewsContentModel()
        {
            NewsItems = new NewsItemSearchModel();
            NewsComments = new NewsCommentSearchModel();
            SearchTitle = new NewsItemSearchModel().SearchTitle;
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.ContentManagement.News.NewsItems.List.SearchTitle")]
        public string SearchTitle { get; set; }

        public NewsItemSearchModel NewsItems { get; set; }

        public NewsCommentSearchModel NewsComments { get; set; }

        #endregion
    }
}
