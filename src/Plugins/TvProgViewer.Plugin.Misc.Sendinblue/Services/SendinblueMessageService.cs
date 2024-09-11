using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Affiliates;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Stores;

namespace TvProgViewer.Plugin.Misc.Sendinblue.Services
{
    /// <summary>
    /// Represents overridden workflow message service
    /// </summary>
    public class SendinblueMessageService : WorkflowMessageService
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ISettingService _settingService;
        private readonly ITokenizer _tokenizer;
        private readonly SendinblueManager _sendinblueEmailManager;

        #endregion

        #region Ctor

        public SendinblueMessageService(CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            IAddressService addressService,
            IAffiliateService affiliateService,
            IUserService userService,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            IOrderService orderService,
            ITvChannelService tvChannelService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IQueuedEmailService queuedEmailService,
            ITokenizer tokenizer,
            MessagesSettings messagesSettings,
            SendinblueManager sendinblueEmailManager)
            : base(commonSettings,
                emailAccountSettings,
                addressService,
                affiliateService,
                userService,
                emailAccountService,
                eventPublisher,
                languageService,
                localizationService,
                messageTemplateService,
                messageTokenProvider,
                orderService,
                tvChannelService,
                queuedEmailService,
                storeContext,
                storeService,
                tokenizer,
                messagesSettings)
        {
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _queuedEmailService = queuedEmailService;
            _settingService = settingService;
            _tokenizer = tokenizer;
            _sendinblueEmailManager = sendinblueEmailManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Send SMS notification by message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="tokens">Tokens</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task SendSmsNotificationAsync(MessageTemplate messageTemplate, IEnumerable<Token> tokens)
        {
            //get plugin settings
            var storeId = (int?)tokens.FirstOrDefault(token => token.Key == "Store.Id")?.Value;
            var sendinblueSettings = await _settingService.LoadSettingAsync<SendinblueSettings>(storeId ?? 0);

            //ensure SMS notifications enabled
            if (!sendinblueSettings.UseSmsNotifications)
                return;

            //whether to send SMS by the passed message template
            var sendSmsForThisMessageTemplate = await _genericAttributeService
                .GetAttributeAsync<bool>(messageTemplate, SendinblueDefaults.UseSmsAttribute);
            if (!sendSmsForThisMessageTemplate)
                return;

            //get text with replaced tokens
            var text = await _genericAttributeService.GetAttributeAsync<string>(messageTemplate, SendinblueDefaults.SmsTextAttribute);
            if (!string.IsNullOrEmpty(text))
                text = _tokenizer.Replace(text, tokens, false);

            //get phone number send to
            var phoneNumberTo = string.Empty;
            var phoneType = await _genericAttributeService.GetAttributeAsync<int>(messageTemplate, SendinblueDefaults.SmartPhoneTypeAttribute);
            switch (phoneType)
            {
                case 0:
                    //merchant phone
                    phoneNumberTo = sendinblueSettings.StoreOwnerPhoneNumber;
                    break;
                case 1:
                    //user phone
                    phoneNumberTo = tokens.FirstOrDefault(token => token.Key == "User.PhoneNumber")?.Value?.ToString();
                    break;
                case 2:
                    //order billing address phone
                    phoneNumberTo = tokens.FirstOrDefault(token => token.Key == "Order.BillingPhoneNumber")?.Value?.ToString();
                    break;
            }

            //try to send SMS
            await _sendinblueEmailManager.SendSMSAsync(phoneNumberTo, sendinblueSettings.SmsSenderName, text);
        }

        /// <summary>
        /// Send email notification by message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        private async Task<int?> SendEmailNotificationAsync(MessageTemplate messageTemplate, EmailAccount emailAccount, IEnumerable<Token> tokens,
            string toEmailAddress, string toName,
            string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null,
            string fromEmail = null, string fromName = null, string subject = null)
        {
            //get plugin settings
            var storeId = (int?)tokens.FirstOrDefault(token => token.Key == "Store.Id")?.Value;
            var sendinblueSettings = await _settingService.LoadSettingAsync<SendinblueSettings>(storeId ?? 0);

            //ensure email notifications enabled
            if (!sendinblueSettings.UseSmtp)
                return null;

            //whether to send email by the passed message template
            var templateId = await _genericAttributeService.GetAttributeAsync<int?>(messageTemplate, SendinblueDefaults.TemplateIdAttribute);
            var sendEmailForThisMessageTemplate = templateId.HasValue;
            if (!sendEmailForThisMessageTemplate)
                return null;

            //get the specified email account from settings
            emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(sendinblueSettings.EmailAccountId) ?? emailAccount;

            //get an email from the template
            var email = await _sendinblueEmailManager.GetQueuedEmailFromTemplateAsync(templateId.Value)
                ?? throw new TvProgException($"There is no template with id {templateId}");

            //replace body and subject tokens
            if (string.IsNullOrEmpty(subject))
                subject = email.Subject;
            if (!string.IsNullOrEmpty(subject))
                email.Subject = _tokenizer.Replace(subject, tokens, false);
            if (!string.IsNullOrEmpty(email.Body))
                email.Body = _tokenizer.Replace(email.Body, tokens, true);

            //set email parameters
            email.Priority = QueuedEmailPriority.High;
            email.From = !string.IsNullOrEmpty(fromEmail) ? fromEmail : email.From;
            email.FromName = !string.IsNullOrEmpty(fromName) ? fromName : email.FromName;
            email.To = toEmailAddress;
            email.ToName = CommonHelper.EnsureMaximumLength(toName, 300);
            email.ReplyTo = replyToEmailAddress;
            email.ReplyToName = replyToName;
            email.CC = string.Empty;
            email.Bcc = messageTemplate.BccEmailAddresses;
            email.AttachmentFilePath = attachmentFilePath;
            email.AttachmentFileName = attachmentFileName;
            email.AttachedDownloadId = messageTemplate.AttachedDownloadId;
            email.CreatedOnUtc = DateTime.UtcNow;
            email.EmailAccountId = emailAccount.Id;
            email.DontSendBeforeDateUtc = messageTemplate.DelayBeforeSend.HasValue
                ? (DateTime?)(DateTime.UtcNow + TimeSpan.FromHours(messageTemplate.DelayPeriod.ToHours(messageTemplate.DelayBeforeSend.Value)))
                : null;

            //queue email
            await _queuedEmailService.InsertQueuedEmailAsync(email);
            return email.Id;
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
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public override async Task<int> SendNotificationAsync(MessageTemplate messageTemplate, EmailAccount emailAccount, int languageId, IList<Token> tokens,
            string toEmailAddress, string toName, string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null, string fromEmail = null, string fromName = null,
            string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            //try to send SMS notification
            await SendSmsNotificationAsync(messageTemplate, tokens);

            //try to send email notification
            var emailId = await SendEmailNotificationAsync(messageTemplate, emailAccount, tokens,
                toEmailAddress, toName, attachmentFilePath, attachmentFileName,
                replyToEmailAddress, replyToName, fromEmail, fromName, subject);
            if (emailId.HasValue)
                return emailId.Value;

            //send base notification
            return await base.SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens,
                toEmailAddress, toName, attachmentFilePath, attachmentFileName,
                replyToEmailAddress, replyToName, fromEmail, fromName, subject);
        }

        #endregion
    }
}