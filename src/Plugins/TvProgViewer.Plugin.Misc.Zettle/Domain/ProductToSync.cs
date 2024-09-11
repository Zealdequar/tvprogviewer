namespace TvProgViewer.Plugin.Misc.Zettle.Domain
{
    /// <summary>
    /// Represents the tvChannel details ready for synchronization
    /// </summary>
    public class TvChannelToSync
    {
        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier as UUID version 1 of tvChannel variant
        /// </summary>
        public string VariantUuid { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel SKU
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the category name
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the image URL
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync images for this tvChannel
        /// </summary>
        public bool ImageSyncEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to sync price for this tvChannel
        /// </summary>
        public bool PriceSyncEnabled { get; set; }
    }
}