using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.WebUI.Models.Catalog;

namespace TvProgViewer.WebUI.Factories
{

    /// <summary>
    /// Represents the interface of the tvChannel model factory
    /// </summary>
    public partial interface ITvChannelModelFactory
    {
        /// <summary>
        /// Get the tvChannel template view path
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the view path
        /// </returns>
        Task<string> PrepareTvChannelTemplateViewPathAsync(TvChannel tvChannel);

        /// <summary>
        /// Prepare the tvChannel overview models
        /// </summary>
        /// <param name="tvChannels">Collection of tvChannels</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="tvChannelThumbPictureSize">TvChannel thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the collection of tvChannel overview model
        /// </returns>
        Task<IEnumerable<TvChannelOverviewModel>> PrepareTvChannelOverviewModelsAsync(IEnumerable<TvChannel> tvChannels,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? tvChannelThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false);

        /// <summary>
        /// Prepare the tvChannel combination models
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel combination models
        /// </returns>
        Task<IList<TvChannelCombinationModel>> PrepareTvChannelCombinationModelsAsync(TvChannel tvChannel);

        /// <summary>
        /// Prepare the tvChannel details model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedTvChannel">Whether the tvChannel is associated</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel details model
        /// </returns>
        Task<TvChannelDetailsModel> PrepareTvChannelDetailsModelAsync(TvChannel tvChannel, ShoppingCartItem updatecartitem = null, bool isAssociatedTvChannel = false);

        /// <summary>
        /// Prepare the tvChannel reviews model
        /// </summary>
        /// <param name="model">TvChannel reviews model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel reviews model
        /// </returns>
        Task<TvChannelReviewsModel> PrepareTvChannelReviewsModelAsync(TvChannelReviewsModel model, TvChannel tvChannel);

        /// <summary>
        /// Prepare the user tvChannel reviews model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user tvChannel reviews model
        /// </returns>
        Task<UserTvChannelReviewsModel> PrepareUserTvChannelReviewsModelAsync(int? page);

        /// <summary>
        /// Prepare the tvChannel email a friend model
        /// </summary>
        /// <param name="model">TvChannel email a friend model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel email a friend model
        /// </returns>
        Task<TvChannelEmailAFriendModel> PrepareTvChannelEmailAFriendModelAsync(TvChannelEmailAFriendModel model, TvChannel tvChannel, bool excludeProperties);

        /// <summary>
        /// Prepare the tvChannel specification model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel specification model
        /// </returns>
        Task<TvChannelSpecificationModel> PrepareTvChannelSpecificationModelAsync(TvChannel tvChannel);
    }
}