using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Infrastructure;

namespace TvProgViewer.Core.Caching
{
    /// <summary>
    /// Представляет распределённый кэш
    /// </summary>
    public abstract class DistributedCacheManager : CacheKeyService, IStaticCacheManager
    {
        #region Поля

        /// <summary>
        /// Содержит ключи, известные этому экземпляру TvProgViewer
        /// </summary>
        protected readonly CacheKeyManager _localKeyManager;
        protected readonly IDistributedCache _distributedCache;
        private readonly ConcurrentTrie<object> _perRequestCache = new();

        /// <summary>
        /// Выполняет текущие задачи по преобретению, используя для предотвращения дублирования работы
        /// </summary>
        private readonly ConcurrentDictionary<string, Lazy<Task<object>>> _ongoing = new();

        #endregion

        #region Конструктор

        public DistributedCacheManager(AppSettings appSettings,
            IDistributedCache distributedCache,
            CacheKeyManager cacheKeyManager)
            : base(appSettings)
        {
            _distributedCache = distributedCache;
            _localKeyManager = cacheKeyManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Очистить все данные в этом экземпляре
        /// </summary>
        /// <returns>Задача, которая представляет асинхронную операцию</returns>
        protected void ClearInstanceData()
        {
            _perRequestCache.Clear();
            _localKeyManager.Clear();
        }

        /// <summary>
        /// Очистить элементы по префиксу ключа кэша
        /// </summary>
        /// <param name="prefix">Префикс ключа кэша</param>
        /// <param name="prefixParameters">Параметры для создания префикса ключа кэша</param>
        /// <returns>Удалённые ключи</returns>
        protected IEnumerable<string> RemoveByPrefixInstanceData(string prefix, params object[] prefixParameters)
        {
            var prefix_ = PrepareKeyPrefix(prefix, prefixParameters);
            _perRequestCache.Prune(prefix_, out _);
            return _localKeyManager.RemoveByPrefix(prefix_);
        }

        /// <summary>
        /// Подготовка параметров ввода в кэш для переданного ключа
        /// </summary>
        /// <param name="key">Ключ кэша</param>
        /// <returns>Опции ввода кэша</returns>
        private static DistributedCacheEntryOptions PrepareEntryOptions(CacheKey key)
        {
            //set expiration time for the passed cache key
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
            };
        }

        private void SetLocal(string key, object value)
        {
            _perRequestCache.Add(key, value);
            _localKeyManager.AddKey(key);
        }

        private void RemoveLocal(string key)
        {
            _perRequestCache.Remove(key);
            _localKeyManager.RemoveKey(key);
        }

        private async Task<(bool isSet, T item)> TryGetItemAsync<T>(string key)
        {
            var json = await _distributedCache.GetStringAsync(key);
            return string.IsNullOrEmpty(json)
              ? (false, default)
              : (true, item: JsonConvert.DeserializeObject<T>(json));
        }

        protected async Task RemoveAsync(string key, bool removeFromInstance = true)
        {
            _ongoing.TryRemove(key, out _);
            await _distributedCache.RemoveAsync(key);
            if (removeFromInstance)
                RemoveLocal(key);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Удаление из кэша значения с указанным ключом
        /// </summary>
        /// <param name="cacheKey">Ключ кэша</param>
        /// <param name="cacheKeyParameters">Параметры для создания ключа кэша</param>
        /// <returns>Задача, которая представляет асинхронную операцию</returns>
        public async Task RemoveAsync(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            await RemoveAsync(PrepareKey(cacheKey, cacheKeyParameters).Key);
        }

        /// <summary>
        /// Получает закэшированный элемент. Если его ещё нет в кэше, тогда загружает и кэшируется
        /// </summary>
        /// <typeparam name="T">Тип кэшированного элемента</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <param name="acquire">Функция для загрузки элемента если он ещё не был закэширован</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию
        /// Результат задачи содержит результат кэширования значения, ассоциированного с указанным ключом
        /// </returns>
        public async Task<T> GetAsync<T>(CacheKey key, Func<Task<T>> acquire)
        {
            if (_perRequestCache.TryGetValue(key.Key, out var data))
                return (T)data;
            var lazy = _ongoing.GetOrAdd(key.Key, _ => new(async () => await acquire(), true));
            var setTask = Task.CompletedTask;
            try
            {
                if (lazy.IsValueCreated)
                    return (T)await lazy.Value;
                var (isSet, item) = await TryGetItemAsync<T>(key.Key);
                if (!isSet)
                {
                    item = (T)await lazy.Value;
                    setTask = _distributedCache.SetStringAsync(
                        key.Key,
                        JsonConvert.SerializeObject(item),
                        PrepareEntryOptions(key));
                }
                SetLocal(key.Key, item);
                return item;
            }
            finally
            {
                _ = setTask.ContinueWith(_ => _ongoing.TryRemove(new KeyValuePair<string, Lazy<Task<object>>>(key.Key, lazy)));
            }
        }

        /// <summary>
        /// Получение закэшированного элемента. Если он ещё не был закэширован, тогда загрузить и закэшировать его
        /// </summary>
        /// <typeparam name="T">Тип кэшируемого элемента</typeparam>
        /// <param name="key">Ключ кэша</param>
        /// <param name="acquire">Функция для загрузки элемента, если он ещё не был закэширован</param>
        /// <returns>
        /// Задача, которая представляет асинхронную операцию
        /// Результат задачи содержит результат кэширования значения, ассоциированного с указанным ключом
        /// </returns>
        public Task<T> GetAsync<T>(CacheKey key, Func<T> acquire)
        {
            return GetAsync(key, () => Task.FromResult(acquire()));
        }

        public async Task<T> GetAsync<T>(CacheKey key, T defaultValue = default)
        {
            var value = await _distributedCache.GetStringAsync(key.Key);
            return value != null
                ? JsonConvert.DeserializeObject<T>(value)
                : defaultValue;
        }

        /// <summary>
        /// Добавление указанного ключа и объекта для кэширования
        /// </summary>
        /// <param name="key">Ключ кэшированного элемента</param>
        /// <param name="data">Значение для кэширования</param>
        /// <returns>Задача, которая представляет асинхронную операцию</returns>
        public async Task SetAsync<T>(CacheKey key, T data)
        {
            if (data == null || (key?.CacheTime ?? 0) <= 0)
                return;

            var lazy = new Lazy<Task<object>>(() => Task.FromResult(data as object), true);
            try
            {
                // дождатья отложенной задачи, чтобы принудительно создать значение, вместо прямой настройки данных таким образом,
                // чтобы другие экземпляры менеджера кэша могли получить к ним доступ во время настройки
                SetLocal(key.Key, await lazy.Value);
                _ongoing.TryAdd(key.Key, lazy);
                await _distributedCache.SetStringAsync(key.Key, JsonConvert.SerializeObject(data), PrepareEntryOptions(key));
            }
            finally
            {
                _ongoing.TryRemove(new KeyValuePair<string, Lazy<Task<object>>>(key.Key, lazy));
            }
        }

        /// <summary>
        /// Удаление элементов по префиксу ключа кэша
        /// </summary>
        /// <param name="prefix">Префикс ключа кэша</param>
        /// <param name="prefixParameters">Параметры для создания префикса ключа кэша</param>
        /// <returns>Задача, которая представляет асинхронную операцию</returns>
        public abstract Task RemoveByPrefixAsync(string prefix, params object[] prefixParameters);

        /// <summary>
        /// Очистить все данные кэша
        /// </summary>
        /// <returns>Задача, которая представляет асинхронную операцию</returns>
        public abstract Task ClearAsync();

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}