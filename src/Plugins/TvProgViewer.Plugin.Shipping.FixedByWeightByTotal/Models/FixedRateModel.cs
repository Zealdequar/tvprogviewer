using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace TvProgViewer.Plugin.Shipping.FixedByWeightByTotal.Models
{
    public record FixedRateModel : BaseTvProgModel
    {
        public int ShippingMethodId { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod")]
        public string ShippingMethodName { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Rate")]
        public decimal Rate { get; set; }

        [UIHint("Int32Nullable")]
        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.TransitDays")]
        public int? TransitDays { get; set; }
    }
}