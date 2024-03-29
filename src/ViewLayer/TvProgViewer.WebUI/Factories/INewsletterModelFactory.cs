﻿using System.Threading.Tasks;
using TvProgViewer.WebUI.Models.Newsletter;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the newsletter model factory
    /// </summary>
    public partial interface INewsletterModelFactory
    {
        /// <summary>
        /// Prepare the newsletter box model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the newsletter box model
        /// </returns>
        Task<NewsletterBoxModel> PrepareNewsletterBoxModelAsync();

        /// <summary>
        /// Prepare the subscription activation model
        /// </summary>
        /// <param name="active">Whether the subscription has been activated</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the subscription activation model
        /// </returns>
        Task<SubscriptionActivationModel> PrepareSubscriptionActivationModelAsync(bool active);
    }
}
