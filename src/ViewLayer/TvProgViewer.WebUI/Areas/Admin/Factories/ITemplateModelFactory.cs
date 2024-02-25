using System.Threading.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.Templates;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the template model factory
    /// </summary>
    public partial interface ITemplateModelFactory
    {
        /// <summary>
        /// Prepare templates model
        /// </summary>
        /// <param name="model">Templates model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the mplates model
        /// </returns>
        Task<TemplatesModel> PrepareTemplatesModelAsync(TemplatesModel model);

        /// <summary>
        /// Prepare paged category template list model
        /// </summary>
        /// <param name="searchModel">Category template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category template list model
        /// </returns>
        Task<CategoryTemplateListModel> PrepareCategoryTemplateListModelAsync(CategoryTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare paged manufacturer template list model
        /// </summary>
        /// <param name="searchModel">Manufacturer template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer template list model
        /// </returns>
        Task<ManufacturerTemplateListModel> PrepareManufacturerTemplateListModelAsync(ManufacturerTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvchannel template list model
        /// </summary>
        /// <param name="searchModel">TvChannel template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel template list model
        /// </returns>
        Task<TvChannelTemplateListModel> PrepareTvChannelTemplateListModelAsync(TvChannelTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare paged topic template list model
        /// </summary>
        /// <param name="searchModel">Topic template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic template list model
        /// </returns>
        Task<TopicTemplateListModel> PrepareTopicTemplateListModelAsync(TopicTemplateSearchModel searchModel);
        
        /// <summary>
        /// Prepare category template search model
        /// </summary>
        /// <param name="searchModel">Category template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the category template search model
        /// </returns>
        Task<CategoryTemplateSearchModel> PrepareCategoryTemplateSearchModelAsync(CategoryTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare manufacturer template search model
        /// </summary>
        /// <param name="searchModel">Manufacturer template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer template search model
        /// </returns>
        Task<ManufacturerTemplateSearchModel> PrepareManufacturerTemplateSearchModelAsync(ManufacturerTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare tvchannel template search model
        /// </summary>
        /// <param name="searchModel">TvChannel template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel template search model
        /// </returns>
        Task<TvChannelTemplateSearchModel> PrepareTvChannelTemplateSearchModelAsync(TvChannelTemplateSearchModel searchModel);

        /// <summary>
        /// Prepare topic template search model
        /// </summary>
        /// <param name="searchModel">Topic template search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the opic template search model
        /// </returns>
        Task<TopicTemplateSearchModel> PrepareTopicTemplateSearchModelAsync(TopicTemplateSearchModel searchModel);
    }
}