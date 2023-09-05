using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Models.Catalog
{
    /// <summary>
    /// Represents a new products model
    /// </summary>
    public partial record NewProductsModel : BaseTvProgModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the catalog products model
        /// </summary>
        public CatalogProductsModel CatalogProductsModel { get; set; }

        #endregion

        #region Ctor

        public NewProductsModel()
        {
            CatalogProductsModel = new CatalogProductsModel();
        }

        #endregion
    }
}