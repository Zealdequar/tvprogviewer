using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Gdpr;
using TVProgViewer.Data;
using TVProgViewer.Services.Authentication.External;
using TVProgViewer.Services.Blogs;
using TVProgViewer.Services.Caching.Extensions;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Forums;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.News;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Stores;

namespace TVProgViewer.Services.Gdpr
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
        private readonly IProductService _productService;
        private readonly IRepository<GdprConsent> _gdprConsentRepository;
        private readonly IRepository<GdprLog> _gdprLogRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public GdprService(IAddressService addressService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IBlogService blogService,
            IUserService UserService,
            IExternalAuthenticationService externalAuthenticationService,
            IEventPublisher eventPublisher,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            INewsService newsService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IProductService productService,
            IRepository<GdprConsent> gdprConsentRepository,
            IRepository<GdprLog> gdprLogRepository,
            IShoppingCartService shoppingCartService,
            IStoreService storeService)
        {
            _addressService = addressService;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _blogService = blogService;
            _userService = UserService;
            _externalAuthenticationService = externalAuthenticationService;
            _eventPublisher = eventPublisher;
            _forumService = forumService;
            _genericAttributeService = genericAttributeService;
            _newsService = newsService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _productService = productService;
            _gdprConsentRepository = gdprConsentRepository;
            _gdprLogRepository = gdprLogRepository;
            _shoppingCartService = shoppingCartService;
            _storeService = storeService;
        }

        #endregion

        #region Methods

        #region GDPR consent

        /// <summary>
        /// Get a GDPR consent
        /// </summary>
        /// <param name="gdprConsentId">The GDPR consent identifier</param>
        /// <returns>GDPR consent</returns>
        public virtual GdprConsent GetConsentById(int gdprConsentId)
        {
            if (gdprConsentId == 0)
                return null;

            return _gdprConsentRepository.ToCachedGetById(gdprConsentId);
        }

        /// <summary>
        /// Get all GDPR consents
        /// </summary>
        /// <returns>GDPR consent</returns>
        public virtual IList<GdprConsent> GetAllConsents()
        {
            var query = from c in _gdprConsentRepository.Table
                        orderby c.DisplayOrder, c.Id
                        select c;
            var gdprConsents = query.ToList();
            return gdprConsents;
        }

        /// <summary>
        /// Insert a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        public virtual void InsertConsent(GdprConsent gdprConsent)
        {
            if (gdprConsent == null)
                throw new ArgumentNullException(nameof(gdprConsent));

            _gdprConsentRepository.Insert(gdprConsent);

            //event notification
            _eventPublisher.EntityInserted(gdprConsent);
        }

        /// <summary>
        /// Update the GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        public virtual void UpdateConsent(GdprConsent gdprConsent)
        {
            if (gdprConsent == null)
                throw new ArgumentNullException(nameof(gdprConsent));

            _gdprConsentRepository.Update(gdprConsent);

            //event notification
            _eventPublisher.EntityUpdated(gdprConsent);
        }

        /// <summary>
        /// Delete a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        public virtual void DeleteConsent(GdprConsent gdprConsent)
        {
            if (gdprConsent == null)
                throw new ArgumentNullException(nameof(gdprConsent));

            _gdprConsentRepository.Delete(gdprConsent);

            //event notification
            _eventPublisher.EntityDeleted(gdprConsent);
        }

        /// <summary>
        /// Gets the latest selected value (a consent is accepted or not by a User)
        /// </summary>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="UserId">User identifier</param>
        /// <returns>Result; null if previous a User hasn't been asked</returns>
        public virtual bool? IsConsentAccepted(int consentId, int UserId)
        {
            //get latest record
            var log = GetAllLog(UserId: UserId, consentId: consentId, pageIndex: 0, pageSize: 1).FirstOrDefault();
            if (log == null)
                return null;

            switch (log.RequestType)
            {
                case GdprRequestType.ConsentAgree:
                    return true;
                case GdprRequestType.ConsentDisagree:
                    return false;
                default:
                    return null;
            }
        }
        #endregion

        #region GDPR log

        /// <summary>
        /// Get a GDPR log
        /// </summary>
        /// <param name="gdprLogId">The GDPR log identifier</param>
        /// <returns>GDPR log</returns>
        public virtual GdprLog GetLogById(int gdprLogId)
        {
            if (gdprLogId == 0)
                return null;

            return _gdprLogRepository.ToCachedGetById(gdprLogId);
        }

        /// <summary>
        /// Get all GDPR log records
        /// </summary>
        /// <param name="UserId">User identifier</param>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="UserInfo">User info (Exact match)</param>
        /// <param name="requestType">GDPR request type</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>GDPR log records</returns>
        public virtual IPagedList<GdprLog> GetAllLog(int UserId = 0, int consentId = 0,
            string UserInfo = "", GdprRequestType? requestType = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _gdprLogRepository.Table;
            if (UserId > 0)
            {
                query = query.Where(log => log.UserId == UserId);
            }

            if (consentId > 0)
            {
                query = query.Where(log => log.ConsentId == consentId);
            }

            if (!string.IsNullOrEmpty(UserInfo))
            {
                query = query.Where(log => log.UserInfo == UserInfo);
            }

            if (requestType != null)
            {
                var requestTypeId = (int)requestType;
                query = query.Where(log => log.RequestTypeId == requestTypeId);
            }

            query = query.OrderByDescending(log => log.CreatedOnUtc).ThenByDescending(log => log.Id);
            return new PagedList<GdprLog>(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Insert a GDPR log
        /// </summary>
        /// <param name="gdprLog">GDPR log</param>
        public virtual void InsertLog(GdprLog gdprLog)
        {
            if (gdprLog == null)
                throw new ArgumentNullException(nameof(gdprLog));

            _gdprLogRepository.Insert(gdprLog);

            //event notification
            _eventPublisher.EntityInserted(gdprLog);
        }

        /// <summary>
        /// Insert a GDPR log
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="requestType">Request type</param>
        /// <param name="requestDetails">Request details</param>
        public virtual void InsertLog(User User, int consentId, GdprRequestType requestType, string requestDetails)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var gdprLog = new GdprLog
            {
                UserId = User.Id,
                ConsentId = consentId,
                UserInfo = User.Email,
                RequestType = requestType,
                RequestDetails = requestDetails,
                CreatedOnUtc = DateTime.UtcNow
            };
            InsertLog(gdprLog);
        }

        /// <summary>
        /// Update the GDPR log
        /// </summary>
        /// <param name="gdprLog">GDPR log</param>
        public virtual void UpdateLog(GdprLog gdprLog)
        {
            if (gdprLog == null)
                throw new ArgumentNullException(nameof(gdprLog));

            _gdprLogRepository.Update(gdprLog);

            //event notification
            _eventPublisher.EntityUpdated(gdprLog);
        }

        /// <summary>
        /// Delete a GDPR log
        /// </summary>
        /// <param name="gdprLog">GDPR log</param>
        public virtual void DeleteLog(GdprLog gdprLog)
        {
            if (gdprLog == null)
                throw new ArgumentNullException(nameof(gdprLog));

            _gdprLogRepository.Delete(gdprLog);

            //event notification
            _eventPublisher.EntityDeleted(gdprLog);
        }

        #endregion

        #region User

        /// <summary>
        /// Перманентное удаление пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        public virtual void PermanentDeleteUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Комменты блога
            var blogComments = _blogService.GetAllComments(UserId: user.Id);
            _blogService.DeleteBlogComments(blogComments);

            // Новостные комменты
            var newsComments = _newsService.GetAllComments(UserId: user.Id);
            _newsService.DeleteNewsComments(newsComments);

            //back in stock subscriptions
            var backInStockSubscriptions = _backInStockSubscriptionService.GetAllSubscriptionsByUserId(user.Id);
            foreach (var backInStockSubscription in backInStockSubscriptions)
                _backInStockSubscriptionService.DeleteSubscription(backInStockSubscription);

            //product review
            var productReviews = _productService.GetAllProductReviews(user.Id);
            var reviewedProducts = _productService.GetProductsByIds(productReviews.Select(p => p.ProductId).Distinct().ToArray());
            _productService.DeleteProductReviews(productReviews);
            //update product totals
            foreach (var product in reviewedProducts)
            {
                _productService.UpdateProductReviewTotals(product);
            }
            
            //external authentication record
            foreach (var ear in _externalAuthenticationService.GetUserExternalAuthenticationRecords(user))
                _externalAuthenticationService.DeleteExternalAuthenticationRecord(ear);

            //forum subscriptions
            var forumSubscriptions = _forumService.GetAllSubscriptions(user.Id);
            foreach (var forumSubscription in forumSubscriptions)
                _forumService.DeleteSubscription(forumSubscription);

            //shopping cart items
            foreach (var sci in _shoppingCartService.GetShoppingCart(user))
                _shoppingCartService.DeleteShoppingCartItem(sci);

            //private messages (sent)
            foreach (var pm in _forumService.GetAllPrivateMessages(0, user.Id, 0, null, null, null, null))
                _forumService.DeletePrivateMessage(pm);

            //private messages (received)
            foreach (var pm in _forumService.GetAllPrivateMessages(0, 0, user.Id, null, null, null, null))
                _forumService.DeletePrivateMessage(pm);

            //newsletter
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(user.Email, store.Id);
                if (newsletter != null)
                    _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
            }

            //generic attributes
            var keyGroup = user.GetType().Name;
            var genericAttributes = _genericAttributeService.GetAttributesForEntity(user.Id, keyGroup);
            _genericAttributeService.DeleteAttributes(genericAttributes);

            //ignore ActivityLog
            //ignore ForumPost, ForumTopic, ignore ForumPostVote
            //ignore Log
            //ignore PollVotingRecord
            //ignore ProductReviewHelpfulness
            //ignore RecurringPayment 
            //ignore ReturnRequest
            //ignore RewardPointsHistory
            //and we do not delete orders

            //remove from Registered role, add to Guest one
            if (_userService.IsRegistered(user))
            {
                var registeredRole = _userService.GetUserRoleBySystemName(TvProgUserDefaults.RegisteredRoleName);
                _userService.RemoveUserRoleMapping(user, registeredRole);
            }

            if (!_userService.IsGuest(user))
            {
                var guestRole = _userService.GetUserRoleBySystemName(TvProgUserDefaults.GuestsRoleName);
                _userService.AddUserRoleMapping(new UserUserRoleMapping { UserId = user.Id, UserRoleId = guestRole.Id });
            }

            var email = user.Email;

            // Очистка другой информации:
            user.Email = string.Empty;
            user.EmailToRevalidate = string.Empty;
            user.UserName = string.Empty;
            user.Active = false;
            user.Deleted = DateTime.Now;
            _userService.UpdateUser(user);

            //raise event
            _eventPublisher.Publish(new UserPermanentlyDeleted(user.Id, email));
        }

        #endregion

        #endregion
    }
}