using System.Collections.Generic;
using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Core.Domain.Vendors;

namespace TVProgViewer.Services.Messages
{
    /// <summary>
    /// Workflow message service
    /// </summary>
    public partial interface IWorkflowMessageService
    {
        #region User workflow

        /// <summary>
        /// Sends 'New user' notification message to a store owner
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendUserRegisteredNotificationMessageAsync(User user, int languageId);

        /// <summary>
        /// Sends a welcome message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendUserWelcomeMessageAsync(User user, int languageId);

        /// <summary>
        /// Sends an email validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendUserEmailValidationMessageAsync(User user, int languageId);

        /// <summary>
        /// Sends an email re-validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendUserEmailRevalidationMessageAsync(User user, int languageId);

        /// <summary>
        /// Sends password recovery message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendUserPasswordRecoveryMessageAsync(User user, int languageId);

        #endregion

        #region Order workflow

        /// <summary>
        /// Sends an order placed notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderPlacedVendorNotificationAsync(Order order, Vendor vendor, int languageId);

        /// <summary>
        /// Sends an order placed notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderPlacedStoreOwnerNotificationAsync(Order order, int languageId);

        /// <summary>
        /// Sends an order placed notification to an affiliate
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderPlacedAffiliateNotificationAsync(Order order, int languageId);

        /// <summary>
        /// Sends an order paid notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderPaidStoreOwnerNotificationAsync(Order order, int languageId);

        /// <summary>
        /// Sends an order paid notification to a user
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderPaidUserNotificationAsync(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null);

        /// <summary>
        /// Sends an order paid notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderPaidVendorNotificationAsync(Order order, Vendor vendor, int languageId);

        /// <summary>
        /// Sends an order paid notification to an affiliate
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderPaidAffiliateNotificationAsync(Order order, int languageId);

        /// <summary>
        /// Sends an order placed notification to a user
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderPlacedUserNotificationAsync(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null);

        /// <summary>
        /// Sends a shipment sent notification to a user
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendShipmentSentUserNotificationAsync(Shipment shipment, int languageId);

        /// <summary>
        /// Sends a shipment delivered notification to a user
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendShipmentDeliveredUserNotificationAsync(Shipment shipment, int languageId);

        /// <summary>
        /// Sends an order completed notification to a user
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderCompletedUserNotificationAsync(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null);

        /// <summary>
        /// Sends an order cancelled notification to a user
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderCancelledUserNotificationAsync(Order order, int languageId);

        /// <summary>
        /// Sends an order refunded notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="refundedAmount">Amount refunded</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderRefundedStoreOwnerNotificationAsync(Order order, decimal refundedAmount, int languageId);

        /// <summary>
        /// Sends an order refunded notification to a user
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="refundedAmount">Amount refunded</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendOrderRefundedUserNotificationAsync(Order order, decimal refundedAmount, int languageId);

        /// <summary>
        /// Sends a new order note added notification to a user
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewOrderNoteAddedUserNotificationAsync(OrderNote orderNote, int languageId);

        /// <summary>
        /// Sends a "Recurring payment cancelled" notification to a store owner
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendRecurringPaymentCancelledStoreOwnerNotificationAsync(RecurringPayment recurringPayment, int languageId);

        /// <summary>
        /// Sends a "Recurring payment cancelled" notification to a user
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendRecurringPaymentCancelledUserNotificationAsync(RecurringPayment recurringPayment, int languageId);

        /// <summary>
        /// Sends a "Recurring payment failed" notification to a user
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendRecurringPaymentFailedUserNotificationAsync(RecurringPayment recurringPayment, int languageId);

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewsLetterSubscriptionActivationMessageAsync(NewsLetterSubscription subscription, int languageId);

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewsLetterSubscriptionDeactivationMessageAsync(NewsLetterSubscription subscription, int languageId);

        #endregion

        #region Send a message to a friend

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product instance</param>
        /// <param name="userEmail">User's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendProductEmailAFriendMessageAsync(User user, int languageId,
            Product product, string userEmail, string friendsEmail, string personalMessage);

        /// <summary>
        /// Sends wishlist "email a friend" message
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="userEmail">User's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendWishlistEmailAFriendMessageAsync(User user, int languageId,
            string userEmail, string friendsEmail, string personalMessage);

        #endregion

        #region Return requests

        /// <summary>
        /// Sends 'New Return Request' message to a store owner
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="order">Order</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewReturnRequestStoreOwnerNotificationAsync(ReturnRequest returnRequest, OrderItem orderItem, Order order, int languageId);

        /// <summary>
        /// Sends 'New Return Request' message to a user
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="order">Order</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewReturnRequestUserNotificationAsync(ReturnRequest returnRequest, OrderItem orderItem, Order order);

        /// <summary>
        /// Sends 'Return Request status changed' message to a user
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="order">Order</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendReturnRequestStatusChangedUserNotificationAsync(ReturnRequest returnRequest, OrderItem orderItem, Order order);

        #endregion

        #region Forum Notifications

        /// <summary>
        /// Sends a forum subscription message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewForumTopicMessageAsync(User user, ForumTopic forumTopic, Forum forum, int languageId);

        /// <summary>
        /// Sends a forum subscription message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewForumPostMessageAsync(User user, ForumPost forumPost,
            ForumTopic forumTopic, Forum forum, int friendlyForumTopicPageIndex, int languageId);

        /// <summary>
        /// Sends a private message notification
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendPrivateMessageNotificationAsync(PrivateMessage privateMessage, int languageId);

        #endregion

        #region Misc

        /// <summary>
        /// Sends 'New vendor account submitted' message to a store owner
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewVendorAccountApplyStoreOwnerNotificationAsync(User user, Vendor vendor, int languageId);

        /// <summary>
        /// Sends 'Vendor information change' message to a store owner
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendVendorInformationChangeNotificationAsync(Vendor vendor, int languageId);

        /// <summary>
        /// Sends a product review notification message to a store owner
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendProductReviewNotificationMessageAsync(ProductReview productReview, int languageId);

        /// <summary>
        /// Sends a product review reply notification message to a user
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendProductReviewReplyUserNotificationMessageAsync(ProductReview productReview, int languageId);

        /// <summary>
        /// Sends a gift card notification
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendGiftCardNotificationAsync(GiftCard giftCard, int languageId);

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendQuantityBelowStoreOwnerNotificationAsync(Product product, int languageId);

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="combination">Attribute combination</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendQuantityBelowStoreOwnerNotificationAsync(ProductAttributeCombination combination, int languageId);

        /// <summary>
        /// Sends a "new VAT submitted" notification to a store owner
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewVatSubmittedStoreOwnerNotificationAsync(User user, string vatName, string vatAddress, int languageId);

        /// <summary>
        /// Sends a blog comment notification message to a store owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendBlogCommentNotificationMessageAsync(BlogComment blogComment, int languageId);

        /// <summary>
        /// Sends a news comment notification message to a store owner
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendNewsCommentNotificationMessageAsync(NewsComment newsComment, int languageId);

        /// <summary>
        /// Sends a 'Back in stock' notification message to a user
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendBackInStockNotificationAsync(BackInStockSubscription subscription, int languageId);

        /// <summary>
        /// Sends "contact us" message
        /// </summary>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendContactUsMessageAsync(int languageId, string senderEmail, string senderName, string subject, string body);

        /// <summary>
        /// Sends "contact vendor" message
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        Task<IList<int>> SendContactVendorMessageAsync(Vendor vendor, int languageId, string senderEmail, string senderName, string subject, string body);

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        Task<int> SendTestEmailAsync(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId);

        #endregion
    }
}