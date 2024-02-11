using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;

namespace TvProgViewer.Services.Tax
{
    /// <summary>
    /// Represents a request to get tax rate
    /// </summary>
    public partial class TaxRateRequest
    {
        /// <summary>
        /// Gets or sets a user
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets a tvchannel
        /// </summary>
        public TvChannel TvChannel { get; set; }

        /// <summary>
        /// Gets or sets an address
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// Gets or sets a tax category identifier
        /// </summary>
        public int TaxCategoryId { get; set; }

        /// <summary>
        /// Gets or sets a price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets a current store identifier
        /// </summary>
        public int CurrentStoreId { get; set; }
    }
}
