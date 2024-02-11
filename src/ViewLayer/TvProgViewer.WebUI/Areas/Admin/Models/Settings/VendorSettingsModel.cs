using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.WebUI.Areas.Admin.Models.Vendors;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a vendor settings model
    /// </summary>
    public partial record VendorSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Ctor

        public VendorSettingsModel()
        {
            VendorAttributeSearchModel = new VendorAttributeSearchModel();
        }

        #endregion

        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.VendorsBlockItemsToDisplay")]
        public int VendorsBlockItemsToDisplay { get; set; }
        public bool VendorsBlockItemsToDisplay_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.ShowVendorOnTvChannelDetailsPage")]
        public bool ShowVendorOnTvChannelDetailsPage { get; set; }
        public bool ShowVendorOnTvChannelDetailsPage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowUsersToContactVendors")]
        public bool AllowUsersToContactVendors { get; set; }
        public bool AllowUsersToContactVendors_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowUsersToApplyForVendorAccount")]
        public bool AllowUsersToApplyForVendorAccount { get; set; }
        public bool AllowUsersToApplyForVendorAccount_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.TermsOfServiceEnabled")]
        public bool TermsOfServiceEnabled { get; set; }
        public bool TermsOfServiceEnabled_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowSearchByVendor")]
        public bool AllowSearchByVendor { get; set; }
        public bool AllowSearchByVendor_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowVendorsToEditInfo")]
        public bool AllowVendorsToEditInfo { get; set; }
        public bool AllowVendorsToEditInfo_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.NotifyStoreOwnerAboutVendorInformationChange")]
        public bool NotifyStoreOwnerAboutVendorInformationChange { get; set; }
        public bool NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.MaximumTvChannelNumber")]
        public int MaximumTvChannelNumber { get; set; }
        public bool MaximumTvChannelNumber_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.AllowVendorsToImportTvChannels")]
        public bool AllowVendorsToImportTvChannels { get; set; }
        public bool AllowVendorsToImportTvChannels_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.Vendor.ShowVendorOnOrderDetailsPage")]
        public bool ShowVendorOnOrderDetailsPage { get; set; }
        public bool ShowVendorOnOrderDetailsPage_OverrideForStore { get; set; }

        public VendorAttributeSearchModel VendorAttributeSearchModel { get; set; }

        #endregion
    }
}