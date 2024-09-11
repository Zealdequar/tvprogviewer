using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Data;
using TvProgViewer.Services.Blogs;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Forums;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.News;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Vendors;
using NUnit.Framework;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Messages
{
    [TestFixture]
    public class WorkflowMessageServiceTests : ServiceTest
    {
        private readonly IWorkflowMessageService _workflowMessageService;

        private readonly List<int> _notActiveTempletes = new();
        private readonly IMessageTemplateService _messageTemplateService;
        private User _user;
        private readonly IRepository<QueuedEmail> _queuedEmailRepository;
        private Order _order;
        private Vendor _vendor;
        private Shipment _shipment;
        private IList<MessageTemplate> _allMessageTemplates;
        private OrderNote _orderNote;
        private RecurringPayment _recurringPayment;
        private NewsLetterSubscription _subscription;
        private TvChannel _tvChannel;
        private OrderItem _orderItem;
        private ReturnRequest _returnRequest;
        private Forum _forum;
        private ForumTopic _forumTopic;
        private ForumPost _forumPost;
        private PrivateMessage _privateMessage;
        private TvChannelReview _tvChannelReview;
        private GiftCard _giftCard;
        private BlogComment _blogComment;
        private NewsComment _newsComment;
        private BackInStockSubscription _backInStockSubscription;
        private readonly IForumService _forumService;

        public WorkflowMessageServiceTests()
        {
            _workflowMessageService = GetService<IWorkflowMessageService>();
            _messageTemplateService = GetService<IMessageTemplateService>();
            _queuedEmailRepository = GetService<IRepository<QueuedEmail>>();
            _forumService = GetService<IForumService>();
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var userService = GetService<IUserService>();
            var orderService = GetService<IOrderService>();
            var vendorService = GetService<IVendorService>();
            var shipmentService = GetService<IShipmentService>();
            var tvChannelService = GetService<ITvChannelService>();
            var giftCardService = GetService<IGiftCardService>();
            var blogService = GetService<IBlogService>();
            var newsService = GetService<INewsService>();

            _order = await orderService.GetOrderByIdAsync(1);
            _orderItem = (await orderService.GetOrderItemsAsync(1)).First();
            _user = await userService.GetUserByEmailAsync(TvProgTestsDefaults.AdminEmail);
            _vendor = await vendorService.GetVendorByIdAsync(1);
            _shipment = await shipmentService.GetShipmentByIdAsync(1);
            _orderNote = await orderService.GetOrderNoteByIdAsync(1);
            _recurringPayment = new RecurringPayment {InitialOrderId = _order.Id, IsActive = true};
            _subscription = new NewsLetterSubscription {Active = true, Email = TvProgTestsDefaults.AdminEmail};
            _tvChannel = await tvChannelService.GetTvChannelByIdAsync(1);
            _returnRequest = new ReturnRequest {UserId = _user.Id, OrderItemId = _orderItem.Id};
            _forum = await _forumService.GetForumByIdAsync(1);
            _forumTopic = new ForumTopic {UserId = _user.Id, ForumId = _forum.Id, Subject = "Subject"};
            await _forumService.InsertTopicAsync(_forumTopic, false);
            _forumPost = new ForumPost { UserId = _user.Id, TopicId = _forumTopic.Id, Text = "Text"};
            await _forumService.InsertPostAsync(_forumPost, false);

            _privateMessage = new PrivateMessage
            {
                FromUserId = 1, ToUserId = 2, Subject = string.Empty, Text = string.Empty
            };
            _tvChannelReview = (await tvChannelService.GetAllTvChannelReviewsAsync()).FirstOrDefault();
            _giftCard = await giftCardService.GetGiftCardByIdAsync(1);
            _blogComment = await blogService.GetBlogCommentByIdAsync(1);
            _newsComment = await newsService.GetNewsCommentByIdAsync(1);
            _backInStockSubscription = new BackInStockSubscription {TvChannelId = _tvChannel.Id, UserId = _user.Id};

            _allMessageTemplates = await _messageTemplateService.GetAllMessageTemplatesAsync(0);

            foreach (var template in _allMessageTemplates.Where(t=>!t.IsActive))
            {
                template.IsActive = true;
                _notActiveTempletes.Add(template.Id);
                await _messageTemplateService.UpdateMessageTemplateAsync(template);
            }
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            foreach (var template in _allMessageTemplates.Where(t => _notActiveTempletes.Contains(t.Id)))
            {
                template.IsActive = false;
                await _messageTemplateService.UpdateMessageTemplateAsync(template);
            }

            await _forumService.DeletePostAsync(_forumPost);
            await _forumService.DeleteTopicAsync(_forumTopic);
        }

        [SetUp]
        public async Task SetUp()
        {
            await _queuedEmailRepository.TruncateAsync();
        }

        protected async Task CheckData(Func<Task<IList<int>>> func)
        {
            var queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
            queuedEmails.Count.Should().Be(0);

            var emailIds = await func();

            emailIds.Count.Should().BeGreaterThan(0);

            queuedEmails = await _queuedEmailRepository.GetAllAsync(query => query);
            queuedEmails.Count.Should().Be(emailIds.Count);
        }

        #region User workflow

        [Test]
        public async Task CanSendUserRegisteredNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendUserRegisteredStoreOwnerNotificationMessageAsync(_user, 1));
        }

        [Test]
        public async Task CanSendUserWelcomeMessage()
        {
            await CheckData(async () => 
                await _workflowMessageService.SendUserWelcomeMessageAsync(_user, 1));
        }

        [Test]
        public async Task CanSendUserEmailValidationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendUserEmailValidationMessageAsync(_user, 1));
        }

        [Test]
        public async Task CanSendUserEmailRevalidationMessage()
        {
            _user.EmailToRevalidate = TvProgTestsDefaults.AdminEmail;
            await CheckData(async () =>
                await _workflowMessageService.SendUserEmailRevalidationMessageAsync(_user, 1));
        }

        [Test]
        public async Task CanSendUserPasswordRecoveryMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendUserPasswordRecoveryMessageAsync(_user, 1));
        }

        #endregion

        #region Order workflow

        [Test]
        public async Task CanSendOrderPlacedVendorNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPlacedVendorNotificationAsync(_order, _vendor, 1));
        }

        [Test]
        public async Task CanSendOrderPlacedStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPlacedStoreOwnerNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPlacedAffiliateNotification()
        {
            _order.AffiliateId = 1;
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPlacedAffiliateNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPaidStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPaidStoreOwnerNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPaidUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPaidUserNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPaidVendorNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPaidVendorNotificationAsync(_order, _vendor, 1));
        }

        [Test]
        public async Task CanSendOrderPaidAffiliateNotification()
        {
            _order.AffiliateId = 1;
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPaidAffiliateNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderPlacedUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderPlacedUserNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendShipmentSentUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendShipmentSentUserNotificationAsync(_shipment, 1));
        }
        [Test]
        public async Task CanSendShipmentDeliveredUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendShipmentDeliveredUserNotificationAsync(_shipment, 1));
        }

        [Test]
        public async Task CanSendOrderCompletedUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderCompletedUserNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderCancelledUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderCancelledUserNotificationAsync(_order, 1));
        }

        [Test]
        public async Task CanSendOrderRefundedStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderRefundedStoreOwnerNotificationAsync(_order, 1M, 1));
        }

        [Test]
        public async Task CanSendOrderRefundedUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendOrderRefundedUserNotificationAsync(_order, 1M, 1));
        }

        [Test]
        public async Task CanSendNewOrderNoteAddedUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewOrderNoteAddedUserNotificationAsync(_orderNote, 1));
        }

        [Test]
        public async Task CanSendRecurringPaymentCancelledStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendRecurringPaymentCancelledStoreOwnerNotificationAsync(_recurringPayment, 1));
        }

        [Test]
        public async Task CanSendRecurringPaymentCancelledUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendRecurringPaymentCancelledUserNotificationAsync(_recurringPayment, 1));
        }

        [Test]
        public async Task CanSendRecurringPaymentFailedUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendRecurringPaymentFailedUserNotificationAsync(_recurringPayment, 1));
        }

        #endregion

        #region Newsletter workflow

        [Test]
        public async Task CanSendNewsLetterSubscriptionActivationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewsLetterSubscriptionActivationMessageAsync(_subscription, 1));
        }

        [Test]
        public async Task CanSendNewsLetterSubscriptionDeactivationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessageAsync(_subscription, 1));
        }

        #endregion

        #region Send a message to a friend

        [Test]
        public async Task CanSendTvChannelEmailAFriendMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendTvChannelEmailAFriendMessageAsync(_user, 1, _tvChannel, TvProgTestsDefaults.AdminEmail, TvProgTestsDefaults.AdminEmail, string.Empty));
        }

        [Test]
        public async Task CanSendWishlistEmailAFriendMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendWishlistEmailAFriendMessageAsync(_user, 1, TvProgTestsDefaults.AdminEmail, TvProgTestsDefaults.AdminEmail, string.Empty));
        }

        #endregion

        #region Return requests

        [Test]
        public async Task CanSendNewReturnRequestStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewReturnRequestStoreOwnerNotificationAsync(_returnRequest, _orderItem, _order, 1));
        }

        [Test]
        public async Task CanSendNewReturnRequestUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewReturnRequestUserNotificationAsync(_returnRequest, _orderItem, _order));
        }

        [Test]
        public async Task CanSendReturnRequestStatusChangedUserNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendReturnRequestStatusChangedUserNotificationAsync(_returnRequest, _orderItem, _order));
        }

        #endregion

        #region Forum Notifications

        [Test]
        public async Task CanSendNewForumTopicMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewForumTopicMessageAsync(_user, _forumTopic, _forum, 1));
        }

        [Test]
        public async Task CanSendNewForumPostMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewForumPostMessageAsync(_user, _forumPost, _forumTopic, _forum, 1, 1));
        }

        [Test]
        public async Task CanSendPrivateMessageNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendPrivateMessageNotificationAsync(_privateMessage, 1));
        }

        #endregion

        #region Misc

        [Test]
        public async Task CanSendNewVendorAccountApplyStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewVendorAccountApplyStoreOwnerNotificationAsync(_user, _vendor, 1));
        }

        [Test]
        public async Task CanSendVendorInformationChangeNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendVendorInformationChangeStoreOwnerNotificationAsync(_vendor, 1));
        }

        [Test]
        public async Task CanSendTvChannelReviewNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendTvChannelReviewStoreOwnerNotificationMessageAsync(_tvChannelReview, 1));
        }

        [Test]
        public async Task CanSendTvChannelReviewReplyUserNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendTvChannelReviewReplyUserNotificationMessageAsync(_tvChannelReview, 1));
        }

        [Test]
        public async Task CanSendGiftCardNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendGiftCardNotificationAsync(_giftCard, 1));
        }

        [Test]
        public async Task CanSendQuantityBelowStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(_tvChannel, 1));
        }
        
        [Test]
        public async Task CanSendNewVatSubmittedStoreOwnerNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewVatSubmittedStoreOwnerNotificationAsync(_user, "vat name", "vat address", 1));
        }

        [Test]
        public async Task CanSendBlogCommentNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendBlogCommentStoreOwnerNotificationMessageAsync(_blogComment, 1));
        }

        [Test]
        public async Task CanSendNewsCommentNotificationMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendNewsCommentStoreOwnerNotificationMessageAsync(_newsComment, 1));
        }

        [Test]
        public async Task CanSendBackInStockNotification()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendBackInStockNotificationAsync(_backInStockSubscription, 1));
        }

        [Test]
        public async Task CanSendContactUsMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendContactUsMessageAsync(1, TvProgTestsDefaults.AdminEmail, "sender name", "subject", "body"));
        }

        [Test]
        public async Task CanSendContactVendorMessage()
        {
            await CheckData(async () =>
                await _workflowMessageService.SendContactVendorMessageAsync(_vendor, 1, TvProgTestsDefaults.AdminEmail, "sender name", "subject", "body"));
        }

        #endregion
    }
}
