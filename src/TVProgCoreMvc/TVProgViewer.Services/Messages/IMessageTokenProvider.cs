using System.Collections.Generic;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Core.Domain.Vendors;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Messages
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
        Task AddStoreTokensAsync(IList<Token> tokens, Store store, EmailAccount emailAccount);

        /// <summary>
        /// Add order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order"></param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="vendorId">Vendor identifier</param>
        Task AddOrderTokensAsync(IList<Token> tokens, Order order, int languageId, int vendorId = 0);

        /// <summary>
        /// Add refunded order tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="order">Order</param>
        /// <param name="refundedAmount">Refunded amount of order</param>
        Task AddOrderRefundedTokensAsync(IList<Token> tokens, Order order, decimal refundedAmount);

        /// <summary>
        /// Add shipment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="shipment">Shipment item</param>
        /// <param name="languageId">Language identifier</param>
        Task AddShipmentTokensAsync(IList<Token> tokens, Shipment shipment, int languageId);

        /// <summary>
        /// Add order note tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="orderNote">Order note</param>
        Task AddOrderNoteTokensAsync(IList<Token> tokens, OrderNote orderNote);

        /// <summary>
        /// Add recurring payment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="recurringPayment">Recurring payment</param>
        Task AddRecurringPaymentTokensAsync(IList<Token> tokens, RecurringPayment recurringPayment);

        /// <summary>
        /// Add return request tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        Task AddReturnRequestTokensAsync(IList<Token> tokens, ReturnRequest returnRequest, OrderItem orderItem);

        /// <summary>
        /// Add gift card tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="giftCard">Gift card</param>
        Task AddGiftCardTokensAsync(IList<Token> tokens, GiftCard giftCard);

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="userId">User identifier</param>
        Task AddUserTokensAsync(IList<Token> tokens, int userId);

        /// <summary>
        /// Add user tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="user">User</param>
        Task AddUserTokensAsync(IList<Token> tokens, User user);

        /// <summary>
        /// Add vendor tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="vendor">Vendor</param>
        Task AddVendorTokensAsync(IList<Token> tokens, Vendor vendor);

        /// <summary>
        /// Add newsletter subscription tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">Newsletter subscription</param>
        Task AddNewsLetterSubscriptionTokensAsync(IList<Token> tokens, NewsLetterSubscription subscription);

        /// <summary>
        /// Add product review tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="productReview">Product review</param>
        Task AddProductReviewTokensAsync(IList<Token> tokens, ProductReview productReview);

        /// <summary>
        /// Add blog comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="blogComment">Blog post comment</param>
        Task AddBlogCommentTokensAsync(IList<Token> tokens, BlogComment blogComment);

        /// <summary>
        /// Add news comment tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="newsComment">News comment</param>
        Task AddNewsCommentTokensAsync(IList<Token> tokens, NewsComment newsComment);

        /// <summary>
        /// Add product tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="product">Product</param>
        /// <param name="languageId">Language identifier</param>
        Task AddProductTokensAsync(IList<Token> tokens, Product product, int languageId);

        /// <summary>
        /// Add product attribute combination tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="combination">Product attribute combination</param>
        /// <param name="languageId">Language identifier</param>
        Task AddAttributeCombinationTokensAsync(IList<Token> tokens, ProductAttributeCombination combination, int languageId);

        /// <summary>
        /// Add forum tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forum">Forum</param>
        Task AddForumTokensAsync(IList<Token> tokens, Forum forum);

        /// <summary>
        /// Add forum topic tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumTopic">Forum topic</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="appendedPostIdentifierAnchor">Forum post identifier</param>
        Task AddForumTopicTokensAsync(IList<Token> tokens, ForumTopic forumTopic,
            int? friendlyForumTopicPageIndex = null, int? appendedPostIdentifierAnchor = null);

        /// <summary>
        /// Add forum post tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="forumPost">Forum post</param>
        Task AddForumPostTokensAsync(IList<Token> tokens, ForumPost forumPost);

        /// <summary>
        /// Add private message tokens
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="privateMessage">Private message</param>
        Task AddPrivateMessageTokensAsync(IList<Token> tokens, PrivateMessage privateMessage);

        /// <summary>
        /// Add tokens of BackInStock subscription
        /// </summary>
        /// <param name="tokens">List of already added tokens</param>
        /// <param name="subscription">BackInStock subscription</param>
        Task AddBackInStockTokensAsync(IList<Token> tokens, BackInStockSubscription subscription);

        /// <summary>
        /// Get collection of allowed (supported) message tokens for campaigns
        /// </summary>
        /// <returns>Collection of allowed (supported) message tokens for campaigns</returns>
        Task<IEnumerable<string>> GetListOfCampaignAllowedTokensAsync();

        /// <summary>
        /// Get collection of allowed (supported) message tokens
        /// </summary>
        /// <param name="tokenGroups">Collection of token groups; pass null to get all available tokens</param>
        /// <returns>Collection of allowed message tokens</returns>
        Task<IEnumerable<string>> GetListOfAllowedTokensAsync(IEnumerable<string> tokenGroups = null);

        /// <summary>
        /// Get token groups of message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <returns>Collection of token group names</returns>
        IEnumerable<string> GetTokenGroups(MessageTemplate messageTemplate);
    }
}