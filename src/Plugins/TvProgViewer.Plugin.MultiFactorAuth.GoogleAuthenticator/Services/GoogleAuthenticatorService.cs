﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Authenticator;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Data;
using TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Domains;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Services
{
    /// <summary>
    /// Represents Google Authenticator service
    /// </summary>
    public class GoogleAuthenticatorService
    {
        #region Fields

        private readonly IRepository<GoogleAuthenticatorRecord> _repository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IWorkContext _workContext;
        private readonly GoogleAuthenticatorSettings _googleAuthenticatorSettings;
        private TwoFactorAuthenticator _twoFactorAuthenticator;
        

        #endregion

        #region Ctr

        public GoogleAuthenticatorService(
            IRepository<GoogleAuthenticatorRecord> repository,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext,
            GoogleAuthenticatorSettings googleAuthenticatorSettings)
        {
            _repository = repository;
            _staticCacheManager = staticCacheManager;
            _workContext = workContext;
            _googleAuthenticatorSettings = googleAuthenticatorSettings;
        }
        #endregion

        #region Properties

        private TwoFactorAuthenticator TwoFactorAuthenticator
        {
            get
            {
                _twoFactorAuthenticator = new TwoFactorAuthenticator();
                return _twoFactorAuthenticator;
            }
        }

        #endregion

        #region Utilites

        /// <summary>
        /// Insert the configuration
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task InsertConfigurationAsync(GoogleAuthenticatorRecord configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            await _repository.InsertAsync(configuration);
            await _staticCacheManager.RemoveByPrefixAsync(GoogleAuthenticatorDefaults.PrefixCacheKey);
        }

        /// <summary>
        /// Update the configuration
        /// </summary>
        /// <param name="configuration">Configuration</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task UpdateConfigurationAsync(GoogleAuthenticatorRecord configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            await _repository.UpdateAsync(configuration);
            await _staticCacheManager.RemoveByPrefixAsync(GoogleAuthenticatorDefaults.PrefixCacheKey);
        }

        /// <summary>
        /// Delete the configuration
        /// </summary>
        /// <param name="configuration">Configuration</param>
        internal async Task DeleteConfigurationAsync(GoogleAuthenticatorRecord configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            await _repository.DeleteAsync(configuration);
            await _staticCacheManager.RemoveByPrefixAsync(GoogleAuthenticatorDefaults.PrefixCacheKey);
        }

        /// <summary>
        /// Get a configuration by the identifier
        /// </summary>
        /// <param name="configurationId">Configuration identifier</param>
        /// <returns>Configuration</returns>
        internal async Task<GoogleAuthenticatorRecord> GetConfigurationByIdAsync(int configurationId)
        {
            if (configurationId == 0)
                return null;

            return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(GoogleAuthenticatorDefaults.ConfigurationCacheKey, configurationId), async () =>
                await _repository.GetByIdAsync(configurationId));
        }

        internal GoogleAuthenticatorRecord GetConfigurationByUserEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var query = _repository.Table;
            return query.FirstOrDefault(record => record.User == email);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get configurations
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of configurations
        /// </returns>
        public async Task<IPagedList<GoogleAuthenticatorRecord>> GetPagedConfigurationsAsync(string email = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _repository.Table;
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.User.Contains(email));
            query = query.OrderBy(configuration => configuration.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Check if the user is registered  
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public bool IsRegisteredUser(string userEmail)
        {
            return GetConfigurationByUserEmail(userEmail) != null;
        }

        /// <summary>
        /// Add configuration of GoogleAuthenticator
        /// </summary>
        /// <param name="userEmail">User email</param>
        /// <param name="key">Secret key</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddGoogleAuthenticatorAccountAsync(string userEmail, string key)
        {
            var account = new GoogleAuthenticatorRecord
            {
                User = userEmail,
                SecretKey = key,
            };

            await InsertConfigurationAsync(account);
        }

        /// <summary>
        /// Update configuration of GoogleAuthenticator
        /// </summary>
        /// <param name="userEmail">User email</param>
        /// <param name="key">Secret key</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task UpdateGoogleAuthenticatorAccountAsync(string userEmail, string key)
        {
            var account = GetConfigurationByUserEmail(userEmail);
            if (account != null)
            {
                account.SecretKey = key;
                await UpdateConfigurationAsync(account);
            }
        }

        /// <summary>
        /// Generate a setup code for a Google Authenticator user to scan
        /// </summary>
        /// <param name="secretkey">Secret key</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the 
        /// </returns>
        public async Task<SetupCode> GenerateSetupCode(string secretkey)
        {
            var user = await _workContext.GetCurrentUserAsync();

            return TwoFactorAuthenticator.GenerateSetupCode(
                _googleAuthenticatorSettings.BusinessPrefix, 
                user.Email, 
                secretkey, false, _googleAuthenticatorSettings.QRPixelsPerModule);
        }

        /// <summary>
        /// Validate token auth
        /// </summary>
        /// <param name="secretkey">Secret key</param>
        /// <param name="token">Token</param>
        /// <returns></returns>
        public bool ValidateTwoFactorToken(string secretkey, string token)
        {
            return TwoFactorAuthenticator.ValidateTwoFactorPIN(secretkey, token);
        }

        #endregion
    }
}