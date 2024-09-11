using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Infrastructure;

namespace TvProgViewer.Core.Caching
{
    /// <summary>
    /// Представляет собой диспетчер кэша памяти 
    /// </summary>
    public partial class MemoryCacheManager : CacheKeyService, IStaticCacheManager
    {
        #region Поля

        // Флаг: Был ли уже вызван Dispose?
        private bool _disposed;

        private readonly IMemoryCache _memoryCache;
        private readonly CacheKeyManager _keyManager;
        private static CancellationTokenSource _clearToken = new();

        #endregion

        #region Конструктор

        public MemoryCacheManager(AppSettings appSettings, IMemoryCache memoryCache, CacheKeyManager cacheKeyManager) 
            : base(appSettings)
        {
            _memoryCache = memoryCache;
            _keyManager = cacheKeyManager;
        }

        #endregion

        #region Утилиты

        /// <summary>
        /// Подготавливает параметры кэширования для переданного ключа  
        /// </summary>
        /// <param name="key">Ключ кэша</param>
        /// <returns>Опции кэширования</returns>
        private MemoryCacheEntryOptions PrepareEntryOptions(CacheKey key)
        {
            // Установить время истечения срока действия переданного ключа:
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
            };

            // Добавить токен для очистки записей кэша:
            options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));
            options.RegisterPostEvictionCallback(OnEviction);
            _keyManager.AddKey(key.Key);

            return options;
        }

        private void OnEviction(object key, object value, EvictionReason reason, object state)
        {
            switch (reason)
            {
                // Мы убираем за собой в другом месте
                case EvictionReason.Removed:
                case EvictionReason.Replaced:
                case EvictionReason.TokenExpired:
                    break;
                // Если запись была удалена самим кэшем, удалить ключ:
                default:
                    _keyManager.RemoveKey(key as string);
                    break;
            }
        }

        #endregion

        #region Методы

        /// <summary>
        /// Удаляет значение с указанным ключом из кэша
        /// </summary>
        /// <param name="cacheKey">Ключ кэша</param>
        /// <param name="cacheKeyParameters">Параметры для создания ключа кэша</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            var key = PrepareKey(cacheKey, cacheKeyParameters).Key;
            _memoryCache.Remove(key);
            _keyManager.RemoveKey(key);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Получение кэшированного элемента. Если он ещё не в кэше, тогда загрузить его в кэш
        /// </summary>
        /// <typeparam name="T">Тип кэшированного элемента</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <param name="acquire">Функция для загрузки элемента если его ещё нет в кэше</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// Результат задачи содержит кэшированное значение, связанное с указанным ключом
        /// </returns>
        public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            if ((key?.CacheTime ?? 0) <= 0)
                return await acquire();

            return await _memoryCache.GetOrCreate(
                key.Key,
                entry =>
                {
                    entry.SetOptions(PrepareEntryOptions(key));
                    return new Lazy<Task<T>>(acquire, true);
                }).Value;
        }

        public async Task<T> GetAsync<T>(CacheKey key, T defaultValue = default)
        {
            var value = _memoryCache.Get<Lazy<Task<T>>>(key.Key)?.Value;
            return value != null ? await value : defaultValue;
        }

        /// <summary>
        /// Получение кэшированного элемента. Если он ещё не в кэше, тогда загрузить его в кэш
        /// </summary>
        /// <typeparam name="T">Тип кэшированного элемента</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <param name="acquire">Функция для загрузки элемента если его ещё нет в кэше</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// Результат задачи содержит кэшированное значение, связанное с указанным ключом
        /// </returns>
        public async Task<T> GetAsync<T>(CacheKey key, Func<T> acquire)
        {
            return await GetAsync(key, () => Task.FromResult(acquire()));
        }

        /// <summary>
        /// Добавляет указанный ключ и объект в кэш
        /// </summary>
        /// <param name="key">Ключ кэшируемого элемента</param>
        /// <param name="data">Значение для кэширования</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public Task SetAsync<T>(CacheKey key, T data)
        {
            if (data != null && (key?.CacheTime ?? 0) > 0)
            {
                _memoryCache.Set(
                    key.Key,
                    new Lazy<Task<T>>(() => Task.FromResult(data), true),
                    PrepareEntryOptions(key));
            }
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Удаление элементов по префиксу ключа кэша
        /// </summary>
        /// <param name="prefix">Префикс ключа кэша</param>
        /// <param name="prefixParameters">Параметры для создания префикса ключа кэша</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters)
        {
            foreach (var key in _keyManager.RemoveByPrefix(PrepareKeyPrefix(prefix, prefixParameters)))
                _memoryCache.Remove(key);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Очистить все данные кэша
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public Task ClearAsync()
        {
            _clearToken.Cancel();
            _clearToken.Dispose();
            _clearToken = new CancellationTokenSource();
            _keyManager.Clear();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Безопасная реализация паттерна Dispose.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _clearToken.Dispose();
                // Не нужно удалять MemoryCache по мере того, как он вводится
            }

            _disposed = true;
        }

        #endregion
    }
}