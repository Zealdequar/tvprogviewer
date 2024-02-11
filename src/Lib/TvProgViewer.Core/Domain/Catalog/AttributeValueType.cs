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
        /// Associated to a tvchannel (used when configuring bundled tvchannels)
        /// </summary>
        AssociatedToTvChannel = 10,
    }
}
