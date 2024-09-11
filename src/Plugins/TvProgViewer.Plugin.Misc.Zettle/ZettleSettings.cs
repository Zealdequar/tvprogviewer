using System.Collections.Generic;
using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Plugin.Misc.Zettle
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class ZettleSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the client identifier
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disconnect an app from an associated Zettle organisation when the plugin is uninstalled
        /// </summary>
        public bool DisconnectOnUninstall { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto synchronization is enabled
        /// </summary>
        public bool AutoSyncEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value how often (in minutes) auto synchronization will run
        /// </summary>
        public int AutoSyncPeriod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to delete library items before importing tvChannels
        /// </summary>
        public bool DeleteBeforeImport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync tvChannels by default
        /// </summary>
        public bool SyncEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync price for tvChannels by default
        /// </summary>
        public bool PriceSyncEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync images for tvChannels by default
        /// </summary>
        public bool ImageSyncEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track inventory for tvChannels by default
        /// </summary>
        public bool InventoryTrackingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to import tvChannels with default sales tax rate or VAT rate
        /// </summary>
        public bool DefaultTaxEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to import discounts (assigned to order subtotal only)
        /// </summary>
        public bool DiscountSyncEnabled { get; set; }

        #region Advanced

        /// <summary>
        /// Gets or sets a webhook URL
        /// </summary>
        public string WebhookUrl { get; set; }

        /// <summary>
        /// Gets or sets a webhook key used to verify that all incoming webhook messages originate from the service
        /// </summary>
        public string WebhookKey { get; set; }

        /// <summary>
        /// Gets or sets the latest import unique identifier as UUID version 1
        /// </summary>
        public string ImportId { get; set; }

        /// <summary>
        /// Gets or sets the number of tvChannels to import in one request (up to 2000 tvChannels per request)
        /// </summary>
        public int ImportTvChannelsNumber { get; set; }

        /// <summary>
        /// Gets or sets a period (in seconds) before the request times out
        /// </summary>
        public int? RequestTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to auto add (by events) records for synchronization
        /// </summary>
        public bool AutoAddRecordsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to log synchronization messages
        /// </summary>
        public bool LogSyncMessages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync tvChannels with categories
        /// </summary>
        public bool CategorySyncEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to clear synchronization records when credentials changed.
        /// If disabled, records will be keep and created in a new app on the first synchronization
        /// </summary>
        public bool ClearRecordsOnChangeCredentials { get; set; }

        /// <summary>
        /// Gets or sets the list of inventory balance changes unique identifiers as UUID version 1
        /// </summary>
        public List<string> InventoryTrackingIds { get; set; } = new();

        #endregion
    }
}