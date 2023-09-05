using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TvProgViewer.Plugin.Shipping.FixedByWeightByTotal.Models
{
    public record ConfigurationModel : BaseSearchModel
    {
        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.LimitMethodsToCreated")]
        public bool LimitMethodsToCreated { get; set; }

        public bool ShippingByWeightByTotalEnabled { get; set; }

        public ConfigurationModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableShippingMethods = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Store")]
        public int SearchStoreId { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Warehouse")]
        public int SearchWarehouseId { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Country")]
        public int SearchCountryId { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.StateProvince")]
        public int SearchStateProvinceId { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.Zip")]
        public string SearchZip { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.FixedByWeightByTotal.Fields.ShippingMethod")]
        public int SearchShippingMethodId { get; set; }
        
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
        public IList<SelectListItem> AvailableShippingMethods { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableWarehouses { get; set; }
    }
}