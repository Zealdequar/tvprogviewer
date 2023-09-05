using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Caching;

namespace TvProgViewer.Plugin.Tax.Avalara.Services.Caching
{
    /// <summary>
    /// Represents a user cache event consumer
    /// </summary>
    public class UserCacheEventConsumer : CacheEventConsumer<User>
    {
        #region Methods

        /// <summary>
        /// Clear cache data
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task ClearCacheAsync(User entity)
        {
            await RemoveByPrefixAsync(AvalaraTaxDefaults.TaxRateCacheKeyByUserPrefix, entity);
        }

        #endregion
    }
}