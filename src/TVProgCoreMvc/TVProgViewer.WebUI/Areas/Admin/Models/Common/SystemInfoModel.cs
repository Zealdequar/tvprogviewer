using System;
using System.Collections.Generic;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Common
{
    public partial record SystemInfoModel : BaseTvProgModel
    {
        public SystemInfoModel()
        {
            Headers = new List<HeaderModel>();
            LoadedAssemblies = new List<LoadedAssembly>();
        }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.ASPNETInfo")]
        public string AspNetInfo { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.IsFullTrust")]
        public string IsFullTrust { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.TvProgVersion")]
        public string TvProgVersion { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.OperatingSystem")]
        public string OperatingSystem { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.ServerLocalTime")]
        public DateTime ServerLocalTime { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.ServerTimeZone")]
        public string ServerTimeZone { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.UTCTime")]
        public DateTime UtcTime { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.CurrentUserTime")]
        public DateTime CurrentUserTime { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.CurrentStaticCacheManager")]
        public string CurrentStaticCacheManager { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.HTTPHOST")]
        public string HttpHost { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.Headers")]
        public IList<HeaderModel> Headers { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.LoadedAssemblies")]
        public IList<LoadedAssembly> LoadedAssemblies { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.RedisEnabled")]
        public bool RedisEnabled { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.UseRedisToStoreDataProtectionKeys")]
        public bool UseRedisToStoreDataProtectionKeys { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.UseRedisForCaching")]
        public bool UseRedisForCaching { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.UseRedisToStorePluginsInfo")]
        public bool UseRedisToStorePluginsInfo { get; set; }

        [TvProgResourceDisplayName("Admin.System.SystemInfo.AzureBlobStorageEnabled")]
        public bool AzureBlobStorageEnabled { get; set; }

        public partial record HeaderModel : BaseTvProgModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public partial record LoadedAssembly : BaseTvProgModel
        {
            public string FullName { get; set; }
            public string Location { get; set; }
            public bool IsDebug { get; set; }
            public DateTime? BuildDate { get; set; }
        }
    }
}