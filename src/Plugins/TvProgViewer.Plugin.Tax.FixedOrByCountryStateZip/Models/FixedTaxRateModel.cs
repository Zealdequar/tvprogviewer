using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Tax.FixedOrByCountryStateZip.Models
{
    public record FixedTaxRateModel : BaseTvProgModel
    {
        public int TaxCategoryId { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.FixedOrByCountryStateZip.Fields.TaxCategoryName")]
        public string TaxCategoryName { get; set; }

        [TvProgResourceDisplayName("Plugins.Tax.FixedOrByCountryStateZip.Fields.Rate")]
        public decimal Rate { get; set; }
    }
}