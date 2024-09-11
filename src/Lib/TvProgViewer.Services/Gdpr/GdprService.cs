using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Events;
using TvProgViewer.Data;
using TvProgViewer.Services.Authentication.External;
using TvProgViewer.Services.Blogs;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Forums;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.News;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Stores;

namespace TvProgViewer.Services.Gdpr
{
    /// <summary>
    /// Represents the GDPR service
    /// </summary>
    public partial class GdprService : IGdprService
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly IBlogService _blogService;
        private readonly IUserService _userService;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IForumService _forumService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly INewsService _newsService;
        private readonly ITvChannelService _tvChannelService;
        private readonly IRepository<GdprConsent> _gdprConsentRepository;
        private readonly IRepository<GdprLog> _gdprLogRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public GdprService(IAddressService addressService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IBlogService blogService,
            IUserService userService,
            IExternalAuthenticationService externalAuthenticationService,
            IEventPublisher eventPublisher,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            INewsService newsService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            ITvChannelService tvChannelService,
            IRepository<GdprConsent> gdprConsentRepository,
            IRepository<GdprLog> gdprLogRepository,
            IShoppingCartService shoppingCartService,
            IStoreService storeService)
        {
            _addressService = addressService;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _blogService = blogService;
            _userService = userService;
            _externalAuthenticationService = externalAuthenticationService;
            _eventPublisher = eventPublisher;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _newsService = newsService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _tvChannelService = tvChannelService;
            _gdprConsentRepository = gdprConsentRepository;
            _gdprLogRepository = gdprLogRepository;
            _shoppingCartService = shoppingCartService;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Insert a GDPR log
        /// </summary>
        /// <param name="gdprLog">GDPR log</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InsertLogAsync(GdprLog gdprLog)
        {
            await _gdprLogRepository.InsertAsync(gdprLog);
        }

        #endregion

        #region Methods

        #region GDPR consent

        /// <summary>
        /// Get a GDPR consent
        /// </summary>
        /// <param name="gdprConsentId">The GDPR consent identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR consent
        /// </returns>
        public virtual async Task<GdprConsent> GetConsentByIdAsync(int gdprConsentId)
        {
            return await _gdprConsentRepository.GetByIdAsync(gdprConsentId, cache => default);
        }

        /// <summary>
        /// Get all GDPR consents
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR consent
        /// </returns>
        public virtual async Task<IList<GdprConsent>> GetAllConsentsAsync()
        {
            var gdprConsents = await _gdprConsentRepository.GetAllAsync(query =>
            {
                return from c in query
                    orderby c.DisplayOrder, c.Id
                    select c;
            }, cache => default);

            return gdprConsents;
        }

        /// <summary>
        /// Insert a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertConsentAsync(GdprConsent gdprConsent)
        {
            await _gdprConsentRepository.InsertAsync(gdprConsent);
        }

        /// <summary>
        /// Update the GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateConsentAsync(GdprConsent gdprConsent)
        {
            await _gdprConsentRepository.UpdateAsync(gdprConsent);
        }

        /// <summary>
        /// Delete a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteConsentAsync(GdprConsent gdprConsent)
        {
            await _gdprConsentRepository.DeleteAsync(gdprConsent);
        }

        /// <summary>
        /// Gets the latest selected value (a consent is accepted or not by a user)
        /// </summary>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result; null if previous a user hasn't been asked
        /// </returns>
        public virtual async Task<bool?> IsConsentAcceptedAsync(int consentId, int userId)
        {
            //get latest record
            var log = (await GetAllLogAsync(userId: userId, consentId: consentId, pageIndex: 0, pageSize: 1)).FirstOrDefault();
            if (log == null)
                return null;

            return log.RequestType switch
            {
                GdprRequestType.ConsentAgree => true,
                GdprRequestType.ConsentDisagree => false,
                _ => null,
            };
        }

        #endregion

        #region GDPR log
        
        /// <summary>
        /// Get all GDPR log records
        /// </summary>
        /// <param name="userId">User identifier</param>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="userInfo">User info (Exact match)</param>
        /// <param name="requestType">GDPR request type</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR log records
        /// </returns>
        public virtual async Task<IPagedList<GdprLog>> GetAllLogAsync(int userId = 0, int consentId = 0,
            string userInfo = "", GdprRequestType? requestType = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return await _gdprLogRepository.GetAllPagedAsync(query =>
            {
                if (userId > 0) 
                    query = query.Where(log => log.UserId == userId);

                if (consentId > 0) 
                    query = query.Where(log => log.ConsentId == consentId);

                if (!string.IsNullOrEmpty(userInfo)) 
                    query = query.Where(log => log.UserInfo == userInfo);

                if (requestType != null)
                {
                    var requestTypeId = (int)requestType;
                    query = query.Where(log => log.RequestTypeId == requestTypeId);
                }

                query = query.OrderByDescending(log => log.CreatedOnUtc).ThenByDescending(log => log.Id);

                return query;
            }, pageIndex, pageSize);
        }

        /// <summary>
        /// Insert a GDPR log
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="requestType">Request type</param>
        /// <param name="requestDetails">Request details</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertLogAsync(User user, int consentId, GdprRequestType requestType, string requestDetails)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var gdprLog = new GdprLog
            {
                UserId = user.Id,
                ConsentId = consentId,
                UserInfo = user.Email,
                RequestType = requestType,
                RequestDetails = requestDetails,
                CreatedOnUtc = DateTime.UtcNow
            };

            await InsertLogAsync(gdprLog);
        }
        
