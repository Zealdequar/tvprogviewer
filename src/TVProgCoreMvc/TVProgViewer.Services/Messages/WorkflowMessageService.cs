using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TVProgViewer.Core;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Forums;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Messages;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Shipping;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Services.Affiliates;
using TVProgViewer.Services.Catalog;
using TVProgViewer.Services.Common;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Orders;
using TVProgViewer.Services.Stores;

namespace TVProgViewer.Services.Messages
{
    /// <summary>
    /// Workflow message service
    /// </summary>
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IAddressService _addressService;
        private readonly IAffiliateService _affiliateService;
        private readonly IUserService _userService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITokenizer _tokenizer;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public WorkflowMessageService(CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            IAddressService addressService,
            IAffiliateService affiliateService,
            IUserService UserService,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            IOrderService orderService,
            IProductService productService,
            IQueuedEmailService queuedEmailService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITokenizer tokenizer,
            LocalizationSettings localizationSettings)
        {
            _commonSettings = commonSettings;
            _emailAccountSettings = emailAccountSettings;
            _addressService = addressService;
            _affiliateService = affiliateService;
            _userService = UserService;
            _emailAccountService = emailAccountService;
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _localizationService = localizationService;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _orderService = orderService;
            _productService = productService;
            _queuedEmailService = queuedEmailService;
            _storeContext = storeContext;
            _storeService = storeService;
            _tokenizer = tokenizer;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get active message templates by the name
        /// </summary>
        /// <param name="messageTemplateName">Message template name</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>List of message templates</returns>
        protected virtual IList<MessageTemplate> GetActiveMessageTemplates(string messageTemplateName, int storeId)
        {
            //get message templates by the name
            var messageTemplates = _messageTemplateService.GetMessageTemplatesByName(messageTemplateName, storeId);

            //no template found
            if (!messageTemplates?.Any() ?? true)
                return new List<MessageTemplate>();

            //filter active templates
            messageTemplates = messageTemplates.Where(messageTemplate => messageTemplate.IsActive).ToList();

            return messageTemplates;
        }

        /// <summary>
        /// Get EmailAccount to use with a message templates
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>EmailAccount</returns>
        protected virtual EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
        {
            var emailAccountId = _localizationService.GetLocalized(messageTemplate, mt => mt.EmailAccountId, languageId);
            //some 0 validation (for localizable "Email account" dropdownlist which saves 0 if "Standard" value is chosen)
            if (emailAccountId == 0)
                emailAccountId = messageTemplate.EmailAccountId;

            var emailAccount = (_emailAccountService.GetEmailAccountById(emailAccountId) ?? _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId)) ??
                               _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
            return emailAccount;
        }

        /// <summary>
        /// Ensure language is active
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>Return a value language identifier</returns>
        protected virtual int EnsureLanguageIsActive(int languageId, int storeId)
        {
            //load language by specified ID
            var language = _languageService.GetLanguageById(languageId);

            if (language == null || !language.Published)
            {
                //load any language from the specified store
                language = _languageService.GetAllLanguages(storeId: storeId).FirstOrDefault();
            }

            if (language == null || !language.Published)
            {
                //load any language
                language = _languageService.GetAllLanguages().FirstOrDefault();
            }

            if (language == null)
                throw new Exception("No active language could be loaded");

            return language.Id;
        }

        #endregion

        #region Methods

        #region User workflow

        /// <summary>
        /// Sends 'New User' notification message to a store owner
        /// </summary>
        /// <param name="User">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendUserRegisteredNotificationMessage(User User, int languageId)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.UserRegisteredNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a welcome message to a User
        /// </summary>
        /// <param name="User">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendUserWelcomeMessage(User User, int languageId)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.UserWelcomeMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = User.Email;
                var toName = _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an email validation message to a User
        /// </summary>
        /// <param name="User">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendUserEmailValidationMessage(User User, int languageId)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.UserEmailValidationMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = User.Email;
                var toName = _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an email re-validation message to a User
        /// </summary>
        /// <param name="User">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendUserEmailRevalidationMessage(User User, int languageId)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.UserEmailRevalidationMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                //email to re-validate
                var toEmail = User.EmailToRevalidate;
                var toName = _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends password recovery message to a User
        /// </summary>
        /// <param name="User">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendUserPasswordRecoveryMessage(User User, int languageId)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.UserPasswordRecoveryMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = User.Email;
                var toName = _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        #endregion

