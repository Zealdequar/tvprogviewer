﻿using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a display default menu item settings model
    /// </summary>
    public partial record DisplayDefaultMenuItemSettingsModel : BaseTvProgModel, ISettingsModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem")]
        public bool DisplayHomepageMenuItem { get; set; }
        public bool DisplayHomepageMenuItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayNewTvChannelsMenuItem")]
        public bool DisplayNewTvChannelsMenuItem { get; set; }
        public bool DisplayNewTvChannelsMenuItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayTvChannelSearchMenuItem")]
        public bool DisplayTvChannelSearchMenuItem { get; set; }
        public bool DisplayTvChannelSearchMenuItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem")]
        public bool DisplayUserInfoMenuItem { get; set; }
        public bool DisplayUserInfoMenuItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem")]
        public bool DisplayBlogMenuItem { get; set; }
        public bool DisplayBlogMenuItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem")]
        public bool DisplayForumsMenuItem { get; set; }
        public bool DisplayForumsMenuItem_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem")]
        public bool DisplayContactUsMenuItem { get; set; }
        public bool DisplayContactUsMenuItem_OverrideForStore { get; set; }

        #endregion
    }
}