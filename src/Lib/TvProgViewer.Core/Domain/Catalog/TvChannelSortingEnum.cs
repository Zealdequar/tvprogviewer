namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents the tvchannel sorting
    /// </summary>
    public enum TvChannelSortingEnum
    {
        /// <summary>
        /// Position (display order)
        /// </summary>
        Position = 0,

        /// <summary>
        /// Name: A to Z
        /// </summary>
        NameAsc = 5,

        /// <summary>
        /// Name: Z to A
        /// </summary>
        NameDesc = 6,

        /*/// <summary>
        /// Price: Low to High
        /// </summary>
        PriceAsc = 10,

        /// <summary>
        /// Price: High to Low
        /// </summary>
        PriceDesc = 11,*/

        /// <summary>
        /// TvChannel creation date
        /// </summary>
        CreatedOn = 15,
    }
}