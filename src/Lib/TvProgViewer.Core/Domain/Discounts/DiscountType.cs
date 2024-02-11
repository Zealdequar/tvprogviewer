namespace TvProgViewer.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount type
    /// </summary>
    public enum DiscountType
    {
        /// <summary>
        /// Assigned to order total 
        /// </summary>
        AssignedToOrderTotal = 1,

        /// <summary>
        /// Assigned to tvchannels (SKUs)
        /// </summary>
        AssignedToSkus = 2,

        /// <summary>
        /// Assigned to categories (all tvchannels in a category)
        /// </summary>
        AssignedToCategories = 5,

        /// <summary>
        /// Assigned to manufacturers (all tvchannels of a manufacturer)
        /// </summary>
        AssignedToManufacturers = 6,

        /// <summary>
        /// Assigned to shipping
        /// </summary>
        AssignedToShipping = 10,

        /// <summary>
        /// Assigned to order subtotal
        /// </summary>
        AssignedToOrderSubTotal = 20
    }
}
