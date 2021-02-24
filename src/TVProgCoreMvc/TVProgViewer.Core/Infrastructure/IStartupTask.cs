using System.Threading.Tasks;

namespace TVProgViewer.Core.Infrastructure
{
    /// <summary>
    /// Интерфейс, который должен реализовывать задания, запускаемые вначале
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Реализует задания
        /// </summary>
        Task ExecuteAsync();

        /// <summary>
        /// Получает порядок запуска заданий
        /// </summary>
        int Order { get; }
    }
}
