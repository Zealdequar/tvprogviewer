﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Data;
using TvProgViewer.Services.Users;

namespace TvProgViewer.Services.Messages
{
    /// <summary>
    /// Campaign service
    /// </summary>
    public partial class CampaignService : ICampaignService
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IRepository<Campaign> _campaignRepository;
        private readonly IStoreContext _storeContext;
        private readonly ITokenizer _tokenizer;

        #endregion

        #region Ctor

        public CampaignService(IUserService userService,
            IEmailSender emailSender,
            IMessageTokenProvider messageTokenProvider,
            IQueuedEmailService queuedEmailService,
            IRepository<Campaign> campaignRepository,
            IStoreContext storeContext,
            ITokenizer tokenizer)
        {
            _userService = userService;
            _emailSender = emailSender;
            _messageTokenProvider = messageTokenProvider;
            _queuedEmailService = queuedEmailService;
            _campaignRepository = campaignRepository;
            _storeContext = storeContext;
            _tokenizer = tokenizer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>        
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertCampaignAsync(Campaign campaign)
        {
            await _campaignRepository.InsertAsync(campaign);
        }

        /// <summary>
        /// Updates a campaign
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateCampaignAsync(Campaign campaign)
        {
            await _campaignRepository.UpdateAsync(campaign);
        }

        /// <summary>
        /// Deleted a queued email
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteCampaignAsync(Campaign campaign)
        {
            await _campaignRepository.DeleteAsync(campaign);
        }

        /// <summary>
        /// Gets a campaign by identifier
        /// </summary>
        /// <param name="campaignId">Campaign identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the campaign
        /// </returns>
        public virtual async Task<Campaign> GetCampaignByIdAsync(int campaignId)
        {
            return await _campaignRepository.GetByIdAsync(campaignId, cache => default);
        }

        /// <summary>
        /// Gets all campaigns
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the campaigns
        /// </returns>
        public virtual async Task<IList<Campaign>> GetAllCampaignsAsync(int storeId = 0)
        {
            var campaigns = await _campaignRepository.GetAllAsync(query =>
            {
                if (storeId > 0) 
                    query = query.Where(c => c.StoreId == storeId);

                query = query.OrderBy(c => c.CreatedOnUtc);

                return query;
            });

            return campaigns;
        }

        /// <summary>
        /// Sends a campaign to specified emails
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the otal emails sent
        /// </returns>
        public virtual async Task<int> SendCampaignAsync(Campaign campaign, EmailAccount emailAccount,
            IEnumerable<NewsLetterSubscription> subscriptions)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            var totalEmailsSent = 0;

            foreach (var subscription in subscriptions)
            {
                var user = await _userService.GetUserByEmailAsync(subscription.Email);
                //ignore deleted or inactive users when sending newsletter campaigns
                if (user != null && (!user.Active || user.Deleted))
                    continue;

                var tokens = new List<Token>();
                await _messageTokenProvider.AddStoreTokensAsync(tokens, await _storeContext.GetCurrentStoreAsync(), emailAccount);
                await _messageTokenProvider.AddNewsLetterSubscriptionTokensAsync(tokens, subscription);
                if (user != null)
                    await _messageTokenProvider.AddUserTokensAsync(tokens, user);

                var subject = _tokenizer.Replace(campaign.Subject, tokens, false);
                var body = _tokenizer.Replace(campaign.Body, tokens, true);

                var email = new QueuedEmail
                {
                    Priority = QueuedEmailPriority.Low,
                    From = emailAccount.Email,
                    FromName = emailAccount.DisplayName,
                    To = subscription.Email,
                    Subject = subject,
                    Body = body,
                    CreatedOnUtc = DateTime.UtcNow,
                    EmailAccountId = emailAccount.Id,
                    DontSendBeforeDateUtc = campaign.DontSendBeforeDateUtc
                };
                await _queuedEmailService.InsertQueuedEmailAsync(email);
                totalEmailsSent++;
            }

            return totalEmailsSent;
        }

        /// <summary>
        /// Sends a campaign to specified email
        /// </summary>
        /// <param name="campaign">Campaign</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="email">Email</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task SendCampaignAsync(Campaign campaign, EmailAccount emailAccount, string email)
        {
            if (campaign == null)
                throw new ArgumentNullException(nameof(campaign));

            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            var tokens = new List<Token>();
            await _messageTokenProvider.AddStoreTokensAsync(tokens, await _storeContext.GetCurrentStoreAsync(), emailAccount);
            var user = await _userService.GetUserByEmailAsync(email);
            if (user != null)
                await _messageTokenProvider.AddUserTokensAsync(tokens, user);

            var subject = _tokenizer.Replace(campaign.Subject, tokens, false);
            var body = _tokenizer.Replace(campaign.Body, tokens, true);

            await _emailSender.SendEmailAsync(emailAccount, subject, body, emailAccount.Email, emailAccount.DisplayName, email, null);
        }

        #endregion
    }
}