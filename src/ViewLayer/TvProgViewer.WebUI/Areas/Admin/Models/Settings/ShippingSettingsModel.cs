﻿using TvProgViewer.WebUI.Areas.Admin.Models.Common;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a shipping settings model
    /// </summary>
    public partial record ShippingSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Ctor

        public ShippingSettingsModel()
        {
            ShippingOriginAddress = new AddressModel();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }
        
        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.ShipToSameAddress")]
        public bool ShipToSameAddress { get; set; }
        public bool ShipToSameAddress_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.AllowPickupInStore")]
        public bool AllowPickupInStore { get; set; }
        public bool AllowPickupInStore_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.DisplayPickupPointsOnMap")]
        public bool DisplayPickupPointsOnMap { get; set; }
        public bool DisplayPickupPointsOnMap_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.IgnoreAdditionalShippingChargeForPickupInStore")]
        public bool IgnoreAdditionalShippingChargeForPickupInStore { get; set; }
        public bool IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.GoogleMapsApiKey")]
        public string GoogleMapsApiKey { get; set; }
        public bool GoogleMapsApiKey_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.UseWarehouseLocation")]
        public bool UseWarehouseLocation { get; set; }
        public bool UseWarehouseLocation_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.NotifyUserAboutShippingFromMultipleLocations")]
        public bool NotifyUserAboutShippingFromMultipleLocations { get; set; }
        public bool NotifyUserAboutShippingFromMultipleLocations_OverrideForStore { get; set; }
        
        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXEnabled")]
        public bool FreeShippingOverXEnabled { get; set; }
        public bool FreeShippingOverXEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXValue")]
        public decimal FreeShippingOverXValue { get; set; }
        public bool FreeShippingOverXValue_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.FreeShippingOverXIncludingTax")]
        public bool FreeShippingOverXIncludingTax { get; set; }
        public bool FreeShippingOverXIncludingTax_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.EstimateShippingCartPageEnabled")]
        public bool EstimateShippingCartPageEnabled { get; set; }
        public bool EstimateShippingCartPageEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.EstimateShippingTvChannelPageEnabled")]
        public bool EstimateShippingTvChannelPageEnabled { get; set; }
        public bool EstimateShippingTvChannelPageEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.EstimateShippingCityNameEnabled")]
        public bool EstimateShippingCityNameEnabled { get; set; }
        public bool EstimateShippingCityNameEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToUsers")]
        public bool DisplayShipmentEventsToUsers { get; set; }
        public bool DisplayShipmentEventsToUsers_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.DisplayShipmentEventsToStoreOwner")]
        public bool DisplayShipmentEventsToStoreOwner { get; set; }
        public bool DisplayShipmentEventsToStoreOwner_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.HideShippingTotal")]
        public bool HideShippingTotal { get; set; }
        public bool HideShippingTotal_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.BypassShippingMethodSelectionIfOnlyOne")]
        public bool BypassShippingMethodSelectionIfOnlyOne { get; set; }
        public bool BypassShippingMethodSelectionIfOnlyOne_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.ConsiderAssociatedTvChannelsDimensions")]
        public bool ConsiderAssociatedTvChannelsDimensions { get; set; }
        public bool ConsiderAssociatedTvChannelsDimensions_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Shipping.ShippingSorting")]
        public int ShippingSorting { get; set; }
        public bool ShippingSorting_OverrideForStore { get; set; }
        public SelectList SortShippingValues { get; set; }

        public AddressModel ShippingOriginAddress { get; set; }
        public bool ShippingOriginAddress_OverrideForStore { get; set; }

        public string PrimaryStoreCurrencyCode { get; set; }

        #endregion
    }
}