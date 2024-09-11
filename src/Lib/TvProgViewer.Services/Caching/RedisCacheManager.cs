using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Configuration;
using StackExchange.Redis;

namespace TvProgViewer.Services.Caching
{
    /// <summary>
    /// Represents a redis distributed cache 
    /// </summary>
    public class RedisCacheManager : DistributedCacheManager
    {
        #region Поля

        private RedisConnectionWrapper _connectionWrapper;

        #endregion

        #region Конструктор

        public RedisCacheManager(AppSettings appSettings,
            IDistributedCache distributedCache,
            RedisConnectionWrapper connectionWrapper,
            CacheKeyManager cacheKeyManager)
            : base(appSettings, distributedCache, cacheKeyManager)
        {
            _connectionWrapper = connectionWrapper;
        }

        #endregion

        #region Утилиты

        /// <summary>
        /// Получение списка префиксов ключей кэша
        /// </summary>
        /// <param name="endPoint">Сетевой адрес</param>
        /// <param name="prefix">Строковый шаблон ключа</param>
        /// <returns>Список ключей кэша</returns>
        protected virtual async Task<IEnumerable<RedisKey>> GetKeysAsync(EndPoint endPoint, string prefix = null)
        {
            return await (await _connectionWrapper.GetServerAsync(endPoint))
                .KeysAsync((await _connectionWrapper.GetDatabaseAsync()).Database, string.IsNullOrEmpty(prefix) ? null : $"{prefix}*")
                .ToListAsync();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Удаление элементов префикса ключа кэша
        /// </summary>
        /// <param name="prefix">Префикс ключа кэша</param>
        /// <param name="prefixParameters">Параметры для создания префкса ключа кэша</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public override async Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            prefix = PrepareKeyPrefix(prefix, prefixParameters);
            var db = await _connectionWrapper.GetDatabaseAsync();

            foreach (var endPoint in await _connectionWrapper.GetEndPointsAsync())
            {
                var keys = await GetKeysAsync(endPoint, prefix);
                db.KeyDelete(keys.ToArray());
            }

            RemoveByPrefixInstanceData(prefix);
        }

        /// <summary>
        /// Очистить все кэшированные данные
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public override async Task ClearAsync()
        {
            await _connectionWrapper.FlushDatabaseAsync();

            ClearInstanceData();
        }

        #endregion
    }
}