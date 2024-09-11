using System;
using System.Threading.Tasks;

namespace TvProgViewer.Core.Caching
{
    /// <summary>
    /// Represents a manager for caching between HTTP requests (long term caching)
    /// </summary>
    public interface IStaticCacheManager : IDisposable
    {
        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cached value associated with the specified key
        /// </returns>
        Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire);

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type of cached item</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cached value associated with the specified key
        /// </returns>
        Task<T> GetAsync<T>(CacheKey key, Func<T> acquire);

        /// <summary>
        /// Получить закэшированный элемент. Если он ещё не в кэше, тогда вернуть значение по умолчанию
        /// </summary>
        /// <typeparam name="T">Тип кэшируемого элемента</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <param name="acquire">Функция для загрузки элемента если его ещё нет в кэше</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// Резульат задачи содержит кэшированное значени, ассоциированное с указанным ключом или значение по умолчанию, 
        /// если оно не было найдено
        /// </returns>
        Task<T> GetAsync<T>(CacheKey key, T defaultValue = default);

        /// <summary>
        /// Remove the value with the specified key from the cache
        /// </summary>
        /// <param name="cacheKey">Cache key</param>
        /// <param name="cacheKeyParameters">Parameters to create cache key</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters);

        /// <summary>
        /// Добавление указанного ключа и объекта кэша
        /// </summary>
        /// <param name="key">Ключ закэшированного элемента</param>
        /// <param name="data">Значение для кэширования</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SetAsync<T>(CacheKey key, T data);

        /// <summary>
        /// Remove items by cache key prefix
        /// </summary>
        /// <param name="prefix">Cache key prefix</param>
        /// <param name="prefixParameters">Parameters to create cache key prefix</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ClearAsync();

        #region Cache key

        /// <summary>
        /// Create a copy of cache key and fills it by passed parameters
        /// </summary>
        /// <param name="cacheKey">Initial cache key</param>
        /// <param name="cacheKeyParameters">Parameters to create cache key</param>
        /// <returns>Cache key</returns>
        CacheKey PrepareKey(CacheKey cacheKey, params object[] cacheKeyParameters);

        /// <summary>
        /// Create a copy of cache key using the default cache time and fills it by passed parameters
        /// </summary>
        /// <param name="cacheKey">Initial cache key</param>
        /// <param name="cacheKeyParameters">Parameters to create cache key</param>
        /// <returns>Cache key</returns>
        CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] cacheKeyParameters);

        /// <summary>
        /// Create a copy of cache key using the short cache time and fills it by passed parameters
        /// </summary>
        /// <param name="cacheKey">Initial cache key</param>
        /// <param name="cacheKeyParameters">Parameters to create cache key</param>
        /// <returns>Cache key</returns>
        CacheKey PrepareKeyForShortTermCache(CacheKey cacheKey, params object[] cacheKeyParameters);

        #endregion
    }
}