using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a product-product tag mapping  cache event consumer
    /// </summary>
    public partial class ProductProductTagMappingCacheEventConsumer : CacheEventConsumer<ProductProductTagMapping>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(ProductProductTagMapping entity)
        {
            await RemoveAsync(TvProgCatalogDefaults.ProductTagsByProductCacheKey, entity.ProductId);
        }
    }
}