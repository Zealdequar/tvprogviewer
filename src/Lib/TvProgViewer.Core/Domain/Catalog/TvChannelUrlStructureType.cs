namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents the tvchannel URL structure type enum
    /// </summary>
    public enum TvChannelUrlStructureType
    {
        /// <summary>
        /// TvChannel only (e.g. '/tvchannel-seo-name')
        /// </summary>
        TvChannel = 0,

        /// <summary>
        /// Category (the most nested), then tvchannel (e.g. '/category-seo-name/tvchannel-seo-name')
        /// </summary>
        CategoryTvChannel = 10,

        /// <summary>
        /// Manufacturer, then tvchannel (e.g. '/manufacturer-seo-name/tvchannel-seo-name')
        /// </summary>
        ManufacturerTvChannel = 20
    }
}