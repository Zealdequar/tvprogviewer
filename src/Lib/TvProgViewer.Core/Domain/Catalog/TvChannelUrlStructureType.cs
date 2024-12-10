namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents the tvChannel URL structure type enum
    /// </summary>
    public enum TvChannelUrlStructureType
    {
        /// <summary>
        /// TvChannel only (e.g. '/tvchannel-seo-name')
        /// </summary>
        TvChannel = 0,

        /// <summary>
        /// Category (the most nested), then tvChannel (e.g. '/category-seo-name/tvchannel-seo-name')
        /// </summary>
        CategoryTvChannel = 10,

        /// <summary>
        /// Manufacturer, then tvChannel (e.g. '/manufacturer-seo-name/tvchannel-seo-name')
        /// </summary>
        ManufacturerTvChannel = 20
    }
}