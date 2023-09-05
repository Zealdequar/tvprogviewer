using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Tax.Avalara.Models.Tax
{
    /// <summary>
    /// Represents an extended tax category model
    /// </summary>
    public record TaxCategoryModel : TvProgViewer.WebUI.Areas.Admin.Models.Tax.TaxCategoryModel
    {
        #region Properties

        public string Description { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.Avalara.Fields.TaxCodeType")]
        public string TypeId { get; set; }
        public string Type { get; set; }

        #endregion
    }
}