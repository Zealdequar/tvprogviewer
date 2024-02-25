using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the tvchannel model factory
    /// </summary>
    public partial interface ITvChannelModelFactory
    {
        /// <summary>
        /// Prepare tvchannel search model
        /// </summary>
        /// <param name="searchModel">TvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel search model
        /// </returns>
        Task<TvChannelSearchModel> PrepareTvChannelSearchModelAsync(TvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvchannel list model
        /// </summary>
        /// <param name="searchModel">TvChannel search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel list model
        /// </returns>
        Task<TvChannelListModel> PrepareTvChannelListModelAsync(TvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare tvchannel model
        /// </summary>
        /// <param name="model">TvChannel model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel model
        /// </returns>
        Task<TvChannelModel> PrepareTvChannelModelAsync(TvChannelModel model, TvChannel tvchannel, bool excludeProperties = false);

        /// <summary>
        /// Prepare required tvchannel search model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Required tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the required tvchannel search model to add to the tvchannel
        /// </returns>
        Task<AddRequiredTvChannelSearchModel> PrepareAddRequiredTvChannelSearchModelAsync(AddRequiredTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare required tvchannel list model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Required tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the required tvchannel list model to add to the tvchannel
        /// </returns>
        Task<AddRequiredTvChannelListModel> PrepareAddRequiredTvChannelListModelAsync(AddRequiredTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged related tvchannel list model
        /// </summary>
        /// <param name="searchModel">Related tvchannel search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvchannel list model
        /// </returns>
        Task<RelatedTvChannelListModel> PrepareRelatedTvChannelListModelAsync(RelatedTvChannelSearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare related tvchannel search model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Related tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvchannel search model to add to the tvchannel
        /// </returns>
        Task<AddRelatedTvChannelSearchModel> PrepareAddRelatedTvChannelSearchModelAsync(AddRelatedTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged related tvchannel list model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Related tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvchannel list model to add to the tvchannel
        /// </returns>
        Task<AddRelatedTvChannelListModel> PrepareAddRelatedTvChannelListModelAsync(AddRelatedTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged cross-sell tvchannel list model
        /// </summary>
        /// <param name="searchModel">Cross-sell tvchannel search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvchannel list model
        /// </returns>
        Task<CrossSellTvChannelListModel> PrepareCrossSellTvChannelListModelAsync(CrossSellTvChannelSearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare cross-sell tvchannel search model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Cross-sell tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvchannel search model to add to the tvchannel
        /// </returns>
        Task<AddCrossSellTvChannelSearchModel> PrepareAddCrossSellTvChannelSearchModelAsync(AddCrossSellTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged cross-sell tvchannel list model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Cross-sell tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvchannel list model to add to the tvchannel
        /// </returns>
        Task<AddCrossSellTvChannelListModel> PrepareAddCrossSellTvChannelListModelAsync(AddCrossSellTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged associated tvchannel list model
        /// </summary>
        /// <param name="searchModel">Associated tvchannel search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the associated tvchannel list model
        /// </returns>
        Task<AssociatedTvChannelListModel> PrepareAssociatedTvChannelListModelAsync(AssociatedTvChannelSearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare associated tvchannel search model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Associated tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the associated tvchannel search model to add to the tvchannel
        /// </returns>
        Task<AddAssociatedTvChannelSearchModel> PrepareAddAssociatedTvChannelSearchModelAsync(AddAssociatedTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged associated tvchannel list model to add to the tvchannel
        /// </summary>
        /// <param name="searchModel">Associated tvchannel search model to add to the tvchannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the associated tvchannel list model to add to the tvchannel
        /// </returns>
        Task<AddAssociatedTvChannelListModel> PrepareAddAssociatedTvChannelListModelAsync(AddAssociatedTvChannelSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvchannel picture list model
        /// </summary>
        /// <param name="searchModel">TvChannel picture search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel picture list model
        /// </returns>
        Task<TvChannelPictureListModel> PrepareTvChannelPictureListModelAsync(TvChannelPictureSearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare paged tvchannel video list model
        /// </summary>
        /// <param name="searchModel">TvChannel video search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel video list model
        /// </returns>
        Task<TvChannelVideoListModel> PrepareTvChannelVideoListModelAsync(TvChannelVideoSearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare paged tvchannel specification attribute list model
        /// </summary>
        /// <param name="searchModel">TvChannel specification attribute search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel specification attribute list model
        /// </returns>
        Task<TvChannelSpecificationAttributeListModel> PrepareTvChannelSpecificationAttributeListModelAsync(
            TvChannelSpecificationAttributeSearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare paged tvchannel specification attribute model
        /// </summary>
        /// <param name="tvchannelId">TvChannel id</param>
        /// <param name="specificationId">Specification attribute id</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel specification attribute model
        /// </returns>
        Task<AddSpecificationAttributeModel> PrepareAddSpecificationAttributeModelAsync(int tvchannelId, int? specificationId);

        /// <summary>
        /// Prepare tvchannel tag search model
        /// </summary>
        /// <param name="searchModel">TvChannel tag search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel tag search model
        /// </returns>
        Task<TvChannelTagSearchModel> PrepareTvChannelTagSearchModelAsync(TvChannelTagSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvchannel tag list model
        /// </summary>
        /// <param name="searchModel">TvChannel tag search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel tag list model
        /// </returns>
        Task<TvChannelTagListModel> PrepareTvChannelTagListModelAsync(TvChannelTagSearchModel searchModel);

        /// <summary>
        /// Prepare tvchannel tag model
        /// </summary>
        /// <param name="model">TvChannel tag model</param>
        /// <param name="tvchannelTag">TvChannel tag</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel tag model
        /// </returns>
        Task<TvChannelTagModel> PrepareTvChannelTagModelAsync(TvChannelTagModel model, TvChannelTag tvchannelTag, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged tvchannel order list model
        /// </summary>
        /// <param name="searchModel">TvChannel order search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel order list model
        /// </returns>
        Task<TvChannelOrderListModel> PrepareTvChannelOrderListModelAsync(TvChannelOrderSearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare paged tier price list model
        /// </summary>
        /// <param name="searchModel">Tier price search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price list model
        /// </returns>
        Task<TierPriceListModel> PrepareTierPriceListModelAsync(TierPriceSearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare tier price model
        /// </summary>
        /// <param name="model">Tier price model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tierPrice">Tier price</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price model
        /// </returns>
        Task<TierPriceModel> PrepareTierPriceModelAsync(TierPriceModel model,
            TvChannel tvchannel, TierPrice tierPrice, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged stock quantity history list model
        /// </summary>
        /// <param name="searchModel">Stock quantity history search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the stock quantity history list model
        /// </returns>
        Task<StockQuantityHistoryListModel> PrepareStockQuantityHistoryListModelAsync(StockQuantityHistorySearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare paged tvchannel attribute mapping list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute mapping search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute mapping list model
        /// </returns>
        Task<TvChannelAttributeMappingListModel> PrepareTvChannelAttributeMappingListModelAsync(TvChannelAttributeMappingSearchModel searchModel,
            TvChannel tvchannel);

        /// <summary>
        /// Prepare tvchannel attribute mapping model
        /// </summary>
        /// <param name="model">TvChannel attribute mapping model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute mapping model
        /// </returns>
        Task<TvChannelAttributeMappingModel> PrepareTvChannelAttributeMappingModelAsync(TvChannelAttributeMappingModel model,
            TvChannel tvchannel, TvChannelAttributeMapping tvchannelAttributeMapping, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged tvchannel attribute value list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute value search model</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute value list model
        /// </returns>
        Task<TvChannelAttributeValueListModel> PrepareTvChannelAttributeValueListModelAsync(TvChannelAttributeValueSearchModel searchModel,
            TvChannelAttributeMapping tvchannelAttributeMapping);

        /// <summary>
        /// Prepare tvchannel attribute value model
        /// </summary>
        /// <param name="model">TvChannel attribute value model</param>
        /// <param name="tvchannelAttributeMapping">TvChannel attribute mapping</param>
        /// <param name="tvchannelAttributeValue">TvChannel attribute value</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute value model
        /// </returns>
        Task<TvChannelAttributeValueModel> PrepareTvChannelAttributeValueModelAsync(TvChannelAttributeValueModel model,
            TvChannelAttributeMapping tvchannelAttributeMapping, TvChannelAttributeValue tvchannelAttributeValue, bool excludeProperties = false);

        /// <summary>
        /// Prepare tvchannel model to associate to the tvchannel attribute value
        /// </summary>
        /// <param name="searchModel">TvChannel model to associate to the tvchannel attribute value</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel model to associate to the tvchannel attribute value
        /// </returns>
        Task<AssociateTvChannelToAttributeValueSearchModel> PrepareAssociateTvChannelToAttributeValueSearchModelAsync(
            AssociateTvChannelToAttributeValueSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvchannel model to associate to the tvchannel attribute value
        /// </summary>
        /// <param name="searchModel">TvChannel model to associate to the tvchannel attribute value</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel model to associate to the tvchannel attribute value
        /// </returns>
        Task<AssociateTvChannelToAttributeValueListModel> PrepareAssociateTvChannelToAttributeValueListModelAsync(
            AssociateTvChannelToAttributeValueSearchModel searchModel);

        /// <summary>
        /// Prepare paged tvchannel attribute combination list model
        /// </summary>
        /// <param name="searchModel">TvChannel attribute combination search model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute combination list model
        /// </returns>
        Task<TvChannelAttributeCombinationListModel> PrepareTvChannelAttributeCombinationListModelAsync(
            TvChannelAttributeCombinationSearchModel searchModel, TvChannel tvchannel);

        /// <summary>
        /// Prepare tvchannel attribute combination model
        /// </summary>
        /// <param name="model">TvChannel attribute combination model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelAttributeCombination">TvChannel attribute combination</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel attribute combination model
        /// </returns>
        Task<TvChannelAttributeCombinationModel> PrepareTvChannelAttributeCombinationModelAsync(TvChannelAttributeCombinationModel model,
            TvChannel tvchannel, TvChannelAttributeCombination tvchannelAttributeCombination, bool excludeProperties = false);
    }
}