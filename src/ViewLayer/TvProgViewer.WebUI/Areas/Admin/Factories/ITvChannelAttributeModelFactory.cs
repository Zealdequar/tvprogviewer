using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tvChannel attribute model factory
    /// </summary>
    public partial interface ITvChannelAttributeModelFactory
    {
        /// <summary>
        /// Prepare tvChannel attribute search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute search model
        /// </returns>
        Task<TvChannelAttributeSearchModel> PrepareTvChannelAttributeSearchModelAsync(TvChannelAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvChannel attribute list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute list model
        /// </returns>
        Task<TvChannelAttributeListModel> PrepareTvChannelAttributeListModelAsync(TvChannelAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare tvChannel attribute model
        /// </summary>
        /// <param name="model">TvChannel attribute model</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute model
        /// </returns>
        Task<TvChannelAttributeModel> PrepareTvChannelAttributeModelAsync(TvChannelAttributeModel model,
            TvChannelAttribute tvChannelAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged predefined tvChannel attribute value list model
        /// </summary>
        /// <param name="searchModel">Predefined tvChannel attribute value search model</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the predefined tvChannel attribute value list model
        /// </returns>
        Task<PredefinedTvChannelAttributeValueListModel> PreparePredefinedTvChannelAttributeValueListModelAsync(
            PredefinedTvChannelAttributeValueSearchModel searchModel, TvChannelAttribute tvChannelAttribute);

        /// <summary>
        /// Prepare predefined tvChannel attribute value model
        /// </summary>
        /// <param name="model">Predefined tvChannel attribute value model</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <param name="tvChannelAttributeValue">Predefined tvChannel attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the predefined tvChannel attribute value model
        /// </returns>
        Task<PredefinedTvChannelAttributeValueModel> PreparePredefinedTvChannelAttributeValueModelAsync(PredefinedTvChannelAttributeValueModel model,
            TvChannelAttribute tvChannelAttribute, PredefinedTvChannelAttributeValue tvChannelAttributeValue, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged list model of tvChannels that use the tvChannel attribute
        /// </summary>
        /// <param name="searchModel">Search model of tvChannels that use the tvChannel attribute</param>
        /// <param name="tvChannelAttribute">TvChannel attribute</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list model of tvChannels that use the tvChannel attribute
        /// </returns>
        Task<TvChannelAttributeTvChannelListModel> PrepareTvChannelAttributeTvChannelListModelAsync(TvChannelAttributeTvChannelSearchModel searchModel,
            TvChannelAttribute tvChannelAttribute);
    }
}