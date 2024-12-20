﻿using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a display default footer item settings model
    /// </summary>
    public partial record DisplayDefaultFooterItemSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplaySitemapFooterItem")]
        public bool DisplaySitemapFooterItem { get; set; }
        public bool DisplaySitemapFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayContactUsFooterItem")]
        public bool DisplayContactUsFooterItem { get; set; }
        public bool DisplayContactUsFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayTvChannelSearchFooterItem")]
        public bool DisplayTvChannelSearchFooterItem { get; set; }
        public bool DisplayTvChannelSearchFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewsFooterItem")]
        public bool DisplayNewsFooterItem { get; set; }
        public bool DisplayNewsFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayBlogFooterItem")]
        public bool DisplayBlogFooterItem { get; set; }
        public bool DisplayBlogFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayForumsFooterItem")]
        public bool DisplayForumsFooterItem { get; set; }
        public bool DisplayForumsFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayRecentlyViewedTvChannelsFooterItem")]
        public bool DisplayRecentlyViewedTvChannelsFooterItem { get; set; }
        public bool DisplayRecentlyViewedTvChannelsFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayCompareTvChannelsFooterItem")]
        public bool DisplayCompareTvChannelsFooterItem { get; set; }
        public bool DisplayCompareTvChannelsFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayNewTvChannelsFooterItem")]
        public bool DisplayNewTvChannelsFooterItem { get; set; }
        public bool DisplayNewTvChannelsFooterItem_OverrideForStore { get; set; }       

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayUserInfoFooterItem")]
        public bool DisplayUserInfoFooterItem { get; set; }
        public bool DisplayUserInfoFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayUserOrdersFooterItem")]
        public bool DisplayUserOrdersFooterItem { get; set; }
        public bool DisplayUserOrdersFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayUserAddressesFooterItem")]
        public bool DisplayUserAddressesFooterItem { get; set; }
        public bool DisplayUserAddressesFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayShoppingCartFooterItem")]
        public bool DisplayShoppingCartFooterItem { get; set; }
        public bool DisplayShoppingCartFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayWishlistFooterItem")]
        public bool DisplayWishlistFooterItem { get; set; }
        public bool DisplayWishlistFooterItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultFooterItemSettingsModel.DisplayApplyVendorAccountFooterItem")]
        public bool DisplayApplyVendorAccountFooterItem { get; set; }
        public bool DisplayApplyVendorAccountFooterItem_OverrideForStore { get; set; }

        #endregion
    }
}