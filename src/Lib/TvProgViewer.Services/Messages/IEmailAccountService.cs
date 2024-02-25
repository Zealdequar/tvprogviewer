﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Messages;

namespace TvProgViewer.Services.Messages
{
    /// <summary>
    /// Email account service
    /// </summary>
    public partial interface IEmailAccountService
    {
        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Deletes an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteEmailAccountAsync(EmailAccount emailAccount);

        /// <summary>
        /// Gets an email account by identifier
        /// </summary>
        /// <param name="emailAccountId">The email account identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the email account
        /// </returns>
        Task<EmailAccount> GetEmailAccountByIdAsync(int emailAccountId);

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the email accounts list
        /// </returns>
        Task<IList<EmailAccount>> GetAllEmailAccountsAsync();
    }
}
