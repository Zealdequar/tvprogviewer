using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a new tvchannels model
    /// </summary>
    public partial record NewTvChannelsModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the catalog tvchannels model
        /// </summary>
        public CatalogTvChannelsModel CatalogTvChannelsModel { get; set; }

        #endregion

        #region Ctor

        public NewTvChannelsModel()
        {
            CatalogTvChannelsModel = new CatalogTvChannelsModel();
        }

        #endregion
    }
}