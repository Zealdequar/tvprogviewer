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
        /// Assigned to tvChannels (SKUs)
        /// </summary>
        AssignedToSkus = 2,

        /// <summary>
        /// Assigned to categories (all tvChannels in a category)
        /// </summary>
        AssignedToCategories = 5,

        /// <summary>
        /// Assigned to manufacturers (all tvChannels of a manufacturer)
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
