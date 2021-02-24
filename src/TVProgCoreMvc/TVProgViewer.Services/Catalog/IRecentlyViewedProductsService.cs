using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;

namespace TVProgViewer.Services.Catalog
{
    /// <summary>
    /// Recently viewed products service
    /// </summary>
    public partial interface IRecentlyViewedProductsService
    {
        /// <summary>
        /// Gets a "recently viewed products" list
        /// </summary>
        /// <param name="number">Number of products to load</param>
        /// <returns>"recently viewed products" list</returns>
        Task<IList<Product>> GetRecentlyViewedProductsAsync(int number);

        /// <summary>
        /// Adds a product to a recently viewed products list
        /// </summary>
        /// <param name="productId">Product identifier</param>
        Task AddProductToRecentlyViewedListAsync(int productId);
    }
}
