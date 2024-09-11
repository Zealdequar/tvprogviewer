using System;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Misc.Zettle.Models
{
    /// <summary>
    /// Represents a synchronization record model
    /// </summary>
    public record SyncRecordModel : BaseTvProgEntityModel
    {
        #region Properties

        public bool Active { get; set; }

        public int TvChannelId { get; set; }

        public string TvChannelName { get; set; }

        public bool PriceSyncEnabled { get; set; }

        public bool ImageSyncEnabled { get; set; }

        public bool InventoryTrackingEnabled { get; set; }

        public DateTime? UpdatedDate { get; set; }

        #endregion
    }
}