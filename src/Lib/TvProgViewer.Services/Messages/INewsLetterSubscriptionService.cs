﻿using System;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Messages;

namespace TvProgViewer.Services.Messages
{
    /// <summary>
    /// Newsletter subscription service interface
    /// </summary>
    public partial interface INewsLetterSubscriptionService
    {
        /// <summary>
        /// Inserts a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true);

        /// <summary>
        /// Updates a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true);

        /// <summary>
        /// Deletes a newsletter subscription
        /// </summary>
        /// <param name="newsLetterSubscription">NewsLetter subscription</param>
        /// <param name="publishSubscriptionEvents">if set to <c>true</c> [publish subscription events].</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteNewsLetterSubscriptionAsync(NewsLetterSubscription newsLetterSubscription, bool publishSubscriptionEvents = true);

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription identifier
        /// </summary>
        /// <param name="newsLetterSubscriptionId">The newsletter subscription identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the newsLetter subscription
        /// </returns>
        Task<NewsLetterSubscription> GetNewsLetterSubscriptionByIdAsync(int newsLetterSubscriptionId);

        /// <summary>
        /// Gets a newsletter subscription by newsletter subscription GUID
        /// </summary>
        /// <param name="newsLetterSubscriptionGuid">The newsletter subscription GUID</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the newsLetter subscription
        /// </returns>
        Task<NewsLetterSubscription> GetNewsLetterSubscriptionByGuidAsync(Guid newsLetterSubscriptionGuid);

        /// <summary>
        /// Gets a newsletter subscription by email and store ID
        /// </summary>
        /// <param name="email">The newsletter subscription email</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the newsLetter subscription
        /// </returns>
        Task<NewsLetterSubscription> GetNewsLetterSubscriptionByEmailAndStoreIdAsync(string email, int storeId);

        /// <summary>
        /// Gets the newsletter subscription list
        /// </summary>
        /// <param name="email">Email to search or string. Empty to load all records.</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="storeId">Store identifier. 0 to load all records.</param>
        /// <param name="isActive">Value indicating whether subscriber record should be active or not; null to load all records</param>
        /// <param name="userRoleId">User role identifier. Used to filter subscribers by user role. 0 to load all records.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the newsLetterSubscription entities
        /// </returns>
        Task<IPagedList<NewsLetterSubscription>> GetAllNewsLetterSubscriptionsAsync(string email = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int storeId = 0, bool? isActive = null, int userRoleId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
