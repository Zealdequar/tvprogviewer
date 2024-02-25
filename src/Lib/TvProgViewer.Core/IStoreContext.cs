using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Stores;

namespace TvProgViewer.Core
{
    /// <summary>
    /// Store context
    /// </summary>
    public interface IStoreContext
    {
        /// <summary>
        /// Gets the current store
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<Store> GetCurrentStoreAsync();

        /// <summary>
        /// Gets the current store
        /// </summary>
        Store GetCurrentStore();

        /// <summary>
        /// Gets active store scope configuration
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<int> GetActiveStoreScopeConfigurationAsync();
    }
}
