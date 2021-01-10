using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Services.Caching.CachingDefaults;

namespace TVProgViewer.Services.Caching.CacheEventConsumers.Catalog
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
        protected override void ClearCache(Category entity)
        {
            var prefix = TvProgCatalogCachingDefaults.CategoriesByParentCategoryPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);
            prefix = TvProgCatalogCachingDefaults.CategoriesByParentCategoryPrefixCacheKey.ToCacheKey(entity.ParentCategoryId);
            RemoveByPrefix(prefix);

            prefix = TvProgCatalogCachingDefaults.CategoriesChildIdentifiersPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);
            prefix = TvProgCatalogCachingDefaults.CategoriesChildIdentifiersPrefixCacheKey.ToCacheKey(entity.ParentCategoryId);
            RemoveByPrefix(prefix);

            prefix = TvProgCatalogCachingDefaults.ProductCategoriesByCategoryPrefixCacheKey.ToCacheKey(entity);
            RemoveByPrefix(prefix);

            RemoveByPrefix(TvProgCatalogCachingDefaults.CategoriesDisplayedOnHomepagePrefixCacheKey);
            RemoveByPrefix(TvProgCatalogCachingDefaults.CategoriesAllPrefixCacheKey);
            RemoveByPrefix(TvProgCatalogCachingDefaults.CategoryBreadcrumbPrefixCacheKey);
            
            RemoveByPrefix(TvProgCatalogCachingDefaults.CategoryNumberOfProductsPrefixCacheKey);

            RemoveByPrefix(TvProgDiscountCachingDefaults.DiscountCategoryIdsPrefixCacheKey);
        }
    }
}
