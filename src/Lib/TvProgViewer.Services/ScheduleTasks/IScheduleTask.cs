using System.Threading.Tasks;

namespace TvProgViewer.Services.ScheduleTasks
{
    /// <summary>
    /// Interface that should be implemented by each task
    /// </summary>
    public partial interface IScheduleTask
    {
        /// <summary>
        /// Executes a task
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ExecuteAsync();
    }
}