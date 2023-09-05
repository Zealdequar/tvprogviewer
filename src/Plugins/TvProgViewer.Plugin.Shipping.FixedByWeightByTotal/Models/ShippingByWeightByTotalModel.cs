using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Shipping.FixedByWeightByTotal.Models
{
    public record ShippingByWeightByTotalModel : BaseTvProgEntityModel
    {
        public ShippingByWeightByTotalModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableShippingMethods = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Store")]
        public int StoreId { get; set; }
        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Store")]
        public string StoreName { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse")]
        public int WarehouseId { get; set; }
        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse")]
        public string WarehouseName { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Country")]
        public int CountryId { get; set; }
        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Country")]
        public string CountryName { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince")]
        public string StateProvinceName { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Zip")]
        public string Zip { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod")]
        public int ShippingMethodId { get; set; }
        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod")]
        public string ShippingMethodName { get; set; }

        [UIHint("Int32Nullable")]
        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.TransitDays")]
        public int? TransitDays { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightFrom")]
        public decimal WeightFrom { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.WeightTo")]
        public decimal WeightTo { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalFrom")]
        public decimal OrderSubtotalFrom { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.OrderSubtotalTo")]
        public decimal OrderSubtotalTo { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.AdditionalFixedCost")]
        public decimal AdditionalFixedCost { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.PercentageRateOfSubtotal")]
        public decimal PercentageRateOfSubtotal { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.RatePerWeightUnit")]
        public decimal RatePerWeightUnit { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.LowerWeightLimit")]
        public decimal LowerWeightLimit { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.DataHtml")]
        public string DataHtml { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }
        public string BaseWeightIn { get; set; }

        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
        public IList<SelectListItem> AvailableShippingMethods { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
    }
}