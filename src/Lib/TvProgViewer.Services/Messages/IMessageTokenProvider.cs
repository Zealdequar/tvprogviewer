using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Domain.Vendors;

namespace TvProgViewer.Services.Messages
{
    /// <summary>
    /// Message token provider
    /// </summary>
    public partial interface IMessageTokenProvider
    {
        /// <summary>
        /// Add store tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="store">Store</param>
        /// <param name="emailAccount">Email account</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddStoreTokensAsync(IList<Token> tokens, Store store, EmailAccount emailAccount);

        /// <summary>
        /// Add order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order"></param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddOrderTokensAsync(IList<Token> tokens, Order order, int languageId, int vendorId = 0);

        /// <summary>
        /// Add refunded order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order">Order</param>
        /// <param name="refundedAmount">Refunded amount of order</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddOrderRefundedTokensAsync(IList<Token> tokens, Order order, decimal refundedAmount);

        /// <summary>
        /// Add shipment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="shipment">Shipment item</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddShipmentTokensAsync(IList<Token> tokens, Shipment shipment, int languageId);

        /// <summary>
        /// Add order note tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="orderNote">Order note</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddOrderNoteTokensAsync(IList<Token> tokens, OrderNote orderNote);

        /// <summary>
        /// Add recurring payment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddRecurringPaymentTokensAsync(IList<Token> tokens, RecurringPayment recurringPayment);

        /// <summary>
        /// Add return request tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddReturnRequestTokensAsync(IList<Token> tokens, ReturnRequest returnRequest, OrderItem orderItem, int languageId);

        /// <summary>
        /// Add gift card tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddGiftCardTokensAsync(IList<Token> tokens, GiftCard giftCard, int languageId);

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="userId">User identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddUserTokensAsync(IList<Token> tokens, int userId);

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddUserTokensAsync(IList<Token> tokens, User user);

        /// <summary>
        /// Add vendor tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="vendor">Vendor</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddVendorTokensAsync(IList<Token> tokens, Vendor vendor);

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription);

        /// <summary>
        /// Add tvChannel review tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddTvChannelReviewTokensAsync(IList<Token> tokens, TvChannelReview tvChannelReview);

        /// <summary>
        /// Add blog comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="blogComment">Blog post comment</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment);

        /// <summary>
        /// Add news comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="newsComment">News comment</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment);
        
        /// <summary>
        /// Add tvChannel tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddTvChannelTokensAsync(IList<Token> tokens, TvChannel tvChannel, int languageId);

        /// <summary>
        /// Add tvChannel attribute combination tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="combination">TvChannel attribute combination</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddAttributeCombinationTokensAsync(IList<Token> tokens, TvChannelAttributeCombination combination, int languageId);

        /// <summary>
        /// Add forum tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forum">Forum</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddForumTokensAsync(IList<Token> tokens, Forum forum);

        /// <summary>
        /// Add forum topic tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddForumTopicTokensAsync(IList<Token> tokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null);

        /// <summary>
        /// Add forum post tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumPost">Forum post</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddForumPostTokensAsync(IList<Token> tokens, ForumPost forumPost);

        /// <summary>
        /// Add private message tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="privateMessage">Private message</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddPrivateMessageTokensAsync(IList<Token> tokens, PrivateMessage privateMessage);

        /// <summary>
        /// Add tokens of BackInStock subscription
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">BackInStock subscription</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task AddBackInStockTokensAsync(IList<Token> tokens, BackInStockSubscription subscription);

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the collection of allowed (supported) message tokens for campaigns
        /// </returns>
        Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync();

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the collection of allowed message tokens
        /// </returns>
        Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null);

        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate);
    }
}