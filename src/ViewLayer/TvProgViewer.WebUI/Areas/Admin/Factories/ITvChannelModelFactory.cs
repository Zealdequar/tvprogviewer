using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tvChannel model factory
    /// </summary>
    public partial interface ITvChannelModelFactory
    {
        /// <summary>
        /// Prepare tvChannel search model
        /// </summary>
        /// <param name="searchModel">TvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel search model
        /// </returns>
        Task<TvChannelSearchModel> PrepareTvChannelSearchModelAsync(TvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvChannel list model
        /// </summary>
        /// <param name="searchModel">TvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel list model
        /// </returns>
        Task<TvChannelListModel> PrepareTvChannelListModelAsync(TvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare tvChannel model
        /// </summary>
        /// <param name="model">TvChannel model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel model
        /// </returns>
        Task<TvChannelModel> PrepareTvChannelModelAsync(TvChannelModel model, TvChannel tvChannel, bool excludeProperties = false);

        /// <summary>
        /// Prepare required tvChannel search model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Required tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the required tvChannel search model to add to the tvChannel
        /// </returns>
        Task<AddRequiredTvChannelSearchModel> PrepareAddRequiredTvChannelSearchModelAsync(AddRequiredTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare required tvChannel list model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Required tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the required tvChannel list model to add to the tvChannel
        /// </returns>
        Task<AddRequiredTvChannelListModel> PrepareAddRequiredTvChannelListModelAsync(AddRequiredTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged related tvChannel list model
        /// </summary>
        /// <param name="searchModel">Related tvChannel search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannel list model
        /// </returns>
        Task<RelatedTvChannelListModel> PrepareRelatedTvChannelListModelAsync(RelatedTvChannelSearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare related tvChannel search model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Related tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannel search model to add to the tvChannel
        /// </returns>
        Task<AddRelatedTvChannelSearchModel> PrepareAddRelatedTvChannelSearchModelAsync(AddRelatedTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged related tvChannel list model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Related tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannel list model to add to the tvChannel
        /// </returns>
        Task<AddRelatedTvChannelListModel> PrepareAddRelatedTvChannelListModelAsync(AddRelatedTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged cross-sell tvChannel list model
        /// </summary>
        /// <param name="searchModel">Cross-sell tvChannel search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannel list model
        /// </returns>
        Task<CrossSellTvChannelListModel> PrepareCrossSellTvChannelListModelAsync(CrossSellTvChannelSearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare cross-sell tvChannel search model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Cross-sell tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannel search model to add to the tvChannel
        /// </returns>
        Task<AddCrossSellTvChannelSearchModel> PrepareAddCrossSellTvChannelSearchModelAsync(AddCrossSellTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged cross-sell tvChannel list model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Cross-sell tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannel list model to add to the tvChannel
        /// </returns>
        Task<AddCrossSellTvChannelListModel> PrepareAddCrossSellTvChannelListModelAsync(AddCrossSellTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged associated tvChannel list model
        /// </summary>
        /// <param name="searchModel">Associated tvChannel search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the associated tvChannel list model
        /// </returns>
        Task<AssociatedTvChannelListModel> PrepareAssociatedTvChannelListModelAsync(AssociatedTvChannelSearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare associated tvChannel search model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Associated tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the associated tvChannel search model to add to the tvChannel
        /// </returns>
        Task<AddAssociatedTvChannelSearchModel> PrepareAddAssociatedTvChannelSearchModelAsync(AddAssociatedTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged associated tvChannel list model to add to the tvChannel
        /// </summary>
        /// <param name="searchModel">Associated tvChannel search model to add to the tvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the associated tvChannel list model to add to the tvChannel
        /// </returns>
        Task<AddAssociatedTvChannelListModel> PrepareAddAssociatedTvChannelListModelAsync(AddAssociatedTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvChannel picture list model
        /// </summary>
        /// <param name="searchModel">TvChannel picture search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel picture list model
        /// </returns>
        Task<TvChannelPictureListModel> PrepareTvChannelPictureListModelAsync(TvChannelPictureSearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare paged tvChannel video list model
        /// </summary>
        /// <param name="searchModel">TvChannel video search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel video list model
        /// </returns>
        Task<TvChannelVideoListModel> PrepareTvChannelVideoListModelAsync(TvChannelVideoSearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare paged tvChannel specification attribute list model
        /// </summary>
        /// <param name="searchModel">TvChannel specification attribute search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel specification attribute list model
        /// </returns>
        Task<TvChannelSpecificationAttributeListModel> PrepareTvChannelSpecificationAttributeListModelAsync(
            TvChannelSpecificationAttributeSearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare paged tvChannel specification attribute model
        /// </summary>
        /// <param name="tvChannelId">TvChannel id</param>
        /// <param name="specificationId">Specification attribute id</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel specification attribute model
        /// </returns>
        Task<AddSpecificationAttributeModel> PrepareAddSpecificationAttributeModelAsync(int tvChannelId, int? specificationId);

        /// <summary>
        /// Prepare tvChannel tag search model
        /// </summary>
        /// <param name="searchModel">TvChannel tag search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tag search model
        /// </returns>
        Task<TvChannelTagSearchModel> PrepareTvChannelTagSearchModelAsync(TvChannelTagSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvChannel tag list model
        /// </summary>
        /// <param name="searchModel">TvChannel tag search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tag list model
        /// </returns>
        Task<TvChannelTagListModel> PrepareTvChannelTagListModelAsync(TvChannelTagSearchModel searchModel);

        /// <summary>
        /// Prepare tvChannel tag model
        /// </summary>
        /// <param name="model">TvChannel tag model</param>
        /// <param name="tvChannelTag">TvChannel tag</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel tag model
        /// </returns>
        Task<TvChannelTagModel> PrepareTvChannelTagModelAsync(TvChannelTagModel model, TvChannelTag tvChannelTag, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged tvChannel order list model
        /// </summary>
        /// <param name="searchModel">TvChannel order search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel order list model
        /// </returns>
        Task<TvChannelOrderListModel> PrepareTvChannelOrderListModelAsync(TvChannelOrderSearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare paged tier price list model
        /// </summary>
        /// <param name="searchModel">Tier price search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price list model
        /// </returns>
        Task<TierPriceListModel> PrepareTierPriceListModelAsync(TierPriceSearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare tier price model
        /// </summary>
        /// <param name="model">Tier price model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tierPrice">Tier price</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price model
        /// </returns>
        Task<TierPriceModel> PrepareTierPriceModelAsync(TierPriceModel model,
            TvChannel tvChannel, TierPrice tierPrice, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged stock quantity history list model
        /// </summary>
        /// <param name="searchModel">Stock quantity history search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the stock quantity history list model
        /// </returns>
        Task<StockQuantityHistoryListModel> PrepareStockQuantityHistoryListModelAsync(StockQuantityHistorySearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare paged tvChannel attribute mapping list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute mapping search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping list model
        /// </returns>
        Task<TvChannelAttributeMappingListModel> PrepareTvChannelAttributeMappingListModelAsync(TvChannelAttributeMappingSearchModel searchModel,
            TvChannel tvChannel);

        /// <summary>
        /// Prepare tvChannel attribute mapping model
        /// </summary>
        /// <param name="model">TvChannel attribute mapping model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute mapping model
        /// </returns>
        Task<TvChannelAttributeMappingModel> PrepareTvChannelAttributeMappingModelAsync(TvChannelAttributeMappingModel model,
            TvChannel tvChannel, TvChannelAttributeMapping tvChannelAttributeMapping, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged tvChannel attribute value list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute value search model</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute value list model
        /// </returns>
        Task<TvChannelAttributeValueListModel> PrepareTvChannelAttributeValueListModelAsync(TvChannelAttributeValueSearchModel searchModel,
            TvChannelAttributeMapping tvChannelAttributeMapping);

        /// <summary>
        /// Prepare tvChannel attribute value model
        /// </summary>
        /// <param name="model">TvChannel attribute value model</param>
        /// <param name="tvChannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="tvChannelAttributeValue">TvChannel attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute value model
        /// </returns>
        Task<TvChannelAttributeValueModel> PrepareTvChannelAttributeValueModelAsync(TvChannelAttributeValueModel model,
            TvChannelAttributeMapping tvChannelAttributeMapping, TvChannelAttributeValue tvChannelAttributeValue, bool excludeProperties = false);

        /// <summary>
        /// Prepare tvChannel model to associate to the tvChannel attribute value
        /// </summary>
        /// <param name="searchModel">TvChannel model to associate to the tvChannel attribute value</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel model to associate to the tvChannel attribute value
        /// </returns>
        Task<AssociateTvChannelToAttributeValueSearchModel> PrepareAssociateTvChannelToAttributeValueSearchModelAsync(
            AssociateTvChannelToAttributeValueSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvChannel model to associate to the tvChannel attribute value
        /// </summary>
        /// <param name="searchModel">TvChannel model to associate to the tvChannel attribute value</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel model to associate to the tvChannel attribute value
        /// </returns>
        Task<AssociateTvChannelToAttributeValueListModel> PrepareAssociateTvChannelToAttributeValueListModelAsync(
            AssociateTvChannelToAttributeValueSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvChannel attribute combination list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute combination search model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combination list model
        /// </returns>
        Task<TvChannelAttributeCombinationListModel> PrepareTvChannelAttributeCombinationListModelAsync(
            TvChannelAttributeCombinationSearchModel searchModel, TvChannel tvChannel);

        /// <summary>
        /// Prepare tvChannel attribute combination model
        /// </summary>
        /// <param name="model">TvChannel attribute combination model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelAttributeCombination">TvChannel attribute combination</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel attribute combination model
        /// </returns>
        Task<TvChannelAttributeCombinationModel> PrepareTvChannelAttributeCombinationModelAsync(TvChannelAttributeCombinationModel model,
            TvChannel tvChannel, TvChannelAttributeCombination tvChannelAttributeCombination, bool excludeProperties = false);
    }
}