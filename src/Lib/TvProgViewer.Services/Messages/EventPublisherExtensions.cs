﻿using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Events;

namespace TvProgViewer.Services.Messages
{
    /// <summary>
    /// Event publisher extensions
    /// </summary>
    public static class EventPublisherExtensions
    {
        /// <summary>
        /// Publishes the newsletter subscribe event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="subscription">The newsletter subscription.</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public static async Task PublishNewsletterSubscribeAsync(this IEventPublisher eventPublisher, NewsLetterSubscription subscription)
        {
            await eventPublisher.PublishAsync(new EmailSubscribedEvent(subscription));
        }

        /// <summary>
        /// Publishes the newsletter unsubscribe event.
        /// </summary>
        /// <param name="eventPublisher">The event publisher.</param>
        /// <param name="subscription">The newsletter subscription.</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public static async Task PublishNewsletterUnsubscribeAsync(this IEventPublisher eventPublisher, NewsLetterSubscription subscription)
        {
            await eventPublisher.PublishAsync(new EmailUnsubscribedEvent(subscription));
        }

        /// <summary>
        /// Entity tokens added
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <typeparam name="U">Type</typeparam>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="entity">Entity</param>
        /// <param name="tokens">Tokens</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public static async Task EntityTokensAddedAsync<T, U>(this IEventPublisher eventPublisher, T entity, System.Collections.Generic.IList<U> tokens) where T : BaseEntity
        {
            await eventPublisher.PublishAsync(new EntityTokensAddedEvent<T, U>(entity, tokens));
        }

        /// <summary>
        /// Message token added
        /// </summary>
        /// <typeparam name="U">Type</typeparam>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="message">Message</param>
        /// <param name="tokens">Tokens</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public static async Task MessageTokensAddedAsync<U>(this IEventPublisher eventPublisher, MessageTemplate message, System.Collections.Generic.IList<U> tokens)
        {
            await eventPublisher.PublishAsync(new MessageTokensAddedEvent<U>(message, tokens));
        }
    }
}