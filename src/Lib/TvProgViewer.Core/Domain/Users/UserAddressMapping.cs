﻿namespace TvProgViewer.Core.Domain.Users
{
    /// <summary>
    /// Represents a user-address mapping class
    /// </summary>
    public partial class UserAddressMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the address identifier
        /// </summary>
        public int AddressId { get; set; }
    }
}