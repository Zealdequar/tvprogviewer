using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a category tvChannel search model
    /// </summary>
    public partial record CategoryTvChannelSearchModel : BaseSearchModel
    {
        #region Properties

        public int CategoryId { get; set; }

        #endregion
    }
}