using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Common
{
    /// <summary>
    /// Represents a popular search term model
    /// </summary>
    public partial record PopularSearchTermModel : BaseTvProgModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.SearchTermReport.Keyword")]
        public string Keyword { get; set; }

        [TvProgResourceDisplayName("Admin.SearchTermReport.Count")]
        public int Count { get; set; }

        #endregion
    }
}