        #region Order workflow

        /// <summary>
        /// Sends an order placed notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderPlacedVendorNotification(Order order, Vendor vendor, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPlacedVendorNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId, vendor.Id);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = vendor.Email;
                var toName = vendor.Name;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order placed notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderPlacedStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPlacedStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order placed notification to an affiliate
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderPlacedAffiliateNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var affiliate = _affiliateService.GetAffiliateById(order.AffiliateId);

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPlacedAffiliateNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var affiliateAddress = _addressService.GetAddressById(affiliate.AddressId);
                var toEmail = affiliateAddress.Email;
                var toName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order paid notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderPaidStoreOwnerNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPaidStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order paid notification to an affiliate
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderPaidAffiliateNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var affiliate = _affiliateService.GetAffiliateById(order.AffiliateId);

            if (affiliate == null)
                throw new ArgumentNullException(nameof(affiliate));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPaidAffiliateNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var affiliateAddress = _addressService.GetAddressById(affiliate.AddressId);
                var toEmail = affiliateAddress.Email;
                var toName = $"{affiliateAddress.FirstName} {affiliateAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order paid notification to a User
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderPaidUserNotification(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPaidUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    attachmentFilePath, attachmentFileName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order paid notification to a vendor
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="vendor">Vendor instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderPaidVendorNotification(Order order, Vendor vendor, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPaidVendorNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId, vendor.Id);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = vendor.Email;
                var toName = vendor.Name;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order placed notification to a User
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderPlacedUserNotification(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderPlacedUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    attachmentFilePath, attachmentFileName);
            }).ToList();
        }

        /// <summary>
        /// Sends a shipment sent notification to a User
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendShipmentSentUserNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = _orderService.GetOrderById(shipment.OrderId);
            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.ShipmentSentUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddShipmentTokens(commonTokens, shipment, languageId);
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a shipment delivered notification to a User
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendShipmentDeliveredUserNotification(Shipment shipment, int languageId)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = _orderService.GetOrderById(shipment.OrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.ShipmentDeliveredUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddShipmentTokens(commonTokens, shipment, languageId);
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order completed notification to a User
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name. If specified, then this file name will be sent to a recipient. Otherwise, "AttachmentFilePath" name will be used.</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderCompletedUserNotification(Order order, int languageId,
            string attachmentFilePath = null, string attachmentFileName = null)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderCompletedUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    attachmentFilePath, attachmentFileName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order cancelled notification to a User
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderCancelledUserNotification(Order order, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderCancelledUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order refunded notification to a store owner
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="refundedAmount">Amount refunded</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderRefundedStoreOwnerNotification(Order order, decimal refundedAmount, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderRefundedStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddOrderRefundedTokens(commonTokens, order, refundedAmount);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends an order refunded notification to a User
        /// </summary>
        /// <param name="order">Order instance</param>
        /// <param name="refundedAmount">Amount refunded</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendOrderRefundedUserNotification(Order order, decimal refundedAmount, int languageId)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.OrderRefundedUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddOrderRefundedTokens(commonTokens, order, refundedAmount);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a new order note added notification to a User
        /// </summary>
        /// <param name="orderNote">Order note</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendNewOrderNoteAddedUserNotification(OrderNote orderNote, int languageId)
        {
            if (orderNote == null)
                throw new ArgumentNullException(nameof(orderNote));

            var order = _orderService.GetOrderById(orderNote.OrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewOrderNoteAddedUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderNoteTokens(commonTokens, orderNote);
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a "Recurring payment cancelled" notification to a store owner
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendRecurringPaymentCancelledStoreOwnerNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            var order = _orderService.GetOrderById(recurringPayment.InitialOrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.RecurringPaymentCancelledStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);
            _messageTokenProvider.AddRecurringPaymentTokens(commonTokens, recurringPayment);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a "Recurring payment cancelled" notification to a User
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendRecurringPaymentCancelledUserNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            var order = _orderService.GetOrderById(recurringPayment.InitialOrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.RecurringPaymentCancelledUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);
            _messageTokenProvider.AddRecurringPaymentTokens(commonTokens, recurringPayment);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a "Recurring payment failed" notification to a User
        /// </summary>
        /// <param name="recurringPayment">Recurring payment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendRecurringPaymentFailedUserNotification(RecurringPayment recurringPayment, int languageId)
        {
            if (recurringPayment == null)
                throw new ArgumentNullException(nameof(recurringPayment));

            var order = _orderService.GetOrderById(recurringPayment.InitialOrderId);

            if (order == null)
                throw new Exception("Order cannot be loaded");

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.RecurringPaymentFailedUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, order.UserId);
            _messageTokenProvider.AddRecurringPaymentTokens(commonTokens, recurringPayment);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = billingAddress.Email;
                var toName = $"{billingAddress.FirstName} {billingAddress.LastName}";

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        #endregion

        #region Newsletter workflow

        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewsletterSubscriptionActivationMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(commonTokens, subscription);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
            }).ToList();
        }

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewsletterSubscriptionDeactivationMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddNewsLetterSubscriptionTokens(commonTokens, subscription);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, subscription.Email, string.Empty);
            }).ToList();
        }

        #endregion

        #region Send a message to a friend

        /// <summary>
        /// Sends "email a friend" message
        /// </summary>
        /// <param name="User">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="product">Product instance</param>
        /// <param name="UserEmail">User's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendProductEmailAFriendMessage(User User, int languageId,
            Product product, string UserEmail, string friendsEmail, string personalMessage)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.EmailAFriendMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);
            _messageTokenProvider.AddProductTokens(commonTokens, product, languageId);
            commonTokens.Add(new Token("EmailAFriend.PersonalMessage", personalMessage, true));
            commonTokens.Add(new Token("EmailAFriend.Email", UserEmail));

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
            }).ToList();
        }

        /// <summary>
        /// Sends wishlist "email a friend" message
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="UserEmail">User's email</param>
        /// <param name="friendsEmail">Friend's email</param>
        /// <param name="personalMessage">Personal message</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendWishlistEmailAFriendMessage(User User, int languageId,
             string UserEmail, string friendsEmail, string personalMessage)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.WishlistToFriendMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);
            commonTokens.Add(new Token("Wishlist.PersonalMessage", personalMessage, true));
            commonTokens.Add(new Token("Wishlist.Email", UserEmail));

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, friendsEmail, string.Empty);
            }).ToList();
        }

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
        public virtual IList<int> SendNewReturnRequestStoreOwnerNotification(ReturnRequest returnRequest, OrderItem orderItem, Order order, int languageId)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewReturnRequestStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, returnRequest.UserId);
            _messageTokenProvider.AddReturnRequestTokens(commonTokens, returnRequest, orderItem);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends 'New Return Request' message to a User
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="order">Order</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendNewReturnRequestUserNotification(ReturnRequest returnRequest, OrderItem orderItem, Order order)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            var languageId = EnsureLanguageIsActive(order.UserLanguageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewReturnRequestUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var User = _userService.GetUserById(returnRequest.UserId);

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, User);
            _messageTokenProvider.AddReturnRequestTokens(commonTokens, returnRequest, orderItem);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = _userService.IsGuest(User) ?
                    billingAddress.Email :
                    User.Email;
                var toName = _userService.IsGuest(User) ?
                    billingAddress.FirstName :
                    _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends 'Return Request status changed' message to a User
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        /// <param name="orderItem">Order item</param>
        /// <param name="order">Order</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendReturnRequestStatusChangedUserNotification(ReturnRequest returnRequest, OrderItem orderItem, Order order)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var store = _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore;
            var languageId = EnsureLanguageIsActive(order.UserLanguageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.ReturnRequestStatusChangedUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var User = _userService.GetUserById(returnRequest.UserId);

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddOrderTokens(commonTokens, order, languageId);
            _messageTokenProvider.AddUserTokens(commonTokens, User);
            _messageTokenProvider.AddReturnRequestTokens(commonTokens, returnRequest, orderItem);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                var toEmail = _userService.IsGuest(User) ?
                    billingAddress.Email :
                    User.Email;
                var toName = _userService.IsGuest(User) ?
                    billingAddress.FirstName :
                    _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        #endregion

        #region Forum Notifications

        /// <summary>
        /// Sends a forum subscription message to a User
        /// </summary>
        /// <param name="User">User instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendNewForumTopicMessage(User User, ForumTopic forumTopic, Forum forum, int languageId)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var store = _storeContext.CurrentStore;

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewForumTopicMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddForumTopicTokens(commonTokens, forumTopic);
            _messageTokenProvider.AddForumTokens(commonTokens, forum);
            _messageTokenProvider.AddUserTokens(commonTokens, User);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = User.Email;
                var toName = _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a forum subscription message to a User
        /// </summary>
        /// <param name="User">User instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendNewForumPostMessage(User User, ForumPost forumPost, ForumTopic forumTopic,
            Forum forum, int friendlyForumTopicPageIndex, int languageId)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var store = _storeContext.CurrentStore;

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewForumPostMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddForumPostTokens(commonTokens, forumPost);
            _messageTokenProvider.AddForumTopicTokens(commonTokens, forumTopic, friendlyForumTopicPageIndex, forumPost.Id);
            _messageTokenProvider.AddForumTokens(commonTokens, forum);
            _messageTokenProvider.AddUserTokens(commonTokens, User);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = User.Email;
                var toName = _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a private message notification
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendPrivateMessageNotification(PrivateMessage privateMessage, int languageId)
        {
            if (privateMessage == null)
                throw new ArgumentNullException(nameof(privateMessage));

            var store = _storeService.GetStoreById(privateMessage.StoreId) ?? _storeContext.CurrentStore;

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.PrivateMessageNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddPrivateMessageTokens(commonTokens, privateMessage);
            _messageTokenProvider.AddUserTokens(commonTokens, privateMessage.ToUserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var User = _userService.GetUserById(privateMessage.ToUserId);
                var toEmail = User.Email;
                var toName = _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        #endregion

        #region Misc

        /// <summary>
        /// Sends 'New vendor account submitted' message to a store owner
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendNewVendorAccountApplyStoreOwnerNotification(User User, Vendor vendor, int languageId)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewVendorAccountApplyStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);
            _messageTokenProvider.AddVendorTokens(commonTokens, vendor);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends 'Vendor information changed' message to a store owner
        /// </summary>
        /// <param name="vendor">Vendor</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendVendorInformationChangeNotification(Vendor vendor, int languageId)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.VendorInformationChangeNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddVendorTokens(commonTokens, vendor);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a gift card notification
        /// </summary>
        /// <param name="giftCard">Gift card</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendGiftCardNotification(GiftCard giftCard, int languageId)
        {
            if (giftCard == null)
                throw new ArgumentNullException(nameof(giftCard));

            var order = _orderService.GetOrderByOrderItem(giftCard.PurchasedWithOrderItemId ?? 0);

            var store = order != null ? _storeService.GetStoreById(order.StoreId) ?? _storeContext.CurrentStore : _storeContext.CurrentStore;

            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.GiftCardNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddGiftCardTokens(commonTokens, giftCard);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = giftCard.RecipientEmail;
                var toName = giftCard.RecipientName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a product review notification message to a store owner
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendProductReviewNotificationMessage(ProductReview productReview, int languageId)
        {
            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.ProductReviewStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddProductReviewTokens(commonTokens, productReview);
            _messageTokenProvider.AddUserTokens(commonTokens, productReview.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a product review reply notification message to a User
        /// </summary>
        /// <param name="productReview">Product review</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendProductReviewReplyUserNotificationMessage(ProductReview productReview, int languageId)
        {
            if (productReview == null)
                throw new ArgumentNullException(nameof(productReview));

            var store = _storeService.GetStoreById(productReview.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.ProductReviewReplyUserNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var User = _userService.GetUserById(productReview.UserId);

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddProductReviewTokens(commonTokens, productReview);
            _messageTokenProvider.AddUserTokens(commonTokens, User);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = User.Email;
                var toName = _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendQuantityBelowStoreOwnerNotification(Product product, int languageId)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.QuantityBelowStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var commonTokens = new List<Token>();
            _messageTokenProvider.AddProductTokens(commonTokens, product, languageId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a "quantity below" notification to a store owner
        /// </summary>
        /// <param name="combination">Attribute combination</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendQuantityBelowStoreOwnerNotification(ProductAttributeCombination combination, int languageId)
        {
            if (combination == null)
                throw new ArgumentNullException(nameof(combination));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.QuantityBelowAttributeCombinationStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            var commonTokens = new List<Token>();
            var product = _productService.GetProductById(combination.ProductId);

            _messageTokenProvider.AddProductTokens(commonTokens, product, languageId);
            _messageTokenProvider.AddAttributeCombinationTokens(commonTokens, combination, languageId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a "new VAT submitted" notification to a store owner
        /// </summary>
        /// <param name="User">User</param>
        /// <param name="vatName">Received VAT name</param>
        /// <param name="vatAddress">Received VAT address</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendNewVatSubmittedStoreOwnerNotification(User User,
            string vatName, string vatAddress, int languageId)
        {
            if (User == null)
                throw new ArgumentNullException(nameof(User));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewVatSubmittedStoreOwnerNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);
            commonTokens.Add(new Token("VatValidationResult.Name", vatName));
            commonTokens.Add(new Token("VatValidationResult.Address", vatAddress));

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a blog comment notification message to a store owner
        /// </summary>
        /// <param name="blogComment">Blog comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>List of queued email identifiers</returns>
        public virtual IList<int> SendBlogCommentNotificationMessage(BlogComment blogComment, int languageId)
        {
            if (blogComment == null)
                throw new ArgumentNullException(nameof(blogComment));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.BlogCommentNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddBlogCommentTokens(commonTokens, blogComment);
            _messageTokenProvider.AddUserTokens(commonTokens, blogComment.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a news comment notification message to a store owner
        /// </summary>
        /// <param name="newsComment">News comment</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendNewsCommentNotificationMessage(NewsComment newsComment, int languageId)
        {
            if (newsComment == null)
                throw new ArgumentNullException(nameof(newsComment));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.NewsCommentNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddNewsCommentTokens(commonTokens, newsComment);
            _messageTokenProvider.AddUserTokens(commonTokens, newsComment.UserId);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends a 'Back in stock' notification message to a User
        /// </summary>
        /// <param name="subscription">Subscription</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendBackInStockNotification(BackInStockSubscription subscription, int languageId)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));

            var User = _userService.GetUserById(subscription.UserId);

            if (User == null)
                throw new ArgumentNullException(nameof(User));

            //ensure that User is registered (simple and fast way)
            if (!CommonHelper.IsValidEmail(User.Email))
                return new List<int>();

            var store = _storeService.GetStoreById(subscription.StoreId) ?? _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.BackInStockNotification, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>();
            _messageTokenProvider.AddUserTokens(commonTokens, User);
            _messageTokenProvider.AddBackInStockTokens(commonTokens, subscription);

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = User.Email;
                var toName = _userService.GetUserFullName(User);

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName);
            }).ToList();
        }

        /// <summary>
        /// Sends "contact us" message
        /// </summary>
        /// <param name="languageId">Message language identifier</param>
        /// <param name="senderEmail">Sender email</param>
        /// <param name="senderName">Sender name</param>
        /// <param name="subject">Email subject. Pass null if you want a message template subject to be used.</param>
        /// <param name="body">Email body</param>
        /// <returns>Queued email identifier</returns>
        public virtual IList<int> SendContactUsMessage(int languageId, string senderEmail,
            string senderName, string subject, string body)
        {
            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.ContactUsMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>
            {
                new Token("ContactUs.SenderEmail", senderEmail),
                new Token("ContactUs.SenderName", senderName),
                new Token("ContactUs.Body", body, true)
            };

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                string fromEmail;
                string fromName;
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    fromEmail = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
                }
                else
                {
                    fromEmail = senderEmail;
                    fromName = senderName;
                }

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = emailAccount.Email;
                var toName = emailAccount.DisplayName;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    fromEmail: fromEmail,
                    fromName: fromName,
                    subject: subject,
                    replyToEmailAddress: senderEmail,
                    replyToName: senderName);
            }).ToList();
        }

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
        public virtual IList<int> SendContactVendorMessage(Vendor vendor, int languageId, string senderEmail,
            string senderName, string subject, string body)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            var store = _storeContext.CurrentStore;
            languageId = EnsureLanguageIsActive(languageId, store.Id);

            var messageTemplates = GetActiveMessageTemplates(MessageTemplateSystemNames.ContactVendorMessage, store.Id);
            if (!messageTemplates.Any())
                return new List<int>();

            //tokens
            var commonTokens = new List<Token>
            {
                new Token("ContactUs.SenderEmail", senderEmail),
                new Token("ContactUs.SenderName", senderName),
                new Token("ContactUs.Body", body, true)
            };

            return messageTemplates.Select(messageTemplate =>
            {
                //email account
                var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

                string fromEmail;
                string fromName;
                //required for some SMTP servers
                if (_commonSettings.UseSystemEmailForContactUsForm)
                {
                    fromEmail = emailAccount.Email;
                    fromName = emailAccount.DisplayName;
                    body = $"<strong>From</strong>: {WebUtility.HtmlEncode(senderName)} - {WebUtility.HtmlEncode(senderEmail)}<br /><br />{body}";
                }
                else
                {
                    fromEmail = senderEmail;
                    fromName = senderName;
                }

                var tokens = new List<Token>(commonTokens);
                _messageTokenProvider.AddStoreTokens(tokens, store, emailAccount);

                //event notification
                _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

                var toEmail = vendor.Email;
                var toName = vendor.Name;

                return SendNotification(messageTemplate, emailAccount, languageId, tokens, toEmail, toName,
                    fromEmail: fromEmail,
                    fromName: fromName,
                    subject: subject,
                    replyToEmailAddress: senderEmail,
                    replyToName: senderName);
            }).ToList();
        }

        /// <summary>
        /// Sends a test email
        /// </summary>
        /// <param name="messageTemplateId">Message template identifier</param>
        /// <param name="sendToEmail">Send to email</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendTestEmail(int messageTemplateId, string sendToEmail, List<Token> tokens, int languageId)
        {
            var messageTemplate = _messageTemplateService.GetMessageTemplateById(messageTemplateId);
            if (messageTemplate == null)
                throw new ArgumentException("Template cannot be loaded");

            //email account
            var emailAccount = GetEmailAccountOfMessageTemplate(messageTemplate, languageId);

            //event notification
            _eventPublisher.MessageTokensAdded(messageTemplate, tokens);

            return SendNotification(messageTemplate, emailAccount, languageId, tokens, sendToEmail, null);
        }

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>Queued email identifier</returns>
        public virtual int SendNotification(MessageTemplate messageTemplate,
            EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            //retrieve localized message template data
            var bcc = _localizationService.GetLocalized(messageTemplate, mt => mt.BccEmailAddresses, languageId);
            if (string.IsNullOrEmpty(subject))
                subject = _localizationService.GetLocalized(messageTemplate, mt => mt.Subject, languageId);
            var body = _localizationService.GetLocalized(messageTemplate, mt => mt.Body, languageId);

            //Replace subject and body tokens 
            var subjectReplaced = _tokenizer.Replace(subject, tokens, false);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //limit name length
            toName = CommonHelper.EnsureMaximumLength(toName, 300);

            var email = new QueuedEmail
            {
                Priority = QueuedEmailPriority.High,
                From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : emailAccount.Email,
                FromName = !string.IsNullOrEmpty(fromName) ? fromName : emailAccount.DisplayName,
                To = toEmailAddress,
                ToName = toName,
                ReplyTo = replyToEmailAddress,
                ReplyToName = replyToName,
                CC = string.Empty,
                Bcc = bcc,
                Subject = subjectReplaced,
                Body = bodyReplaced,
                AttachmentFilePath = attachmentFilePath,
                AttachmentFileName = attachmentFileName,
                AttachedDownloadId = messageTemplate.AttachedDownloadId,
                CreatedOnUtc = DateTime.UtcNow,
                EmailAccountId = emailAccount.Id,
                DontSendBeforeDateUtc = !messageTemplate.DelayBeforeSend.HasValue ? null
                    : (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
            };

            _queuedEmailService.InsertQueuedEmail(email);
            return email.Id;
        }

        #endregion

        #endregion
    }
}