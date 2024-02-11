using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tvchannel attribute model factory
    /// </summary>
    public partial interface ITvChannelAttributeModelFactory
    {
        /// <summary>
        /// Prepare tvchannel attribute search model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute search model
        /// </returns>
        Task<TvChannelAttributeSearchModel> PrepareTvChannelAttributeSearchModelAsync(TvChannelAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvchannel attribute list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute search model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute list model
        /// </returns>
        Task<TvChannelAttributeListModel> PrepareTvChannelAttributeListModelAsync(TvChannelAttributeSearchModel searchModel);

        /// <summary>
        /// Prepare tvchannel attribute model
        /// </summary>
        /// <param name="model">TvChannel attribute model</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel attribute model
        /// </returns>
        Task<TvChannelAttributeModel> PrepareTvChannelAttributeModelAsync(TvChannelAttributeModel model,
            TvChannelAttribute tvchannelAttribute, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged predefined tvchannel attribute value list model
        /// </summary>
        /// <param name="searchModel">Predefined tvchannel attribute value search model</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the predefined tvchannel attribute value list model
        /// </returns>
        Task<PredefinedTvChannelAttributeValueListModel> PreparePredefinedTvChannelAttributeValueListModelAsync(
            PredefinedTvChannelAttributeValueSearchModel searchModel, TvChannelAttribute tvchannelAttribute);

        /// <summary>
        /// Prepare predefined tvchannel attribute value model
        /// </summary>
        /// <param name="model">Predefined tvchannel attribute value model</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <param name="tvchannelAttributeValue">Predefined tvchannel attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the predefined tvchannel attribute value model
        /// </returns>
        Task<PredefinedTvChannelAttributeValueModel> PreparePredefinedTvChannelAttributeValueModelAsync(PredefinedTvChannelAttributeValueModel model,
            TvChannelAttribute tvchannelAttribute, PredefinedTvChannelAttributeValue tvchannelAttributeValue, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged list model of tvchannels that use the tvchannel attribute
        /// </summary>
        /// <param name="searchModel">Search model of tvchannels that use the tvchannel attribute</param>
        /// <param name="tvchannelAttribute">TvChannel attribute</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list model of tvchannels that use the tvchannel attribute
        /// </returns>
        Task<TvChannelAttributeTvChannelListModel> PrepareTvChannelAttributeTvChannelListModelAsync(TvChannelAttributeTvChannelSearchModel searchModel,
            TvChannelAttribute tvchannelAttribute);
    }
}