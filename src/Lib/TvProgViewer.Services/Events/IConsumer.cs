using System.Threading.Tasks;

namespace TvProgViewer.Services.Events
{
    /// <summary>
    /// Consumer interface
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public interface IConsumer<T>
    {
        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="eventMessage">Event</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task HandleEventAsync(T eventMessage);
    }
}
