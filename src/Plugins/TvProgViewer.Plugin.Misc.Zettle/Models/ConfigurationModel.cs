using System;
using System.ComponentModel.DataAnnotations;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.Plugin.Misc.Zettle.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseTvProgModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            Account = new AccountModel();
            Import = new ImportModel();
            SyncRecordSearchModel = new SyncRecordSearchModel();
        }

        #endregion

        #region Properties

        public bool Connected { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.ClientId")]
        public string ClientId { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.ApiKey")]
        [DataType(DataType.Password)]
        public string ApiKey { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.DisconnectOnUninstall")]
        public bool DisconnectOnUninstall { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.AutoSyncEnabled")]
        public bool AutoSyncEnabled { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.AutoSyncPeriod")]
        public int AutoSyncPeriod { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.DeleteBeforeImport")]
        public bool DeleteBeforeImport { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.SyncEnabled")]
        public bool SyncEnabled { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.PriceSyncEnabled")]
        public bool PriceSyncEnabled { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.ImageSyncEnabled")]
        public bool ImageSyncEnabled { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.InventoryTrackingEnabled")]
        public bool InventoryTrackingEnabled { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.DefaultTaxEnabled")]
        public bool DefaultTaxEnabled { get; set; }

        [TvProgResourceDisplayName("Plugins.Misc.Zettle.Configuration.Fields.DiscountSyncEnabled")]
        public bool DiscountSyncEnabled { get; set; }

        public AccountModel Account { get; set; }

        public ImportModel Import { get; set; }

        public SyncRecordSearchModel SyncRecordSearchModel { get; set; }

        #endregion

        #region Nested classes

        /// <summary>
        /// Represents an account model
        /// </summary>
        public record AccountModel : BaseTvProgModel
        {
            #region Properties

            public bool Accepted { get; set; }

            public string UserStatus { get; set; }

            [TvProgResourceDisplayName("Plugins.Misc.Zettle.Account.Fields.Name")]
            public string Name { get; set; }

            [TvProgResourceDisplayName("Plugins.Misc.Zettle.Account.Fields.Currency")]
            public string Currency { get; set; }

            [TvProgResourceDisplayName("Plugins.Misc.Zettle.Account.Fields.TaxationType")]
            public string TaxationType { get; set; }

            [TvProgResourceDisplayName("Plugins.Misc.Zettle.Account.Fields.TaxationMode")]
            public string TaxationMode { get; set; }

            #endregion
        }

        /// <summary>
        /// Represents an import model
        /// </summary>
        public record ImportModel : BaseTvProgModel
        {
            #region Properties

            public bool Active { get; set; }

            [TvProgResourceDisplayName("Plugins.Misc.Zettle.Import.Fields.StartDate")]
            public DateTime? StartDate { get; set; }

            [TvProgResourceDisplayName("Plugins.Misc.Zettle.Import.Fields.EndDate")]
            public DateTime? EndDate { get; set; }

            [TvProgResourceDisplayName("Plugins.Misc.Zettle.Import.Fields.State")]
            public string State { get; set; }

            [TvProgResourceDisplayName("Plugins.Misc.Zettle.Import.Fields.Items")]
            public string Items { get; set; }

            #endregion
        }

        #endregion
    }
}