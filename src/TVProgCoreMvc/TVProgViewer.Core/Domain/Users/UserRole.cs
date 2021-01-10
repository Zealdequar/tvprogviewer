namespace TVProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Represents a User role
    /// </summary>
    public partial class UserRole : BaseEntity
    {
        /// <summary>
        /// Gets or sets the User role name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the User role is marked as free shipping
        /// </summary>
        public bool FreeShipping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the User role is marked as tax exempt
        /// </summary>
        public bool TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the User role is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the User role is system
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Gets or sets the User role system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Users must change passwords after a specified time
        /// </summary>
        public bool EnablePasswordLifetime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Users of this role have other tax display type chosen instead of the default one
        /// </summary>
        public bool OverrideTaxDisplayType { get; set; }

        /// <summary>
        /// Gets or sets identifier of the default tax display type (used only with "OverrideTaxDisplayType" enabled)
        /// </summary>
        public int DefaultTaxDisplayTypeId { get; set; }

        /// <summary>
        /// Gets or sets a product identifier that is required by this User role. 
        /// A User is added to this User role once a specified product is purchased.
        /// </summary>
        public int PurchasedWithProductId { get; set; }
    }
}