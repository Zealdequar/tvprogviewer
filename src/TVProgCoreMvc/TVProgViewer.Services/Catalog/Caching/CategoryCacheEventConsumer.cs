using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching;
using TVProgViewer.Services.Discounts;

namespace TVProgViewer.Services.Catalog.Caching
{
    /// <summary>
    /// Represents a category cache event consumer
    /// </summary>
    public partial class CategoryCacheEventConsumer : CacheEventConsumer<Category>
    {
        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        protected override async Task ClearCacheAsync(Category entity)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesByParentCategoryPrefix, entity);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesByParentCategoryPrefix, entity.ParentCategoryId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesChildIdsPrefix, entity);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesChildIdsPrefix, entity.ParentCategoryId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesHomepagePrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoryBreadcrumbPrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoryProductsNumberPrefix);
            await RemoveByPrefixAsync(TvProgDiscountDefaults.CategoryIdsPrefix);
        }
    }
}
