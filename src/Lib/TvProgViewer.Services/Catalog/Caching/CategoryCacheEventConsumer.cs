using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Caching;
using TvProgViewer.Services.Discounts;

namespace TvProgViewer.Services.Catalog.Caching
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
        /// <param name="entityEventType">Entity event type</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(Category entity, EntityEventType entityEventType)
        {
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesByParentCategoryPrefix, entity);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesByParentCategoryPrefix, entity.ParentCategoryId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesChildIdsPrefix, entity);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesChildIdsPrefix, entity.ParentCategoryId);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoriesHomepagePrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoryBreadcrumbPrefix);
            await RemoveByPrefixAsync(TvProgCatalogDefaults.CategoryTvChannelsNumberPrefix);
            await RemoveByPrefixAsync(TvProgDiscountDefaults.CategoryIdsPrefix);

            if (entityEventType == EntityEventType.Delete)
                await RemoveAsync(TvProgCatalogDefaults.SpecificationAttributeOptionsByCategoryCacheKey, entity);

            await RemoveAsync(TvProgDiscountDefaults.AppliedDiscountsCacheKey, nameof(Category), entity);

            await base.ClearCacheAsync(entity, entityEventType);
        }
    }
}
