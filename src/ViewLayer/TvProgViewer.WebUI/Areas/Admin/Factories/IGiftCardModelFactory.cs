﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.WebUI.Areas.Admin.Models.Orders;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the gift card model factory
    /// </summary>
    public partial interface IGiftCardModelFactory
    {
        /// <summary>
        /// Prepare gift card search model
        /// </summary>
        /// <param name="searchModel">Gift card search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gift card search model
        /// </returns>
        Task<GiftCardSearchModel> PrepareGiftCardSearchModelAsync(GiftCardSearchModel searchModel);

        /// <summary>
        /// Prepare paged gift card list model
        /// </summary>
        /// <param name="searchModel">Gift card search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gift card list model
        /// </returns>
        Task<GiftCardListModel> PrepareGiftCardListModelAsync(GiftCardSearchModel searchModel);

        /// <summary>
        /// Prepare gift card model
        /// </summary>
        /// <param name="model">Gift card model</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gift card model
        /// </returns>
        Task<GiftCardModel> PrepareGiftCardModelAsync(GiftCardModel model, GiftCard giftCard, bool excludeProperties = false);

        /// <summary>
        /// Prepare paged gift usage history card list model
        /// </summary>
        /// <param name="searchModel">Gift card usage history search model</param>
        /// <param name="giftCard">Gift card</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gift card usage history list model
        /// </returns>
        Task<GiftCardUsageHistoryListModel> PrepareGiftCardUsageHistoryListModelAsync(GiftCardUsageHistorySearchModel searchModel,
            GiftCard giftCard);
    }
}