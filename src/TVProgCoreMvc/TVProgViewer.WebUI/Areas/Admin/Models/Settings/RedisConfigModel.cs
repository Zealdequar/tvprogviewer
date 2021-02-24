using System.ComponentModel.DataAnnotations;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents Redis configuration model
    /// </summary>
    public partial record RedisConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Redis.Enabled")]
        public bool Enabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Redis.ConnectionString")]
        public string ConnectionString { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Redis.DatabaseId")]
        [UIHint("Int32Nullable")]
        public int? DatabaseId { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Redis.UseCaching")]
        public bool UseCaching { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Redis.StoreDataProtectionKeys")]
        public bool StoreDataProtectionKeys { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Redis.StorePluginsInfo")]
        public bool StorePluginsInfo { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.Redis.IgnoreTimeoutException")]
        public bool IgnoreTimeoutException { get; set; }

        #endregion
    }
}