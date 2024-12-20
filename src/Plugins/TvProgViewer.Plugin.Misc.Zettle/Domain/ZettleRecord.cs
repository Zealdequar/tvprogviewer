﻿using System;
using TvProgViewer.Core;

namespace TvProgViewer.Plugin.Misc.Zettle.Domain
{
    /// <summary>
    /// Represents a record of the tvChannel and other details required for synchronization with Zettle
    /// </summary>
    public class ZettleRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets a value indicating whether the record is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1 of tvChannel variant
        /// </summary>
        public string VariantUuid { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel attribute combination identifier
        /// </summary>
        public int CombinationId { get; set; }

        /// <summary>
        /// Gets or sets the image URL
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets an operation type identifier
        /// </summary>
        public int OperationTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync price for this tvChannel
        /// </summary>
        public bool PriceSyncEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync images for this tvChannel
        /// </summary>
        public bool ImageSyncEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to track inventory for this tvChannel
        /// </summary>
        public bool InventoryTrackingEnabled { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was updated
        /// </summary>
        public DateTime? UpdatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets an operation type
        /// </summary>
        public OperationType OperationType
        {
            get => (OperationType)OperationTypeId;
            set => OperationTypeId = (int)value;
        }
    }
}