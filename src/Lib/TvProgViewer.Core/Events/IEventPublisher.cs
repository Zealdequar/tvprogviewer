﻿using System.Threading.Tasks;

 namespace TvProgViewer.Core.Events
 {
     /// <summary>
     /// Represents an event publisher
     /// </summary>
     public partial interface IEventPublisher
     {
        /// <summary>
        /// Publish event to consumers
        /// </summary>
        /// <typeparam name="TEvent">Type of event</typeparam>
        /// <param name="event">Event object</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task PublishAsync<TEvent>(TEvent @event);

         /// <summary>
         /// Publish event to consumers
         /// </summary>
         /// <typeparam name="TEvent">Type of event</typeparam>
         /// <param name="event">Event object</param>
         void Publish<TEvent>(TEvent @event);
    }
 }