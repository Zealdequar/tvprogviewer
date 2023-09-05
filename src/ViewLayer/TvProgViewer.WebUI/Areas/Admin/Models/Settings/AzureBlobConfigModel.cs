using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents Azure Blob storage configuration model
    /// </summary>
    public partial record AzureBlobConfigModel : BaseTvProgModel, IConfigModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.ConnectionString")]
        public string ConnectionString { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.ContainerName")]
        public string ContainerName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.EndPoint")]
        public string EndPoint { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.AppendContainerName")]
        public bool AppendContainerName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.StoreDataProtectionKeys")]
        public bool StoreDataProtectionKeys { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysContainerName")]
        public string DataProtectionKeysContainerName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.AppSettings.AzureBlob.DataProtectionKeysVaultId")]
        public string DataProtectionKeysVaultId { get; set; }

        #endregion
    }
}