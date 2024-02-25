using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.WebUI.Models.Catalog;

namespace TvProgViewer.WebUI.Factories
{

    /// <summary>
    /// Represents the interface of the tvchannel model factory
    /// </summary>
    public partial interface ITvChannelModelFactory
    {
        /// <summary>
        /// Get the tvchannel template view path
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the view path
        /// </returns>
        Task<string> PrepareTvChannelTemplateViewPathAsync(TvChannel tvchannel);

        /// <summary>
        /// Prepare the tvchannel overview models
        /// </summary>
        /// <param name="tvchannels">Collection of tvchannels</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="tvchannelThumbPictureSize">TvChannel thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the collection of tvchannel overview model
        /// </returns>
        Task<IEnumerable<TvChannelOverviewModel>> PrepareTvChannelOverviewModelsAsync(IEnumerable<TvChannel> tvchannels,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? tvchannelThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false);

        /// <summary>
        /// Prepare the tvchannel combination models
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel combination models
        /// </returns>
        Task<IList<TvChannelCombinationModel>> PrepareTvChannelCombinationModelsAsync(TvChannel tvchannel);

        /// <summary>
        /// Prepare the tvchannel details model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedTvChannel">Whether the tvchannel is associated</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel details model
        /// </returns>
        Task<TvChannelDetailsModel> PrepareTvChannelDetailsModelAsync(TvChannel tvchannel, ShoppingCartItem updatecartitem = null, bool isAssociatedTvChannel = false);

        /// <summary>
        /// Prepare the tvchannel reviews model
        /// </summary>
        /// <param name="model">TvChannel reviews model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel reviews model
        /// </returns>
        Task<TvChannelReviewsModel> PrepareTvChannelReviewsModelAsync(TvChannelReviewsModel model, TvChannel tvchannel);

        /// <summary>
        /// Prepare the user tvchannel reviews model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user tvchannel reviews model
        /// </returns>
        Task<UserTvChannelReviewsModel> PrepareUserTvChannelReviewsModelAsync(int? page);

        /// <summary>
        /// Prepare the tvchannel email a friend model
        /// </summary>
        /// <param name="model">TvChannel email a friend model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel email a friend model
        /// </returns>
        Task<TvChannelEmailAFriendModel> PrepareTvChannelEmailAFriendModelAsync(TvChannelEmailAFriendModel model, TvChannel tvchannel, bool excludeProperties);

        /// <summary>
        /// Prepare the tvchannel specification model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel specification model
        /// </returns>
        Task<TvChannelSpecificationModel> PrepareTvChannelSpecificationModelAsync(TvChannel tvchannel);
    }
}