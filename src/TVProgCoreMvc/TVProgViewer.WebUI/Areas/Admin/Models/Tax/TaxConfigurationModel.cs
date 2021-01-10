using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Tax
{
    /// <summary>
    /// Represents a tax configuration model
    /// </summary>
    public partial record TaxConfigurationModel : BaseTvProgModel
    {
        #region Ctor

        public TaxConfigurationModel()
        {
            TaxProviders = new TaxProviderSearchModel();
            TaxCategories = new TaxCategorySearchModel();
        }

        #endregion

        #region Properties

        public TaxProviderSearchModel TaxProviders { get; set; }

        public TaxCategorySearchModel TaxCategories { get; set; }

        #endregion
    }
}
