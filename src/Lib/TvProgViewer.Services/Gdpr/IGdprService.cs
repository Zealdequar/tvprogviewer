﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Gdpr;

namespace TvProgViewer.Services.Gdpr
{
    /// <summary>
    /// Represents the GDPR service interface
    /// </summary>
    public partial interface IGdprService
    {
        #region GDPR consent

        /// <summary>
        /// Get a GDPR consent
        /// </summary>
        /// <param name="gdprConsentId">The GDPR consent identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR consent
        /// </returns>
        Task<GdprConsent> GetConsentByIdAsync(int gdprConsentId);

        /// <summary>
        /// Get all GDPR consents
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gDPR consent
        /// </returns>
        Task<IList<GdprConsent>> GetAllConsentsAsync();

        /// <summary>
        /// Insert a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertConsentAsync(GdprConsent gdprConsent);

        /// <summary>
        /// Update the GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateConsentAsync(GdprConsent gdprConsent);

        /// <summary>
        /// Delete a GDPR consent
        /// </summary>
        /// <param name="gdprConsent">GDPR consent</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteConsentAsync(GdprConsent gdprConsent);

        /// <summary>
        /// Gets the latest selected value (a consent is accepted or not by a user)
        /// </summary>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result; null if previous a user hasn't been asked
        /// </returns>
        Task<bool?> IsConsentAcceptedAsync(int consentId, int userId);

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
        Task<IPagedList<GdprLog>> GetAllLogAsync(int userId = 0, int consentId = 0,
            string userInfo = "", GdprRequestType? requestType = null,
            int pageIndex = 0, int pageSize = int.MaxValue);
        
        /// <summary>
        /// Insert a GDPR log
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="consentId">Consent identifier</param>
        /// <param name="requestType">Request type</param>
        /// <param name="requestDetails">Request details</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertLogAsync(User user, int consentId, GdprRequestType requestType, string requestDetails);

        #endregion

        #region User

        /// <summary>
        /// Permanent delete of user
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PermanentDeleteUserAsync(User user);

        #endregion
    }
}
