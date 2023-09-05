using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents distributed cache configuration model
    /// </summary>
    public partial record DistributedCacheConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.DistributedCacheType")]
        public SelectList DistributedCacheTypeValues { get; set; }
        public int DistributedCacheType { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.Enabled")]
        public bool Enabled { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.ConnectionString")]
        public string ConnectionString { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.SchemaName")]
        public string SchemaName { get; set; } = "dbo";

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.DistributedCache.TableName")]
        public string TableName { get; set; } = "DistributedCache";

        #endregion
    }
}