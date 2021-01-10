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
        void Execute();

        /// <summary>
        /// Получает порядок запуска заданий
        /// </summary>
        int Order { get; }
    }
}
