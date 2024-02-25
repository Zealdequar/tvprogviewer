using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the category model factory
    /// </summary>
    public partial interface ICategoryModelFactory
    {
        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category search model
        /// </returns>
        Task<CategorySearchModel> PrepareCategorySearchModelAsync(CategorySearchModel searchModel);

        /// <summary>
        /// Prepare paged category list model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category list model
        /// </returns>
        Task<CategoryListModel> PrepareCategoryListModelAsync(CategorySearchModel searchModel);

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category model
        /// </returns>
        Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged category tvchannel list model
        /// </summary>
        /// <param name="searchModel">Category tvchannel search model</param>
        /// <param name="category">Category</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category tvchannel list model
        /// </returns>
        Task<CategoryTvChannelListModel> PrepareCategoryTvChannelListModelAsync(CategoryTvChannelSearchModel searchModel, Category category);

        /// <summary>
        /// Prepare tvchannel search model to add to the category
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the category</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel search model to add to the category
        /// </returns>
        Task<AddTvChannelToCategorySearchModel> PrepareAddTvChannelToCategorySearchModelAsync(AddTvChannelToCategorySearchModel searchModel);

        /// <summary>
        /// Prepare paged tvchannel list model to add to the category
        /// </summary>
        /// <param name="searchModel">TvChannel search model to add to the category</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel list model to add to the category
        /// </returns>
        Task<AddTvChannelToCategoryListModel> PrepareAddTvChannelToCategoryListModelAsync(AddTvChannelToCategorySearchModel searchModel);
    }
}