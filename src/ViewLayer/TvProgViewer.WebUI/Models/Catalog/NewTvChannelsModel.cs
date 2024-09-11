using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a new tvChannels model
    /// </summary>
    public partial record NewTvChannelsModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the catalog tvChannels model
        /// </summary>
        public CatalogTvChannelsModel CatalogTvChannelsModel { get; set; }

        public string MetaKeywords { get; set; }
        public string MetaDescription { get;set; }

        #endregion

        #region Ctor

        public NewTvChannelsModel()
        {
            CatalogTvChannelsModel = new CatalogTvChannelsModel();
        }

        #endregion
    }
}