        #endregion

        #region User

        /// <summary>
        /// Permanent delete of user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PermanentDeleteUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //blog comments
            var blogComments = await _blogService.GetAllCommentsAsync(userId: user.Id);
            await _blogService.DeleteBlogCommentsAsync(blogComments);

            //news comments
            var newsComments = await _newsService.GetAllCommentsAsync(userId: user.Id);
            await _newsService.DeleteNewsCommentsAsync(newsComments);

            //back in stock subscriptions
            var backInStockSubscriptions = await _backInStockSubscriptionService.GetAllSubscriptionsByUserIdAsync(user.Id);
            foreach (var backInStockSubscription in backInStockSubscriptions)
                await _backInStockSubscriptionService.DeleteSubscriptionAsync(backInStockSubscription);

            //tvChannel review
            var tvChannelReviews = await _tvChannelService.GetAllTvChannelReviewsAsync(user.Id);
            var reviewedTvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(tvChannelReviews.Select(p => p.TvChannelId).Distinct().ToArray());
            await _tvChannelService.DeleteTvChannelReviewsAsync(tvChannelReviews);
            //update tvChannel totals
            foreach (var tvChannel in reviewedTvChannels) 
                await _tvChannelService.UpdateTvChannelReviewTotalsAsync(tvChannel);

            //external authentication record
            foreach (var ear in await _externalAuthenticationService.GetUserExternalAuthenticationRecordsAsync(user))
                await _externalAuthenticationService.DeleteExternalAuthenticationRecordAsync(ear);

            //forum subscriptions
            var forumSubscriptions = await _forumService.GetAllSubscriptionsAsync(user.Id);
            foreach (var forumSubscription in forumSubscriptions)
                await _forumService.DeleteSubscriptionAsync(forumSubscription);

            //shopping cart items
            foreach (var sci in await _shoppingCartService.GetShoppingCartAsync(user))
                await _shoppingCartService.DeleteShoppingCartItemAsync(sci);

            //private messages (sent)
            foreach (var pm in await _forumService.GetAllPrivateMessagesAsync(0, user.Id, 0, null, null, null, null))
                await _forumService.DeletePrivateMessageAsync(pm);

            //private messages (received)
            foreach (var pm in await _forumService.GetAllPrivateMessagesAsync(0, 0, user.Id, null, null, null, null))
                await _forumService.DeletePrivateMessageAsync(pm);

            //newsletter
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(user.Email, store.Id);
                if (newsletter != null)
                    await _newsLetterSubscriptionService.DeleteNewsLetterSubscriptionAsync(newsletter);
            }

            //addresses
            foreach (var address in await _userService.GetAddressesByUserIdAsync(user.Id))
            {
                await _userService.RemoveUserAddressAsync(user, address);
                await _userService.UpdateUserAsync(user);
                //now delete the address record
                await _addressService.DeleteAddressAsync(address);
            }

            //generic attributes
            var keyGroup = user.GetType().Name;
            var genericAttributes = await _genericAttributeService.GetAttributesForEntityAsync(user.Id, keyGroup);
            await _genericAttributeService.DeleteAttributesAsync(genericAttributes);

            //ignore ActivityLog
            //ignore ForumPost, ForumTopic, ignore ForumPostVote
            //ignore Log
            //ignore PollVotingRecord
            //ignore TvChannelReviewHelpfulness
            //ignore RecurringPayment 
            //ignore ReturnRequest
            //ignore RewardPointsHistory
            //and we do not delete orders

            //remove from Registered role, add to Guest one
            if (await _userService.IsRegisteredAsync(user))
            {
                var registeredRole = await _userService.GetUserRoleBySystemNameAsync(TvProgUserDefaults.RegisteredRoleName);
                await _userService.RemoveUserRoleMappingAsync(user, registeredRole);
            }

            if (!await _userService.IsGuestAsync(user))
            {
                var guestRole = await _userService.GetUserRoleBySystemNameAsync(TvProgUserDefaults.GuestsRoleName);
                await _userService.AddUserRoleMappingAsync(new UserUserRoleMapping { UserId = user.Id, UserRoleId = guestRole.Id });
            }

            var email = user.Email;

            //clear other information
            user.Email = string.Empty;
            user.EmailToRevalidate = string.Empty;
            user.Username = string.Empty;
            user.Active = false;
            user.Deleted = true;
            
            await _userService.UpdateUserAsync(user);

            //raise event
            await _eventPublisher.PublishAsync(new UserPermanentlyDeleted(user.Id, email));
        }

        #endregion

        #endregion
    }
}