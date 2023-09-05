﻿using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a cache configuration model
    /// </summary>
    public partial record CacheConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Cache.DefaultCacheTime")]
        public int DefaultCacheTime { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Cache.ShortTermCacheTime")]
        public int ShortTermCacheTime { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Cache.BundledFilesCacheTime")]
        public int BundledFilesCacheTime { get; set; }

        #endregion
    }
}