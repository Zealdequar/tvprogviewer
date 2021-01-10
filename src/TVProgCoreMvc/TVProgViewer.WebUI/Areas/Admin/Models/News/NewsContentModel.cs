using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.News
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
        }

        #endregion

        #region Properties

        public NewsItemSearchModel NewsItems { get; set; }

        public NewsCommentSearchModel NewsComments { get; set; }

        #endregion
    }
}
