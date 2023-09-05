using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Shipping.UPS.Models
{
    public record UPSShippingModel : BaseTvProgModel
    {
        #region Ctor

        public UPSShippingModel()
        {
            CarrierServices = new List<string>();
            AvailableCarrierServices = new List<SelectListItem>();
            AvailableUserClassifications = new List<SelectListItem>();
            AvailablePickupTypes = new List<SelectListItem>();
            AvailablePackagingTypes = new List<SelectListItem>();
            AvaliablePackingTypes = new List<SelectListItem>();
            AvaliableWeightTypes = new List<SelectListItem>();
            AvaliableDimensionsTypes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.AccountNumber")]
        public string AccountNumber { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.AccessKey")]
        public string AccessKey { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.Username")]
        public string Username { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.Password")]
        public string Password { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.AdditionalHandlingCharge")]
        public decimal AdditionalHandlingCharge { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.InsurePackage")]
        public bool InsurePackage { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.UserClassification")]
        public int UserClassification { get; set; }
        public IList<SelectListItem> AvailableUserClassifications { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.PickupType")]
        public int PickupType { get; set; }
        public IList<SelectListItem> AvailablePickupTypes { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.PackagingType")]
        public int PackagingType { get; set; }
        public IList<SelectListItem> AvailablePackagingTypes { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.AvailableCarrierServices")]
        public IList<SelectListItem> AvailableCarrierServices { get; set; }
        public IList<string> CarrierServices { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.SaturdayDeliveryEnabled")]
        public bool SaturdayDeliveryEnabled { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.PassDimensions")]
        public bool PassDimensions { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.PackingPackageVolume")]
        public int PackingPackageVolume { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.PackingType")]
        public int PackingType { get; set; }
        public IList<SelectListItem> AvaliablePackingTypes { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.Tracing")]
        public bool Tracing { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.WeightType")]
        public string WeightType { get; set; }
        public IList<SelectListItem> AvaliableWeightTypes { get; set; }

        [TvProgResourceDisplayName("Plugins.Shipping.UPS.Fields.DimensionsType")]
        public string DimensionsType { get; set; }
        public IList<SelectListItem> AvaliableDimensionsTypes { get; set; }

        #endregion
    }
}