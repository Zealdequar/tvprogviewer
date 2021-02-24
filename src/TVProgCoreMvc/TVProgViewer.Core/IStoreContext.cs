using System.Threading.Tasks;
using TVProgViewer.Core.Domain.Stores;

namespace TVProgViewer.Core
{
    /// <summary>
    /// Store context
    /// </summary>
    public interface IStoreContext
    {
        /// <summary>
        /// Gets the current store
        /// </summary>
        Task<Store> GetCurrentStoreAsync();

        /// <summary>
        /// Gets active store scope configuration
        /// </summary>
        Task<int> GetActiveStoreScopeConfigurationAsync();
    }
}
