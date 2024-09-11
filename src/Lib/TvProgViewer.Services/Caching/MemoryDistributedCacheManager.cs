using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Configuration;

namespace TvProgViewer.Services.Caching
{
    public class MemoryDistributedCacheManager : DistributedCacheManager
    {
        #region Конструктор

        public MemoryDistributedCacheManager(AppSettings appSettings, 
            IDistributedCache distributedCache,
            CacheKeyManager cacheKeyManager)
            : base(appSettings, distributedCache, cacheKeyManager)
        {
        }

        #endregion

        public override async Task ClearAsync()
        {
            foreach (var key in _localKeyManager.Keys)
                await RemoveAsync(key, false);
            ClearInstanceData();
        }

        public override async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            var prefix_ = PrepareKeyPrefix(prefix, prefixParameters);
            foreach (var key in RemoveByPrefixInstanceData(prefix_))
                await RemoveAsync(key, false);
        }
    }
}