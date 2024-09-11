namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents an attribute value type
    /// </summary>
    public enum AttributeValueType
    {
        /// <summary>
        /// Simple attribute value
        /// </summary>
        Simple = 0,

        /// <summary>
        /// Associated to a tvChannel (used when configuring bundled tvChannels)
        /// </summary>
        AssociatedToTvChannel = 10,
    }
}
