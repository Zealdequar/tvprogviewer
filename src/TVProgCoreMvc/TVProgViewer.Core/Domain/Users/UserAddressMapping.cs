namespace TVProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Represents a User-address mapping class
    /// </summary>
    public partial class UserAddressMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the User identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the address identifier
        /// </summary>
        public int AddressId { get; set; }
    }
}