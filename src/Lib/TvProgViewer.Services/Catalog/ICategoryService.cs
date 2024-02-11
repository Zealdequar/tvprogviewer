using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Category service interface
    /// </summary>
    public partial interface ICategoryService
    {
        /// <summary>
        /// Clean up category references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task ClearDiscountCategoryMappingAsync(Discount discount);

        /// <summary>
        /// Delete category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCategoryAsync(Category category);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        Task<IList<Category>> GetAllCategoriesAsync(int storeId = 0, bool showHidden = false);

        /// <summary>
        /// Gets all categories
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" tvchannels
        /// false - load only "Unpublished" tvchannels
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        Task<IPagedList<Category>> GetAllCategoriesAsync(string categoryName, int storeId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null);

        /// <summary>
        /// Gets all categories filtered by parent category identifier
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        Task<IList<Category>> GetAllCategoriesByParentCategoryIdAsync(int parentCategoryId, bool showHidden = false);

        /// <summary>
        /// Gets all categories displayed on the home page
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        Task<IList<Category>> GetAllCategoriesDisplayedOnHomepageAsync(bool showHidden = false);

        /// <summary>
        /// Get category identifiers to which a discount is applied
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <param name="user">User</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category identifiers
        /// </returns>
        Task<IList<int>> GetAppliedCategoryIdsAsync(Discount discount, User user);

        /// <summary>
        /// Gets child category identifiers
        /// </summary>
        /// <param name="parentCategoryId">Parent category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category identifiers
        /// </returns>
        Task<IList<int>> GetChildCategoryIdsAsync(int parentCategoryId, int storeId = 0, bool showHidden = false);

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category
        /// </returns>
        Task<Category> GetCategoryByIdAsync(int categoryId);

        /// <summary>
        /// Get categories for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted categories</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of categories
        /// </returns>
        Task<IPagedList<Category>> GetCategoriesByAppliedDiscountAsync(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Inserts category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertCategoryAsync(Category category);

        /// <summary>
        /// Updates the category
        /// </summary>
        /// <param name="category">Category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateCategoryAsync(Category category);

        /// <summary>
        /// Delete a list of categories
        /// </summary>
        /// <param name="categories">Categories</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteCategoriesAsync(IList<Category> categories);

        /// <summary>
        /// Deletes a tvchannel category mapping
        /// </summary>
        /// <param name="tvchannelCategory">TvChannel category</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTvChannelCategoryAsync(TvChannelCategory tvchannelCategory);

        /// <summary>
        /// Get a discount-category mapping record
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        Task<DiscountCategoryMapping> GetDiscountAppliedToCategoryAsync(int categoryId, int discountId);

        /// <summary>
        /// Inserts a discount-category mapping record
        /// </summary>
        /// <param name="discountCategoryMapping">Discount-category mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertDiscountCategoryMappingAsync(DiscountCategoryMapping discountCategoryMapping);

        /// <summary>
        /// Deletes a discount-category mapping record
        /// </summary>
        /// <param name="discountCategoryMapping">Discount-category mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteDiscountCategoryMappingAsync(DiscountCategoryMapping discountCategoryMapping);

        /// <summary>
        /// Gets tvchannel category mapping collection
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel a category mapping collection
        /// </returns>
        Task<IPagedList<TvChannelCategory>> GetTvChannelCategoriesByCategoryIdAsync(int categoryId,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Gets a tvchannel category mapping collection
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel category mapping collection
        /// </returns>
        Task<IList<TvChannelCategory>> GetTvChannelCategoriesByTvChannelIdAsync(int tvchannelId, bool showHidden = false);

        /// <summary>
        /// Gets a tvchannel category mapping 
        /// </summary>
        /// <param name="tvchannelCategoryId">TvChannel category mapping identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel category mapping
        /// </returns>
        Task<TvChannelCategory> GetTvChannelCategoryByIdAsync(int tvchannelCategoryId);

        /// <summary>
        /// Inserts a tvchannel category mapping
        /// </summary>
        /// <param name="tvchannelCategory">>TvChannel category mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTvChannelCategoryAsync(TvChannelCategory tvchannelCategory);

        /// <summary>
        /// Updates the tvchannel category mapping 
        /// </summary>
        /// <param name="tvchannelCategory">>TvChannel category mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTvChannelCategoryAsync(TvChannelCategory tvchannelCategory);

        /// <summary>
        /// Returns a list of names of not existing categories
        /// </summary>
        /// <param name="categoryIdsNames">The names and/or IDs of the categories to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of names and/or IDs not existing categories
        /// </returns>
        Task<string[]> GetNotExistingCategoriesAsync(string[] categoryIdsNames);

        /// <summary>
        /// Get category IDs for tvchannels
        /// </summary>
        /// <param name="tvchannelIds">TvChannels IDs</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category IDs for tvchannels
        /// </returns>
        Task<IDictionary<int, int[]>> GetTvChannelCategoryIdsAsync(int[] tvchannelIds);

        /// <summary>
        /// Gets categories by identifier
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the categories
        /// </returns>
        Task<IList<Category>> GetCategoriesByIdsAsync(int[] categoryIds);

        /// <summary>
        /// Returns a TvChannelCategory that has the specified values
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>A TvChannelCategory that has the specified values; otherwise null</returns>
        TvChannelCategory FindTvChannelCategory(IList<TvChannelCategory> source, int tvchannelId, int categoryId);

        /// <summary>
        /// Get formatted category breadcrumb 
        /// Note: ACL and store mapping is ignored
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="separator">Separator</param>
        /// <param name="languageId">Language identifier for localization</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted breadcrumb
        /// </returns>
        Task<string> GetFormattedBreadCrumbAsync(Category category, IList<Category> allCategories = null,
            string separator = ">>", int languageId = 0);

        /// <summary>
        /// Get category breadcrumb 
        /// </summary>
        /// <param name="category">Category</param>
        /// <param name="allCategories">All categories</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the category breadcrumb 
        /// </returns>
        Task<IList<Category>> GetCategoryBreadCrumbAsync(Category category, IList<Category> allCategories = null, bool showHidden = false);
    }
}